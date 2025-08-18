using CinemaApi.DTOs.Schedule;
using CinemaApi.DTOs;
using CinemaApi.DTOs.Studio;
using CinemaApi.DTOs.Movie;
using CinemaApi.Interfaces;
using CinemaApi.Models;

namespace CinemaApi.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IScheduleRepository scheduleRepository, ILogger<ScheduleService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            
            var scheduleDtos = schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                StudioId = s.StudioId,
                MovieId = s.MovieId,
                Studio = new StudioDto
                {
                    Id = s.Studio.Id,
                    Name = s.Studio.Name,
                    Capacity = s.Studio.Capacity,
                    Facilities = s.Studio.Facilities
                },
                Movie = new MovieDto
                {
                    Id = s.Movie.Id,
                    Title = s.Movie.Title,
                    Genre = s.Movie.Genre,
                    Duration = s.Movie.Duration,
                    Description = s.Movie.Description
                },
                ShowDateTime = s.ShowDateTime,
                TicketPrice = s.TicketPrice
            });

            return scheduleDtos;
        }

        public async Task<ScheduleDto?> GetScheduleByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            
            if (schedule == null)
            {
                return null;
            }

            return new ScheduleDto
            {
                Id = schedule.Id,
                StudioId = schedule.StudioId,
                MovieId = schedule.MovieId,
                Studio = new StudioDto
                {
                    Id = schedule.Studio.Id,
                    Name = schedule.Studio.Name,
                    Capacity = schedule.Studio.Capacity,
                    Facilities = schedule.Studio.Facilities
                },
                Movie = new MovieDto
                {
                    Id = schedule.Movie.Id,
                    Title = schedule.Movie.Title,
                    Genre = schedule.Movie.Genre,
                    Duration = schedule.Movie.Duration,
                    Description = schedule.Movie.Description
                },
                ShowDateTime = schedule.ShowDateTime,
                TicketPrice = schedule.TicketPrice
            };
        }

        public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleDto createScheduleDto)
        {
            _logger.LogInformation("Creating new schedule for Movie ID: {MovieId}, Studio ID: {StudioId}", 
                createScheduleDto.MovieId, createScheduleDto.StudioId);

            var schedule = new Schedule
            {
                StudioId = createScheduleDto.StudioId,
                MovieId = createScheduleDto.MovieId,
                ShowDateTime = createScheduleDto.ShowDateTime,
                TicketPrice = createScheduleDto.TicketPrice
            };

            var createdSchedule = await _scheduleRepository.CreateAsync(schedule);
            _logger.LogInformation("Schedule created successfully (ID: {ScheduleId})", createdSchedule.Id);

            return new ScheduleDto
            {
                Id = createdSchedule.Id,
                StudioId = createdSchedule.StudioId,
                MovieId = createdSchedule.MovieId,
                Studio = new StudioDto
                {
                    Id = createdSchedule.Studio.Id,
                    Name = createdSchedule.Studio.Name,
                    Capacity = createdSchedule.Studio.Capacity,
                    Facilities = createdSchedule.Studio.Facilities
                },
                Movie = new MovieDto
                {
                    Id = createdSchedule.Movie.Id,
                    Title = createdSchedule.Movie.Title,
                    Genre = createdSchedule.Movie.Genre,
                    Duration = createdSchedule.Movie.Duration,
                    Description = createdSchedule.Movie.Description
                },
                ShowDateTime = createdSchedule.ShowDateTime,
                TicketPrice = createdSchedule.TicketPrice
            };
        }

        public async Task<ScheduleDto?> UpdateScheduleAsync(int id, UpdateScheduleDto updateScheduleDto)
        {
            _logger.LogInformation("Updating schedule with ID: {ScheduleId}", id);

            if (!await _scheduleRepository.ExistsAsync(id))
            {
                return null;
            }

            var scheduleToUpdate = new Schedule
            {
                StudioId = updateScheduleDto.StudioId,
                MovieId = updateScheduleDto.MovieId,
                ShowDateTime = updateScheduleDto.ShowDateTime,
                TicketPrice = updateScheduleDto.TicketPrice
            };

            var updatedSchedule = await _scheduleRepository.UpdateAsync(id, scheduleToUpdate);
            if (updatedSchedule != null)
            {
                _logger.LogInformation("Schedule updated successfully (ID: {ScheduleId})", updatedSchedule.Id);
                
                return new ScheduleDto
                {
                    Id = updatedSchedule.Id,
                    StudioId = updatedSchedule.StudioId,
                    MovieId = updatedSchedule.MovieId,
                    Studio = new StudioDto
                    {
                        Id = updatedSchedule.Studio.Id,
                        Name = updatedSchedule.Studio.Name,
                        Capacity = updatedSchedule.Studio.Capacity,
                        Facilities = updatedSchedule.Studio.Facilities
                    },
                    Movie = new MovieDto
                    {
                        Id = updatedSchedule.Movie.Id,
                        Title = updatedSchedule.Movie.Title,
                        Genre = updatedSchedule.Movie.Genre,
                        Duration = updatedSchedule.Movie.Duration,
                        Description = updatedSchedule.Movie.Description
                    },
                    ShowDateTime = updatedSchedule.ShowDateTime,
                    TicketPrice = updatedSchedule.TicketPrice
                };
            }

            return null;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            if (!await _scheduleRepository.ExistsAsync(id))
            {
                return false;
            }

            var result = await _scheduleRepository.DeleteAsync(id);
            return result;
        }

        public async Task<bool> ScheduleExistsAsync(int id)
        {
            return await _scheduleRepository.ExistsAsync(id);
        }
    }
}