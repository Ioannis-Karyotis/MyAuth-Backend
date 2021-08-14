using Microsoft.AspNetCore.Identity;
using MyAuth.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Database
{
    public class MyAuthUser
    {

        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        [Required]
        [MaxLength(40)]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email{ get; set; }
        public string Password { get; set; }
        public TxtFile FaceDescriptor { get; set; }
        public bool HasFaceRegistered { get; set; }
        public DateTime Created{ get; set; }
        public virtual List<ExternalAppAuthUser> ExternalAppAuthUsers { get; set; }
        public virtual List<ExternalApp> ExternalApps{ get; set; }
    }
}
