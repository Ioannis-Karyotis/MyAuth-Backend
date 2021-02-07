using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Database
{
    public class MyAuthUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string Surname { get; set; }

        public static explicit operator MyAuthUser(Task<IdentityUser> v)
        {
            throw new NotImplementedException();
        }
    }
}
