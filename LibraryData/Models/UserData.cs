using Enums.Users;
using LibraryReporter.Data.Interfaces.Models;

namespace LibraryReporter.Data.Models
{
    public class UserData : BaseModel, IUserData
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public Role Role { get; set; }

        public Language Language { get; set; }

            }
}
