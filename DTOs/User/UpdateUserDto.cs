using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}