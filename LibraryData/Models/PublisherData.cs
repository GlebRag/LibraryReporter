using Library.Data.Interfaces.Models;
using LibraryReporter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Models
{
    public class PublisherData : BaseModel, IPublisherData
    {
        public string Name { get; set; }
        public string City { get; set; }

        public string Email { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}
