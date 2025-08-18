using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs;

namespace CinemaApi.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync();
        Task<ScheduleDto?> GetScheduleByIdAsync(int id);
        Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto);
        Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> ScheduleExistsAsync(int id);
    }
}