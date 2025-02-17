using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wine.Application.Dtos;
using Wine.Application.Services.Contracts;

namespace Wine.API.Controllers;

[ApiController]
[Route("api/wines")]
public class WinesController : ControllerBase
{
    private readonly IWineService _wineService;

    public WinesController(IWineService wineService)
    {
        _wineService = wineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var wines = await _wineService.GetAllAsync();
        return Ok(wines);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var wine = await _wineService.GetByIdAsync(id);
        return Ok(wine);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(WineDto wineDto)
    {
        var wineId = await _wineService.CreateAsync(wineDto);
        return Ok(new
        {
            Id = wineId
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, WineDto wineDto)
    {
        var updatedWine = await _wineService.UpdateAsync(id, wineDto);
        return Ok(updatedWine);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var isSuccessful = await _wineService.DeleteAsync(id);
        return Ok(isSuccessful);
    }
}