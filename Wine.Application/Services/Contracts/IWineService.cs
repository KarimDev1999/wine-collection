using Wine.Application.Dtos;

namespace Wine.Application.Services.Contracts;

public interface IWineService
{
   Task<IEnumerable<WineDto>> GetAllAsync();
   Task<WineDto> GetByIdAsync(int id);
   Task<int> CreateAsync(WineDto wineDto);
   Task<WineDto> UpdateAsync(int id, WineDto wineDto);
   Task<bool> DeleteAsync(int id);
   
   
   
}