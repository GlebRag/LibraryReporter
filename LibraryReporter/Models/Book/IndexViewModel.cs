using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryReporter.Models.Book
{
    public class IndexViewModel
    {
        public string UserName { get; set; }

        public List<BookViewModel> Books { get; set; }
    }
}
