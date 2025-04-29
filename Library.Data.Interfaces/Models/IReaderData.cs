using LibraryReporter.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface IReaderData : IBaseModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly Birtday { get; set; }

        public DateOnly AddedDate { get; set; }
    }
}
