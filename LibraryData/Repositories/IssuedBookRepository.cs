using Library.Data.Interfaces.Models;
using Library.Data.Interfaces.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Data.Models;
using LibraryReporter.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Repositories
{
    public interface IIssuedBookRepositoryReal : IssuedBookRepository<IssuedBookData>
    {
        void AcceptanceBook(int bookId);

        //void BuyBook(BookData dataBook, int userId);
        //void Create(BookData dataBook);
        void IssueBook(IssuedBookData dataIssueBook, int bookId);
        //IEnumerable<BookData> SearchBook(string name, string author, string publisher, string barcode);
        ////IEnumerable<BookData> GetBook(int userId);
        //void QuantityCounting(int bookId);
        //bool IsThisUserBoughtThisBook(int bookId, int userId);

        //void Update(BookData dataBook, int bookId);
        //void UpdateCoverUrl(int bookId, string coverUrl);
        //void SaveBook(BookData dataBook, string? userName); //Для сохранения в Pdf
    }

    public class IssuedBookRepository : BaseRepository<IssuedBookData>, IIssuedBookRepositoryReal
    {
        public IssuedBookRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        public void AcceptanceBook(int bookId)
        {
            var issuedBook = _webDbContext.IssuedBooks.First(x => x.BookId == bookId);
            var issuedBookId = issuedBook.Id;
            Delete(issuedBookId);
            var book = _webDbContext.Books.First(x => x.Id == bookId);
            book.Status = Enums.Status.Status.InLibrary;
            _webDbContext.SaveChanges();
        }

        public void IssueBook(IssuedBookData dataIssueBook, int bookId)
        {
            Add(dataIssueBook);
            var book = _webDbContext.Books.First(x => x.Id == bookId);
            book.Status = Enums.Status.Status.InArmas;
            _webDbContext.SaveChanges();
        }
    }
}
