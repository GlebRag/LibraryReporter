using Enums.Status;
using Enums.Users;
using LibraryReporter.Data.Interfaces.Models;
using System.Numerics;

namespace LibraryReporter.Data.Models
{
    public class BookData : BaseModel, IBookData
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        //public string? UrlCover { get; set; }
        public string Barcode { get; set; }

        public Status Status { get; set; }

    }
}
