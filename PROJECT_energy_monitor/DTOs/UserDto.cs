namespace PowerMonitor.API.DTOs
{
    public record UserDto(string Username, string PasswordHash, string FullName, string Role);
}