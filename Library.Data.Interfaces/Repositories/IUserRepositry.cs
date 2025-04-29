using LibraryReporter.Data.Interfaces.Models;

namespace LibraryReporter.Data.Interfaces.Repositories
{
    public interface IUserRepositry<T> : IBaseRepository<T>
        where T : IUserData
    {
    }
}
