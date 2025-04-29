using LibraryReporter.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface IPublisherData : IBaseModel
    {
        public string Name { get; set; }
        public string City { get; set; }

        public string Email { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}
