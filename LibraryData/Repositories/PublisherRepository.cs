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
using Library.Data.Models;

namespace LibraryReporter.Data.Repositories
{
    public interface IPublisherRepositoryReal : IPublisherRepository<PublisherData>
    {
        //void BuyBook(BookData dataBook, int userId);
        void Create(PublisherData dataPublisher);
        IEnumerable<PublisherData> SearchPublisher(string name, string city, string email);
        ////IEnumerable<BookData> GetBook(int userId);
        //void QuantityCounting(int bookId);
        //bool IsThisUserBoughtThisBook(int bookId, int userId);

        //void Update(BookData dataBook, int bookId);
        //void UpdateCoverUrl(int bookId, string coverUrl);
        //void SaveBook(BookData dataBook, string? userName); //Для сохранения в Pdf
    }

    public class PublisherRepository : BaseRepository<PublisherData>, IPublisherRepositoryReal
    {
        public PublisherRepository(WebDbContext webDbContext) : base(webDbContext)
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

        public void Create(PublisherData dataPublisher)
        {
            Add(dataPublisher);
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

        public IEnumerable<PublisherData> SearchPublisher(string name, string city, string email)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder("SELECT * FROM dbo.Publishers WHERE 1=1"); //Это условие всгеда истинно

            if (!string.IsNullOrEmpty(name))
            {
                sql.Append(" AND Name = @Name");
                parameters.Add(new SqlParameter("@Name", name));
            }

            if (!string.IsNullOrEmpty(city))
            {
                sql.Append(" AND City = @City");
                parameters.Add(new SqlParameter("@City", city));
            }

            if (!string.IsNullOrEmpty(email))
            {
                sql.Append(" AND Email = @Email");
                parameters.Add(new SqlParameter("@Email", email));
            }

            var result = _webDbContext
                .Database
                .SqlQueryRaw<PublisherData>(sql.ToString(), parameters.ToArray())
                .ToList();

            return result;
        }

        //public void Update(BookData dataBook, int bookId)
        //{
        //    var book = _dbSet.First(x => x.Id == bookId);
        //    book.Name = dataBook.Name;
        //    book.Author = dataBook.Author;
        //    book.UrlCover = dataBook.UrlCover;
        //    book.Publisher = dataBook.Publisher;
        //    book.Price = dataBook.Price;
        //    book.Count = dataBook.Count;
        //    _webDbContext.SaveChanges();
        //}

        //public void UpdateCoverUrl(int bookId, string coverUrl)
        //{
        //    var book = _dbSet.First(x => x.Id == bookId);
        //    book.UrlCover = coverUrl;
        //    _webDbContext.SaveChanges();
        //}


    }
}
