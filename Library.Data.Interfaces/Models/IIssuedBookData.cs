using LibraryReporter.Data.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Interfaces.Models
{
    public interface IIssuedBookData : IBaseModel
    {
        public int ModerId { get; set; } // Кто выдал книгу
        public int BookId { get; set; } // Книга
        public int ReaderId { get; set; } // Кому выдана
        /// <summary>
        /// Последний день - дата выдачи = срок на котороый выдается книга, по умолчанию это будет 2 недели
        /// </summary>
        public DateOnly IssueDate { get; set; } // Дата выдачи книги
        public DateOnly AcceptanceLastDate { get; set; } // Последний день принятия книги
    }
}
