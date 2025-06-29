namespace AuthServer.Application.DTOs
{
    public class RevokeTokenRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string? IpAddress { get; set; }
    }
}
