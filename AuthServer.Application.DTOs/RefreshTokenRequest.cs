namespace AuthServer.Application.DTOs
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string? IpAddress { get; set; }
    }
}
