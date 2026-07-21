using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPP_Compras.Models.DTOs.DeliverySchedules;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Controllers;

[ApiController]
[Authorize]
[Route("api/delivery-schedules")]
public sealed class DeliverySchedulesController
    : ControllerBase
{
    private readonly IDeliveryScheduleService
        _deliveryScheduleService;

    public DeliverySchedulesController(
        IDeliveryScheduleService deliveryScheduleService)
    {
        _deliveryScheduleService =
            deliveryScheduleService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DeliveryScheduleResponse>> Create(
        [FromBody] CreateDeliveryScheduleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            DeliveryScheduleResponse schedule =
                await _deliveryScheduleService.CreateAsync(
                    request,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = schedule.Id },
                schedule);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet]
    public async Task<
        ActionResult<
            IReadOnlyCollection<DeliveryScheduleResponse>>> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool onlyFuture = true,
        CancellationToken cancellationToken = default)
    {
        if (includeInactive &&
            !User.IsInRole("Admin"))
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    message =
                        "Somente administradores podem consultar horários inativos."
                });
        }

        IReadOnlyCollection<DeliveryScheduleResponse> schedules =
            await _deliveryScheduleService.GetAllAsync(
                includeInactive,
                onlyFuture,
                cancellationToken);

        return Ok(schedules);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryScheduleResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            DeliveryScheduleResponse schedule =
                await _deliveryScheduleService.GetByIdAsync(
                    id,
                    cancellationToken);

            if (!schedule.IsActive &&
                !User.IsInRole("Admin"))
            {
                return NotFound(new
                {
                    message =
                        "Horário de entrega não encontrado."
                });
            }

            return Ok(schedule);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DeliveryScheduleResponse>> Update(
        int id,
        [FromBody] UpdateDeliveryScheduleRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            DeliveryScheduleResponse schedule =
                await _deliveryScheduleService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(schedule);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _deliveryScheduleService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
}