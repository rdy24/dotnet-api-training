using CinemaApi.DTOs.Studio;

namespace CinemaApi.Interfaces
{
    public interface IStudioService
    {
        Task<IEnumerable<StudioDto>> GetAllStudiosAsync();
        Task<StudioDto?> GetStudioByIdAsync(int id);
        Task<StudioDto> CreateStudioAsync(CreateStudioDto createStudioDto);
        Task<StudioDto?> UpdateStudioAsync(int id, UpdateStudioDto updateStudioDto);
        Task<bool> DeleteStudioAsync(int id);
        Task<bool> StudioExistsAsync(int id);
    }
}