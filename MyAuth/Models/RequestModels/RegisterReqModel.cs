using MyAuth.Utils.Extentions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.RequestModels
{
    public class RegisterReqModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string FaceDescriptor { get; set; }


        public bool HasValidPayload()
        {
            if (
                !Name.CheckAllCasesIsNotNull() ||
                !Surname.CheckAllCasesIsNotNull() ||
                !Email.IsValidEmail() ||
                !Password.ValidatePasswordRules() ||
                !ConfirmPassword.ValidatePasswordRules() ||
                Password != ConfirmPassword)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
