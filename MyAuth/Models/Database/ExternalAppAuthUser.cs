using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Database
{
    public class ExternalAppAuthUser
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MyAuthUserId { get; set; }
        public Guid ExternalAppId { get; set; }
        public DateTime Created { get; set; }
        public virtual ExternalApp ExternalApp{ get; set; }
        public virtual MyAuthUser AuthUser{ get; set; }
    }
}
