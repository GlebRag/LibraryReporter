using Library.Data.Interfaces.Models;
using LibraryReporter.Data.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Repositories
{
    public interface IssuedBookRepository<T> : IBaseRepository<T>
        where T : IIssuedBookData
    {
    }
}
