using Library.Data.Interfaces.Models;
using LibraryReporter.Data.Interfaces.Models;

namespace LibraryReporter.Data.Interfaces.Repositories
{
    public interface IPublisherRepository<T> : IBaseRepository<T>
        where T : IPublisherData
    {
    }
}
