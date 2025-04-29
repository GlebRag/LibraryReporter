using Enums.Status;
using Enums.Users;
using System.Numerics;

namespace LibraryReporter.Data.Interfaces.Models
{
    public interface IBookData : IBaseModel
    {
        public string BookName { get; set; }
        public string Author { get; set; }

        public string Publisher { get; set; }
        //public string? UrlCover { get; set; }
        public string Barcode { get; set; }

        public Status Status { get; set; }
    }
}
