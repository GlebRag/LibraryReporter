using LibraryReporter.Data.Models;
using LibraryReporter.Data;
using Enums.Users;
using Microsoft.EntityFrameworkCore;
using LibraryReporter.Data.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using LibraryReporter.Data.Interfaces.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;
using System.Text;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Net;

namespace LibraryReporter.Data.Repositories
{
    public interface IBookRepositoryReal : IBookRepository<BookData>
    {
        //void BuyBook(BookData dataBook, int userId);
        void Create(BookData dataBook);
        IEnumerable<BookData> SearchBook(string name, string author, string publisher, string barcode);
        ////IEnumerable<BookData> GetBook(int userId);
        //void QuantityCounting(int bookId);
        //bool IsThisUserBoughtThisBook(int bookId, int userId);

        void Update(BookData dataBook, int bookId);
        //void UpdateCoverUrl(int bookId, string coverUrl);
        //void SaveBook(BookData dataBook, string? userName); //Для сохранения в Pdf
    }

    public class BookRepository : BaseRepository<BookData>, IBookRepositoryReal
    {
        public BookRepository(WebDbContext webDbContext) : base(webDbContext)
        {
        }

        //public void BuyBook(BookData dataBook, int userId)
        //{
        //    var bookId = dataBook.Id;
        //    // Создаем новую запись о покупке
        //    SalesData data = new SalesData
        //    {

        //        BookId = dataBook.Id,
        //        UserId = userId,
        //        Name = dataBook.Name,
        //        Author = dataBook.Author,
        //        Price = dataBook.Price,
        //        Publisher = dataBook.Publisher,
        //        Quantity = dataBook.Count,
        //        TimeOfSale = DateOnly.FromDateTime(DateTime.Now)

        //    };
        //    _webDbContext.Sales.Add(data);

        //    _webDbContext.SaveChanges();

        //    QuantityCounting(bookId);

        //}

        //public void QuantityCounting(int bookId)
        //{
        //    // Подсчитываем общее количество заказов для книги
        //    var countClients = _webDbContext.Sales
        //    .Where(x => x.BookId == bookId) // Находим все строки с указанным bookId
        //        .Sum(x => x.Quantity); // Суммируем значения Quantity

        //    // Находим книгу в таблице
        //    var book = _dbSet.First(x => x.Id == bookId);

        //    // Уменьшаем остаток книг с учетом общего количества заказов
        //    book.Count = book.InitialQuantity - countClients;

        //    // Сохраняем изменения в базе данных
        //    _webDbContext.SaveChanges();
        //}

        public void Create(BookData dataBook)
        {
            Add(dataBook);
        }

        ////public IEnumerable<BookData> GetBook(int userId)
        ////{
        ////    var result = _webDbContext.Users
        ////    .Where(u => u.Id == userId)
        ////    .Select(u => u.BooksWhichUserTakes);


        ////    return result.ToList();

        ////}

        //public bool IsThisUserBoughtThisBook(int bookId, int userId)
        //{
        //    var user = _webDbContext.Users.First(x => x.Id == userId);
        //    var book = _dbSet.First(x => x.Id == bookId);
        //    if(user.BooksWhichUserTakes.Contains(book))
        //    {
        //        return true;
        //    }
        //    return false;

        //}

        public IEnumerable<BookData> SearchBook(string name, string author, string publisher, string barcode)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.Books WHERE 1=1"); //Это условие всгеда истинно

            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND BookName = @BookName");
                parameters.Add(new SqlParameter("@BookName", name));
            }

            if (!string.IsNullOrEmpty(author))
            {
                sql.Append(" AND Author = @Author");
                parameters.Add(new SqlParameter("@Author", author));
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                sql.Append(" AND Publisher = @Publisher");
                parameters.Add(new SqlParameter("@Publisher", publisher));
            }

            if (!string.IsNullOrEmpty(barcode))
            {
                sql.Append(" AND Barcode = @Barcode");
                parameters.Add(new SqlParameter("@Barcode", barcode));
            }


            var result = _webDbContext
                .Database
                .SqlQueryRaw<BookData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }

        public void Update(BookData dataBook, int bookId)
        {
            var book = _dbSet.First(x => x.Id == bookId);
            book.BookName = dataBook.BookName;
            book.Author = dataBook.Author;
            book.Publisher = dataBook.Publisher;
            book.Barcode = dataBook.Barcode;
            book.Status = dataBook.Status;
            _webDbContext.SaveChanges();
        }

        //public void UpdateCoverUrl(int bookId, string coverUrl)
        //{
        //    var book = _dbSet.First(x => x.Id == bookId);
        //    book.UrlCover = coverUrl;
        //    _webDbContext.SaveChanges();
        //}


    }
}
