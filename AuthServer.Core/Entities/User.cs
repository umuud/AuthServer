using Microsoft.AspNetCore.Identity;
using System;

namespace AuthServer.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// Kullanıcının adı
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Kullanıcının soyadı
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Hesabın aktif olup olmadığını gösterir
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Hesabın oluşturulma zamanı (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Son başarılı giriş zamanı (UTC)
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Profil güncellenme zamanı (UTC)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
