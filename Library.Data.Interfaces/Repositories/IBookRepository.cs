using LibraryReporter.Data.Interfaces.Models;

namespace LibraryReporter.Data.Interfaces.Repositories
{
    public interface IBookRepository<T> : IBaseRepository<T>
        where T : IBookData
    {
    }
}
