using Library.Data.Repositories;
using LibraryReporter.Data.Repositories;
using LibraryReporter.Data;
using LibraryReporter.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryReporter.Models.Book;
using LibraryReporter.Models.LibraryReport;
using LibraryReporter.Models.Reader;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace LibraryReporter.Controllers
{
    public class LibraryReportController : Controller
    {
        private readonly ILogger<LibraryReportController> _logger;
        private AuthService _authService;
        private IBookRepositoryReal _bookRepository;
        private IIssuedBookRepositoryReal _issuedBookRepository;
        private IReaderRepositoryReal _readerRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepository;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public LibraryReportController(ILogger<LibraryReportController> logger,
            IBookRepositoryReal bookRepository,
            IAuthorRepositoryReal authorRepository,
            IIssuedBookRepositoryReal issuedBookRepository,
            IReaderRepositoryReal readerRepository,
            IPublisherRepositoryReal publisherRepository,
            WebDbContext webDbContext,
            IUserRepositryReal userRepository,
            AuthService authService,
            IWebHostEnvironment webHostEnvironment)
        {
            _issuedBookRepository = issuedBookRepository;
            _readerRepository = readerRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _webDbContext = webDbContext;
            _userRepository = userRepository;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public IActionResult IssuedBooks()
        {
            var books = _bookRepository.GetAll()
                .Where(book => book.Status == Enums.Status.Status.InArmas)
                .ToList();

            var issuedDatas = _issuedBookRepository.GetAll().ToList();

            // Совмещаем данные по совпадению идентификатора книги:
            // для каждой записи о выдаче находим соответствующую книгу.
            var issuedBooksViewModels = (from issued in issuedDatas
                                         join book in books on issued.BookId equals book.Id
                                         select new IssuedBookReportViewModel
                                         {
                                             ModerSurname = _userRepository.Get(issued.ModerId)?.Login,
                                             ReaderSurname = _readerRepository.Get(issued.ReaderId)?.Surname,
                                             BookName = book.BookName,
                                             Author = book.Author,
                                             Publisher = book.Publisher,
                                             Barcode = book.Barcode,
                                             IssueDate = issued.IssueDate,
                                             AcceptanceLastDate = issued.AcceptanceLastDate
                                         }).ToList();

            return View(issuedBooksViewModels);
        }


        public IActionResult InLibraryBooks()
        {
            var booksInLibrary = _bookRepository.GetAll()
                .Where(book => book.Status == Enums.Status.Status.InLibrary)
                .ToList();

            var inLibrarybooksViewModels = booksInLibrary
                .Select(dbBook =>
                    new InLibraryBookReportViewModel
                    {
                        Id = dbBook.Id,
                        BookName = dbBook.BookName,
                        Author = dbBook.Author,
                        Publisher = dbBook.Publisher,
                        Barcode = dbBook.Barcode
                    }
                )
                .ToList();

            return View(inLibrarybooksViewModels);

        }



        public IActionResult SearchIssueBookReport(string barcode, DateOnly issueDate, DateOnly acceptanceLastDate)
        {
            var books = _bookRepository.GetAll()
                .Where(book => book.Status == Enums.Status.Status.InArmas)
                .ToList();

            var issuedDatas = _issuedBookRepository.SearchIssuedBooks(issueDate, acceptanceLastDate).ToList();

            var issuedBooksViewModels = (from issued in issuedDatas
                                         join book in books on issued.BookId equals book.Id
                                         where string.IsNullOrEmpty(barcode) || book.Barcode == barcode // Фильтрация по barcode
                                         select new IssuedBookReportViewModel
                                         {
                                             ModerSurname = _userRepository.Get(issued.ModerId)?.Login,
                                             ReaderSurname = _readerRepository.Get(issued.ReaderId)?.Surname,
                                             BookName = book.BookName,
                                             Author = book.Author,
                                             Publisher = book.Publisher,
                                             Barcode = book.Barcode,
                                             IssueDate = issued.IssueDate,
                                             AcceptanceLastDate = issued.AcceptanceLastDate
                                         }).ToList();

            return View("IssuedBooks", issuedBooksViewModels);
        }

        public IActionResult SearchInLibraryBookReport(string name, string author, string publisher, string barcode)
        {
            var viewModels = _bookRepository
                .SearchBook(name, author, publisher, barcode)
                .Where(book => book.Status == Enums.Status.Status.InLibrary)
                .Select(dbBook => new InLibraryBookReportViewModel
                {
                    Id = dbBook.Id,
                    BookName = dbBook.BookName,
                    Author = dbBook.Author,
                    Publisher = dbBook.Publisher,
                    Barcode = dbBook.Barcode,
                })
                .ToList();

            return View("InLibraryBooks", viewModels);
        }


        [HttpPost]
        public IActionResult CreateReportForInLibraryBooks([FromBody] List<InLibraryBookReportViewModel> model)
        {
            // Проверяем, есть ли данные
            if (model == null || !model.Any())
            {
                return BadRequest("Данные отсутствуют.");
            }

            var uniquePublishers = model.Select(m => m.Publisher).Distinct().ToList();

            // Создаем документ Word
            using (var wordDoc = WordprocessingDocument.Create("Report.docx", WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();
                mainPart.Document.Append(body);

                // Заголовок "Отчет"
                var titleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text("Отчет")) { RunProperties = new RunProperties(new Bold()) }
                );
                body.Append(titleParagraph);

                // Подзаголовок с датами
                string dateSubtitleText;
                var dateToday = DateOnly.FromDateTime(DateTime.Now);
                dateSubtitleText = $"Отчет книг в библиотеке на {dateToday:dd.MM.yyyy}";
                var dateSubtitleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text(dateSubtitleText))
                );
                body.Append(dateSubtitleParagraph);


                // Добавляем издателя, если он один
                if (uniquePublishers.Count == 1)
                {
                    var publisherParagraph = new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(new Text($"Книги от издательства {uniquePublishers[0]}"))
                    );
                    body.Append(publisherParagraph);
                }

                // Разделение перед таблицей
                body.Append(new Paragraph(new Run(new Text(""))));

                // Добавляем таблицу
                var table = new Table();
                var tableProperties = new TableProperties(
                    new TableWidth { Width = "100%", Type = TableWidthUnitValues.Pct },
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 12 },
                        new BottomBorder { Val = BorderValues.Single, Size = 12 },
                        new LeftBorder { Val = BorderValues.Single, Size = 12 },
                        new RightBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 12 }
                    )
                );
                table.AppendChild(tableProperties);

                // Добавляем заголовки таблицы
                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("Название книги")))),
                    new TableCell(new Paragraph(new Run(new Text("Автор")))),
                    new TableCell(new Paragraph(new Run(new Text("Издатель")))),
                    new TableCell(new Paragraph(new Run(new Text("Штрихкод"))))
                );
                table.Append(headerRow);

                // Заполняем таблицу данными
                foreach (var order in model)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(order.BookName)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Author)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Publisher)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Barcode))))
                    );
                    table.Append(row);
                }

                body.Append(table);

                // Разделение перед подписью
                body.Append(new Paragraph(new Run(new Text(""))));

                // Подпись и дата
                var footerTable = new Table();
                footerTable.AppendChild(new TableRow(
                    new TableCell(new Paragraph(new Run(new Text($"Отчет создан сотрудником: {_authService.GetName()} Подпись: ____________  ")))),
                    new TableCell(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                        new Run(new Text($"Дата: {DateTime.Now:dd.MM.yyyy}")))
                    )
                ));
                body.Append(footerTable);

                // Сохраняем документ
                mainPart.Document.Save();
            }

            return File(System.IO.File.ReadAllBytes("Report.docx"), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Report.docx");
        }

        [HttpPost]
        public IActionResult CreateReportForIssuedBooks([FromBody] List<IssuedBookReportViewModel> model)
        {
            // Проверяем, есть ли данные
            if (model == null || !model.Any())
            {
                return BadRequest("Данные отсутствуют.");
            }

            // Находим минимальную и максимальную дату
            var minDate = model.Min(m => m.IssueDate);
            var maxDate = model.Max(m => m.IssueDate);

            // Проверяем количество уникальных фамилий
            var uniqueLastNames = model.Select(m => m.ModerSurname).Distinct().ToList();

            var uniquePublishers = model.Select(m => m.Publisher).Distinct().ToList();

            // Создаем документ Word
            using (var wordDoc = WordprocessingDocument.Create("Report.docx", WordprocessingDocumentType.Document))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();
                mainPart.Document.Append(body);

                // Заголовок "Отчет"
                var titleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text("Отчет")) { RunProperties = new RunProperties(new Bold()) }
                );
                body.Append(titleParagraph);

                // Подзаголовок с датами
                string dateSubtitleText;
                if (minDate == maxDate)
                {
                    dateSubtitleText = $"Отчет выданных книг за {minDate:dd.MM.yyyy}";
                }
                else
                {
                    dateSubtitleText = $"Отчет выданных книг с {minDate:dd.MM.yyyy} по {maxDate:dd.MM.yyyy}";
                }
                var dateSubtitleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text(dateSubtitleText))
                );
                body.Append(dateSubtitleParagraph);

                // Добавляем фамилию, если она одна
                if (uniqueLastNames.Count == 1)
                {
                    var lastNameParagraph = new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(new Text($"Выданные сотрудником {uniqueLastNames[0]}"))
                    );
                    body.Append(lastNameParagraph);
                }

                // Добавляем издателя, если он один
                if (uniquePublishers.Count == 1)
                {
                    var publisherParagraph = new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(new Text($"Книги от издательства {uniquePublishers[0]}"))
                    );
                    body.Append(publisherParagraph);
                }

                // Разделение перед таблицей
                body.Append(new Paragraph(new Run(new Text(""))));

                // Добавляем таблицу
                var table = new Table();
                var tableProperties = new TableProperties(
                    new TableWidth { Width = "100%", Type = TableWidthUnitValues.Pct },
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 12 },
                        new BottomBorder { Val = BorderValues.Single, Size = 12 },
                        new LeftBorder { Val = BorderValues.Single, Size = 12 },
                        new RightBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 12 }
                    )
                );
                table.AppendChild(tableProperties);

                // Добавляем заголовки таблицы
                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("Фамилия сотрудника")))),
                    new TableCell(new Paragraph(new Run(new Text("Фамилия читателя")))),
                    new TableCell(new Paragraph(new Run(new Text("Название книги")))),
                    new TableCell(new Paragraph(new Run(new Text("Автор")))),
                    new TableCell(new Paragraph(new Run(new Text("Издатель")))),
                    new TableCell(new Paragraph(new Run(new Text("Штрихкод")))),
                    new TableCell(new Paragraph(new Run(new Text("Дата выдачи")))),
                    new TableCell(new Paragraph(new Run(new Text("Дата возврата"))))
                );
                table.Append(headerRow);

                // Заполняем таблицу данными
                foreach (var order in model)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(order.ModerSurname)))),
                        new TableCell(new Paragraph(new Run(new Text(order.ReaderSurname)))),
                        new TableCell(new Paragraph(new Run(new Text(order.BookName)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Author)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Publisher)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Barcode)))),
                        new TableCell(new Paragraph(new Run(new Text(order.IssueDate.ToString("dd.MM.yyyy"))))),
                        new TableCell(new Paragraph(new Run(new Text(order.AcceptanceLastDate.ToString("dd.MM.yyyy")))))
                    );
                    table.Append(row);
                }

                body.Append(table);

                // Разделение перед подписью
                body.Append(new Paragraph(new Run(new Text(""))));

                // Подпись и дата
                var footerTable = new Table();
                footerTable.AppendChild(new TableRow(
                    new TableCell(new Paragraph(new Run(new Text($"Отчет создан сотрудником: {_authService.GetName()} Подпись: ____________  ")))),
                    new TableCell(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                        new Run(new Text($" Дата: {DateTime.Now:dd.MM.yyyy}")))
                    )
                ));
                body.Append(footerTable);

                // Сохраняем документ
                mainPart.Document.Save();
            }

            return File(System.IO.File.ReadAllBytes("Report.docx"), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Report.docx");
        }
    }
}
