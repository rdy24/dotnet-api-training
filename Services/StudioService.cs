using CinemaApi.DTOs.Studio;
using CinemaApi.Interfaces;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public class StudioService : IStudioService
    {
        private readonly IStudioRepository _studioRepository;
        private readonly ILogger<StudioService> _logger;

        public StudioService(IStudioRepository studioRepository, ILogger<StudioService> logger)
        {
            _studioRepository = studioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<StudioDto>> GetAllStudiosAsync()
        {
            var studios = await _studioRepository.GetAllAsync();
            
            var studioDtos = studios.Select(s => new StudioDto
            {
                Id = s.Id,
                Name = s.Name,
                Capacity = s.Capacity,
                Facilities = s.Facilities
            });

            return studioDtos;
        }

        public async Task<StudioDto?> GetStudioByIdAsync(int id)
        {
            var studio = await _studioRepository.GetByIdAsync(id);
            
            if (studio == null)
            {
                _logger.LogWarning("Studio not found with ID: {StudioId}", id);
                return null;
            }

            _logger.LogInformation("Studio found: {StudioName} (ID: {StudioId})", studio.Name, studio.Id);
            return new StudioDto
            {
                Id = studio.Id,
                Name = studio.Name,
                Capacity = studio.Capacity,
                Facilities = studio.Facilities
            };
        }

        public async Task<StudioDto> CreateStudioAsync(CreateStudioDto createStudioDto)
        {
            _logger.LogInformation("Creating new studio: {StudioName}", createStudioDto.Name);

            var studio = new Studio
            {
                Name = createStudioDto.Name,
                Capacity = createStudioDto.Capacity,
                Facilities = createStudioDto.Facilities
            };

            var createdStudio = await _studioRepository.CreateAsync(studio);
            _logger.LogInformation("Studio created successfully: {StudioName} (ID: {StudioId})", 
                createdStudio.Name, createdStudio.Id);

            return new StudioDto
            {
                Id = createdStudio.Id,
                Name = createdStudio.Name,
                Capacity = createdStudio.Capacity,
                Facilities = createdStudio.Facilities
            };
        }

        public async Task<StudioDto?> UpdateStudioAsync(int id, UpdateStudioDto updateStudioDto)
        {
            _logger.LogInformation("Updating studio with ID: {StudioId}", id);

            if (!await _studioRepository.ExistsAsync(id))
            {
                return null;
            }

            var studioToUpdate = new Studio
            {
                Name = updateStudioDto.Name,
                Capacity = updateStudioDto.Capacity,
                Facilities = updateStudioDto.Facilities
            };

            var updatedStudio = await _studioRepository.UpdateAsync(id, studioToUpdate);
            if (updatedStudio != null)
            {
                _logger.LogInformation("Studio updated successfully: {StudioName} (ID: {StudioId})", 
                    updatedStudio.Name, updatedStudio.Id);
                
                return new StudioDto
                {
                    Id = updatedStudio.Id,
                    Name = updatedStudio.Name,
                    Capacity = updatedStudio.Capacity,
                    Facilities = updatedStudio.Facilities
                };
            }

            return null;
        }

        public async Task<bool> DeleteStudioAsync(int id)
        {
            if (!await _studioRepository.ExistsAsync(id))
            {
                return false;
            }

            var result = await _studioRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> StudioExistsAsync(int id)
        {
            return await _studioRepository.ExistsAsync(id);
        }
    }
}