using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Wine.Application.Dtos;
using Wine.Application.Services.Contracts;
using Wine.Domain.Contracts;
using Wine.Domain.Exceptions;

namespace Wine.Application.Services;

public class WineService : IWineService
{
    private readonly IWineRepository _wineRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<WineDto> _wineValidator;
    private readonly ILogger _logger;

    public WineService(IWineRepository wineRepository, IMapper mapper, IValidator<WineDto> wineValidator, ILogger<WineService> logger)
    {
        _wineRepository = wineRepository;
        _mapper = mapper;
        _wineValidator = wineValidator;
        _logger = logger;
    }
    public async Task<IEnumerable<WineDto>> GetAllAsync()
    {
        _logger.LogInformation("WineService.GetAllAsync started;");
        var wines = await _wineRepository.GetAllAsync();
        var wineDtos = _mapper.Map<IEnumerable<Domain.Entities.Wine>, IEnumerable<WineDto>>(wines);
        _logger.LogInformation("WineService.GetAllAsync finished;");
        return wineDtos;
    }

    public async Task<WineDto> GetByIdAsync(int id)
    {
        _logger.LogInformation($"WineService.GetByIdAsync started; wineId: {id}");
        var wine = await _wineRepository.GetByIdAsync(id);
        if (wine == null)
        {
            throw new EntityNotFoundException($"Unable to find wine with id : {id}");
        }
        var wineDto = _mapper.Map<Domain.Entities.Wine, WineDto>(wine);
        _logger.LogInformation($"WineService.GetByIdAsync finished;");
        return wineDto;

    }

    public async Task<int> CreateAsync(WineDto wineDto)
    {
        _logger.LogInformation($"WineService.CreateAsync started; title: {wineDto?.Title}");
        var validationResult = await _wineValidator.ValidateAsync(wineDto);
        if (!validationResult.IsValid)
        {
           throw new ValidationException(validationResult.Errors);
        }

        var wine = _mapper.Map<WineDto, Domain.Entities.Wine>(wineDto);
        var createdWine = await _wineRepository.AddAsync(wine);
        _logger.LogInformation($"WineService.CreateAsync finished;"); 
        return createdWine.Id;
    }

    public async Task<WineDto> UpdateAsync(int id, WineDto wineDto)
    {
        _logger.LogInformation($"WineService.UpdateAsync started; id: {id}");
        var validationResult = await _wineValidator.ValidateAsync(wineDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        var wineToUpdate = await _wineRepository.GetByIdAsync(id);
        if (wineToUpdate == null)
        {
            throw new EntityNotFoundException($"Unable to find wine with id : {id}, title: {wineDto.Title}");
        }

        var updatedWine = await _wineRepository.UpdateAsync(wineToUpdate);
        var updatedWineDto = _mapper.Map<Domain.Entities.Wine, WineDto>(updatedWine); 
        _logger.LogInformation($"WineService.UpdateAsync finished;");
        return updatedWineDto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation($"WineService.DeleteAsync started; id : {id}");
        var isSuccessful = await _wineRepository.DeleteAsync(id);
        return isSuccessful;
    }
}