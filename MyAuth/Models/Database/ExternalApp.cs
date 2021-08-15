using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Database
{
    public class ExternalApp
    {
        [Key]
        public Guid Id { get; set; }
        public string AppName { get; set; }
        public string BaseUrl { get; set; }
        public Guid MyAuthUserId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual List<ExternalAppAuthUser> ExternalAppAuthUsers{ get; set; }
        public virtual MyAuthUser AuthUserOwner { get; set; }
    }
}
