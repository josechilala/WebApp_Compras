using WebAPP_Compras.Models.DTOs.DeliverySchedules;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class DeliveryScheduleService
    : IDeliveryScheduleService
{
    private readonly IDeliveryScheduleRepository
        _deliveryScheduleRepository;

    public DeliveryScheduleService(
        IDeliveryScheduleRepository deliveryScheduleRepository)
    {
        _deliveryScheduleRepository =
            deliveryScheduleRepository;
    }

    public async Task<DeliveryScheduleResponse> CreateAsync(
        CreateDeliveryScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateSchedule(
            request.DeliveryDate,
            request.StartTime,
            request.EndTime,
            request.MaximumOrders,
            reservedOrders: 0);

        bool hasOverlap =
            await _deliveryScheduleRepository
                .HasOverlappingScheduleAsync(
                    request.DeliveryDate,
                    request.StartTime,
                    request.EndTime,
                    ignoredScheduleId: null,
                    cancellationToken);

        if (hasOverlap)
        {
            throw new InvalidOperationException(
                "Já existe um horário de entrega ativo que se sobrepõe ao período informado.");
        }

        var deliverySchedule = new DeliverySchedule
        {
            DeliveryDate = request.DeliveryDate.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MaximumOrders = request.MaximumOrders,
            ReservedOrders = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _deliveryScheduleRepository.AddAsync(
            deliverySchedule,
            cancellationToken);

        await _deliveryScheduleRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(deliverySchedule);
    }

    public async Task<
        IReadOnlyCollection<DeliveryScheduleResponse>> GetAllAsync(
        bool includeInactive,
        bool onlyFuture,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<DeliverySchedule> schedules =
            await _deliveryScheduleRepository.GetAllAsync(
                includeInactive,
                onlyFuture,
                cancellationToken);

        return schedules
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<DeliveryScheduleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        DeliverySchedule? schedule =
            await _deliveryScheduleRepository.GetByIdAsync(
                id,
                includeInactive: true,
                cancellationToken);

        if (schedule is null)
        {
            throw new KeyNotFoundException(
                "Horário de entrega não encontrado.");
        }

        return MapToResponse(schedule);
    }

    public async Task<DeliveryScheduleResponse> UpdateAsync(
        int id,
        UpdateDeliveryScheduleRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        DeliverySchedule? schedule =
            await _deliveryScheduleRepository.GetTrackedByIdAsync(
                id,
                includeInactive: false,
                cancellationToken);

        if (schedule is null)
        {
            throw new KeyNotFoundException(
                "Horário de entrega não encontrado ou inativo.");
        }

        ValidateSchedule(
            request.DeliveryDate,
            request.StartTime,
            request.EndTime,
            request.MaximumOrders,
            schedule.ReservedOrders);

        bool hasOverlap =
            await _deliveryScheduleRepository
                .HasOverlappingScheduleAsync(
                    request.DeliveryDate,
                    request.StartTime,
                    request.EndTime,
                    ignoredScheduleId: id,
                    cancellationToken);

        if (hasOverlap)
        {
            throw new InvalidOperationException(
                "Já existe um horário de entrega ativo que se sobrepõe ao período informado.");
        }

        schedule.DeliveryDate = request.DeliveryDate.Date;
        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.MaximumOrders = request.MaximumOrders;
        schedule.UpdatedAt = DateTime.UtcNow;

        _deliveryScheduleRepository.Update(schedule);

        await _deliveryScheduleRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(schedule);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        DeliverySchedule? schedule =
            await _deliveryScheduleRepository.GetTrackedByIdAsync(
                id,
                includeInactive: false,
                cancellationToken);

        if (schedule is null)
        {
            throw new KeyNotFoundException(
                "Horário de entrega não encontrado ou já inativo.");
        }

        if (schedule.ReservedOrders > 0)
        {
            throw new InvalidOperationException(
                "Não é possível desativar um horário que possui pedidos reservados.");
        }

        schedule.IsActive = false;
        schedule.UpdatedAt = DateTime.UtcNow;

        _deliveryScheduleRepository.Update(schedule);

        await _deliveryScheduleRepository.SaveChangesAsync(
            cancellationToken);
    }

    private static void ValidateSchedule(
        DateTime deliveryDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int maximumOrders,
        int reservedOrders)
    {
        if (deliveryDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException(
                "A data de entrega não pode ser anterior à data atual.");
        }

        if (startTime < TimeSpan.Zero ||
            startTime >= TimeSpan.FromDays(1))
        {
            throw new ArgumentException(
                "O horário inicial é inválido.");
        }

        if (endTime <= TimeSpan.Zero ||
            endTime > TimeSpan.FromDays(1))
        {
            throw new ArgumentException(
                "O horário final é inválido.");
        }

        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "O horário final deve ser posterior ao horário inicial.");
        }

        DateTime scheduleEnd =
            deliveryDate.Date.Add(endTime);

        if (scheduleEnd <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "O horário de entrega informado já passou.");
        }

        if (maximumOrders <= 0)
        {
            throw new ArgumentException(
                "A quantidade máxima de pedidos deve ser maior que zero.");
        }

        if (maximumOrders < reservedOrders)
        {
            throw new InvalidOperationException(
                "A quantidade máxima não pode ser menor que o número de pedidos já reservados.");
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "O identificador do horário de entrega é inválido.");
        }
    }

    private static DeliveryScheduleResponse MapToResponse(
        DeliverySchedule schedule)
    {
        int availableOrders = Math.Max(
            schedule.MaximumOrders -
            schedule.ReservedOrders,
            0);

        return new DeliveryScheduleResponse
        {
            Id = schedule.Id,
            DeliveryDate = schedule.DeliveryDate,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            MaximumOrders = schedule.MaximumOrders,
            ReservedOrders = schedule.ReservedOrders,
            AvailableOrders = availableOrders,
            HasAvailability =
                schedule.IsActive &&
                availableOrders > 0 &&
                schedule.DeliveryDate.Date
                    .Add(schedule.EndTime) >
                DateTime.UtcNow,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt,
            IsActive = schedule.IsActive
        };
    }
}