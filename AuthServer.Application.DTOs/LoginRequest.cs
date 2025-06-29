namespace AuthServer.Application.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
    }
}
