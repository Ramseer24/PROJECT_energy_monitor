using System.ComponentModel.DataAnnotations;

namespace PowerMonitor.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  // У реальності використовуйте хешування
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "operator";  // admin, operator, viewer
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}