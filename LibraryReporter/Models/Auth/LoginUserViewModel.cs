using LibraryReporter.Models.CustomValidationAttrubites;
using System.ComponentModel.DataAnnotations;

namespace LibraryReporter.Models.Auth
{
    public class LoginUserViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [LoginAndPasswordIsCorrect]
        public string Password { get; set; }
    }
}
