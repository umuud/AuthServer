namespace AuthServer.Core.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime Expires { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;

        public DateTime Created { get; set; }

        public string CreatedByIp { get; set; } = null!;

        public DateTime? Revoked { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }

        public bool IsActive => Revoked == null && !IsExpired;

        // Foreign key
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

