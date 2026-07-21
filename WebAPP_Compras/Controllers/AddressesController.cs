using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPP_Compras.Models.DTOs.Addresses;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Controllers;

[ApiController]
[Authorize]
[Route("api/addresses")]
public sealed class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpPost]
    public async Task<ActionResult<AddressResponse>> Create(
        [FromBody] CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            AddressResponse address =
                await _addressService.CreateAsync(
                    request,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = address.Id },
                address);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet]
    public async Task<
        ActionResult<IReadOnlyCollection<AddressResponse>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AddressResponse> addresses =
            await _addressService.GetAllAsync(
                includeInactive,
                cancellationToken);

        return Ok(addresses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AddressResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            AddressResponse address =
                await _addressService.GetByIdAsync(
                    id,
                    cancellationToken);

            return Ok(address);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AddressResponse>> Update(
        int id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            AddressResponse address =
                await _addressService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(address);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _addressService.DeleteAsync(
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
    }
}