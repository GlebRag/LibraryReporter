using Library.Data.Interfaces.Models;
using Library.Data.Interfaces.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Data.Interfaces.Repositories;
using LibraryReporter.Data.Models;
using LibraryReporter.Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.Data.Repositories
{
    public interface IIssuedBookRepositoryReal : IssuedBookRepository<IssuedBookData>
    {
        void AcceptanceBook(int bookId);

        //void BuyBook(BookData dataBook, int userId);
        //void Create(BookData dataBook);
        void IssueBook(IssuedBookData dataIssueBook, int bookId);
        IEnumerable<IssuedBookData> SearchIssuedBooks(DateOnly issueDate, DateOnly acceptanceLastDate);
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

        public IEnumerable<IssuedBookData> SearchIssuedBooks(DateOnly issueDate, DateOnly acceptanceLastDate)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.IssuedBooks WHERE 1=1");

            if (issueDate != DateOnly.MinValue && acceptanceLastDate != DateOnly.MinValue)
            {
                sql.Append(" AND issueDate BETWEEN @IssueDate AND @AcceptanceLastDate");
                parameters.Add(new SqlParameter("@IssueDate", issueDate));
                parameters.Add(new SqlParameter("@AcceptanceLastDate", acceptanceLastDate));
            }
            else if (issueDate != DateOnly.MinValue)
            {
                sql.Append(" AND issueDate >= @IssueDate");
                parameters.Add(new SqlParameter("@IssueDate", issueDate));
            }
            else if (acceptanceLastDate != DateOnly.MinValue)
            {
                sql.Append(" AND issueDate <= @AcceptanceLastDate");
                parameters.Add(new SqlParameter("@AcceptanceLastDate", acceptanceLastDate));
            }
            else
            {
                sql.Append(" AND issueDate IS NOT NULL");
            }

            var result = _webDbContext
                .Database
                .SqlQueryRaw<IssuedBookData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }


    }
}
