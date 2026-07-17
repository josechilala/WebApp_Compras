using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPP_Compras.Models.DTOs.Stores;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Controllers;

[ApiController]
[Route("api/stores")]
public sealed class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<StoreResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StoreResponse>>> GetAll(
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<StoreResponse> stores =
            await _storeService.GetAllAsync(
                includeInactive,
                cancellationToken);

        return Ok(stores);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(StoreResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            StoreResponse store =
                await _storeService.GetByIdAsync(
                    id,
                    cancellationToken);

            return Ok(store);
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
    [HttpPost]
    [ProducesResponseType(
        typeof(StoreResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StoreResponse>> Create(
        [FromBody] CreateStoreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            StoreResponse store =
                await _storeService.CreateAsync(
                    request,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = store.Id },
                store);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
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
    [ProducesResponseType(
        typeof(StoreResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StoreResponse>> Update(
        int id,
        [FromBody] UpdateStoreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            StoreResponse store =
                await _storeService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(store);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
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
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storeService.DeleteAsync(
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