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
using Enums.Action;
using DocumentFormat.OpenXml.Bibliography;

namespace LibraryReporter.Controllers
{
    public class LibraryReportController : Controller
    {
        private readonly ILogger<LibraryReportController> _logger;
        private AuthService _authService;
        private IBookRepositoryReal _bookRepository;
        private IIssuedBookRepositoryReal _issuedBookRepository;
        private IActionsHistoryRepositoryReal _actionsHistoryRepository;
        private IReaderRepositoryReal _readerRepository;
        private IAuthorRepositoryReal _authorRepository;
        private IPublisherRepositoryReal _publisherRepository;
        private IUserRepositryReal _userRepository;
        private WebDbContext _webDbContext;
        private IWebHostEnvironment _webHostEnvironment;

        public LibraryReportController(ILogger<LibraryReportController> logger,
            IBookRepositoryReal bookRepository,
            IActionsHistoryRepositoryReal actionsHistoryRepository,
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
            _actionsHistoryRepository = actionsHistoryRepository;
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



        public IActionResult ActionsHistory()
        {
            var actions = _actionsHistoryRepository.GetAll()
                .ToList();

            var actionsViewModels = actions
                .Select(dbActions =>
                    new ActionsReportViewModel
                    {
                        ModerSurname = dbActions.ModerSurname,
                        Action = ActionsHelper.GetActionDescription(dbActions.Actions),
                        Description = dbActions.Description,
                        ExecutionDate = dbActions.ExecutionDate
                    }
                )
                .ToList();

            return View(actionsViewModels);

        }


        public IActionResult SearchActionsReport(Actions action, DateOnly dateFrom, DateOnly dateTo)
        {

            var actionsViewModels = _actionsHistoryRepository
                .SearchAction(action, dateFrom, dateTo)
                .Select(dbActions => new ActionsReportViewModel
                {
                    ModerSurname = dbActions.ModerSurname,
                    Action = ActionsHelper.GetActionDescription(dbActions.Actions),
                    Description = dbActions.Description,
                    ExecutionDate = dbActions.ExecutionDate
                })
                .ToList();

            return View("ActionsHistory", actionsViewModels);
        }

        [HttpPost]
        public IActionResult CreateReportForInLibraryBooks([FromBody] List<InLibraryBookReportViewModel> model)
        {
            // Проверяем, есть ли данные
            if (model == null || !model.Any())
            {
                return BadRequest("Данные отсутствуют.");
            }

            // Подсчитываем общее количество книг
            int totalInLibraryBooks = model.Count;

            // Функция для поиска наиболее часто встречающегося элемента.
            // Если ни один элемент не повторяется (все встречаются по 1 разу), возвращает "-"
            string FindMostFrequent(IEnumerable<string> items)
            {
                // Фильтруем null и пустые строки, чтобы избежать ошибок
                var filteredItems = items.Where(x => !string.IsNullOrEmpty(x));
                var grouped = filteredItems.GroupBy(x => x);
                if (!grouped.Any())
                    return "-";
                int maxCount = grouped.Max(g => g.Count());
                // Если максимальное количество равно 1, значит повторов нет
                if (maxCount == 1)
                    return "-";
                return grouped.OrderByDescending(g => g.Count()).First().Key;
            }

            // Вычисляем статистику
            string mostFrequentAuthor = FindMostFrequent(model.Select(m => m.Author));
            string mostFrequentPublisher = FindMostFrequent(model.Select(m => m.Publisher));
            string mostFrequentBook = FindMostFrequent(model.Select(m => m.BookName));

            // Устанавливаем сегодняшнюю дату в формате DateOnly
            var dateToday = DateOnly.FromDateTime(DateTime.Now);

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
                    new Run(new Text("Отчет"))
                    { RunProperties = new RunProperties(new Bold()) }
                );
                body.Append(titleParagraph);

                // Подзаголовок с датой
                var dateSubtitleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Отчет книг в библиотеке на {dateToday:dd.MM.yyyy}"))
                );
                body.Append(dateSubtitleParagraph);

                // Разделитель перед таблицей
                body.Append(new Paragraph(new Run(new Text(string.Empty))));

                // Создаем таблицу
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
                foreach (var book in model)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(book.BookName)))),
                        new TableCell(new Paragraph(new Run(new Text(book.Author)))),
                        new TableCell(new Paragraph(new Run(new Text(book.Publisher)))),
                        new TableCell(new Paragraph(new Run(new Text(book.Barcode))))
                    );
                    table.Append(row);
                }
                body.Append(table);

                // Добавляем сводную информацию (статистика) под таблицей

                // Абзац с количеством книг (жирным, по центру)
                var booksCountParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Всего книг в библиотеке: {totalInLibraryBooks} на {dateToday:dd.MM.yyyy}"))
                    { RunProperties = new RunProperties(new Bold()) }
                );
                body.Append(booksCountParagraph);

                // Абзац с самым часто встречающимся автором
                var statsParagraphAuthor = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Самый часто повторяющийся автор: {mostFrequentAuthor}"))
                );
                body.Append(statsParagraphAuthor);

                // Абзац с самым часто встречающимся издателем
                var statsParagraphPublisher = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Самый часто повторяющийся издатель: {mostFrequentPublisher}"))
                );
                body.Append(statsParagraphPublisher);

                // Абзац с самой часто выдаваемой (повторяющейся) книгой
                var statsParagraphBook = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Самая часто выдаваемая книга: {mostFrequentBook}"))
                );
                body.Append(statsParagraphBook);

                // Разделитель перед подписью
                body.Append(new Paragraph(new Run(new Text(string.Empty))));

                // Подпись и дата в футере документа
                var footerTable = new Table();
                footerTable.AppendChild(new TableRow(
                    new TableCell(new Paragraph(new Run(new Text($"Отчет создан сотрудником: {_authService.GetName()}  Подпись: ____________")))),
                    new TableCell(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                        new Run(new Text($"Дата: {DateTime.Now:dd.MM.yyyy}")))
                    )
                ));
                body.Append(footerTable);

                // Сохраняем документ
                mainPart.Document.Save();
            }

            // Отправляем созданный документ клиенту
            return File(
                System.IO.File.ReadAllBytes("Report.docx"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Report.docx"
            );
        }



        [HttpPost]
        public IActionResult CreateReportForIssuedBooks([FromBody] List<IssuedBookReportViewModel> model)
        {
            // Проверяем, есть ли данные
            if (model == null || !model.Any())
            {
                return BadRequest("Данные отсутствуют.");
            }

            // Подсчитываем количество книг
            int totalIssuedBooks = model.Count;

            // Определяем диапазон дат
            var issueDates = model.Select(m => m.IssueDate).OrderBy(date => date).ToList();
            string dateRange = issueDates.Count > 1
                ? $"За период: {issueDates.First():dd.MM.yyyy} по {issueDates.Last():dd.MM.yyyy}"
                : $"За {issueDates.First():dd.MM.yyyy}";

            // Функция для поиска наиболее часто встречающегося элемента.
            // Если ни один элемент не повторяется (максимальное количество повторов равно 1),
            // возвращаем прочерк ("-").
            string FindMostFrequent(IEnumerable<string> items)
            {
                var groups = items.GroupBy(x => x);
                if (!groups.Any())
                    return "-";
                int maxCount = groups.Max(g => g.Count());
                if (maxCount == 1)
                    return "-";
                return groups.OrderByDescending(g => g.Count()).First().Key;
            }

            // Вычисляем самые часто встречающиеся значения
            string mostFrequentAuthor = FindMostFrequent(model.Select(m => m.Author));
            string mostFrequentPublisher = FindMostFrequent(model.Select(m => m.Publisher));
            string mostFrequentModerator = FindMostFrequent(model.Select(m => m.ModerSurname));
            string mostFrequentBook = FindMostFrequent(model.Select(m => m.BookName));

            // Находим минимальную и максимальную дату для подзаголовка
            var minDate = model.Min(m => m.IssueDate);
            var maxDate = model.Max(m => m.IssueDate);

            // Проверяем количество уникальных фамилий сотрудника
            var uniqueLastNames = model.Select(m => m.ModerSurname).Distinct().ToList();
            // Проверяем количество уникальных издательств
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
                    new Run(new Text("Отчет"))
                    {
                        RunProperties = new RunProperties(new Bold())
                    }
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

                // Добавляем фамилию сотрудника, если она одна
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

                // Разделитель перед таблицей
                body.Append(new Paragraph(new Run(new Text(""))));

                // Создаем таблицу
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

                // Заголовок таблицы
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

                // Добавляем абзац с количеством книг (жирный текст)
                var booksCountParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Всего выданных книг: {totalIssuedBooks}"))
                    {
                        RunProperties = new RunProperties(new Bold())
                    }
                );
                body.Append(booksCountParagraph);

                // Абзац с диапазоном дат (жирный текст)
                var dateRangeParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text(dateRange))
                    {
                        RunProperties = new RunProperties(new Bold())
                    }
                );
                body.Append(dateRangeParagraph);

                // Добавляем статистику. Каждый параметр оформляем отдельным абзацем.
                var statsParagraphAuthor = new Paragraph(
                    new Run(new Text($"Самый часто повторяющийся автор: {mostFrequentAuthor}"))
                );
                body.Append(statsParagraphAuthor);

                var statsParagraphPublisher = new Paragraph(
                    new Run(new Text($"Самый часто повторяющийся издатель: {mostFrequentPublisher}"))
                );
                body.Append(statsParagraphPublisher);

                var statsParagraphBook = new Paragraph(
                    new Run(new Text($"Самая часто выдаваемая книга: {mostFrequentBook}"))
                );
                body.Append(statsParagraphBook);

                var statsParagraphModerator = new Paragraph(
                    new Run(new Text($"Самый активный сотрудник: {mostFrequentModerator}"))
                );
                body.Append(statsParagraphModerator);

                // Разделитель перед подписью
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

            return File(
                System.IO.File.ReadAllBytes("Report.docx"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Report.docx"
            );
        }





        [HttpPost]
        public IActionResult CreateReportForActions([FromBody] List<ActionsReportViewModel> model)
        {
            // Проверяем, есть ли данные
            if (model == null || !model.Any())
            {
                return BadRequest("Данные отсутствуют.");
            }

            // Находим минимальную и максимальную дату исполнения
            var minDate = model.Min(m => m.ExecutionDate);
            var maxDate = model.Max(m => m.ExecutionDate);

            // Подсчитываем общее количество событий
            int totalEvents = model.Count;

            // Формируем текст для сводной информации, зависящий от диапазона дат
            string summaryText;
            if (minDate == maxDate)
            {
                summaryText = $"Всего событий: {totalEvents} за {minDate:dd.MM.yyyy}";
            }
            else
            {
                summaryText = $"Всего событий: {totalEvents} за период: {minDate:dd.MM.yyyy} по {maxDate:dd.MM.yyyy}";
            }

            // Вычисляем самого активного сотрудника (группируем по фамилии сотрудника)
            var mostActiveGroup = model.GroupBy(m => m.ModerSurname)
                                       .OrderByDescending(g => g.Count())
                                       .FirstOrDefault();
            string mostActiveEmployee = mostActiveGroup != null ? mostActiveGroup.Key : "-";

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
                    new Run(new Text("Отчет"))
                    {
                        RunProperties = new RunProperties(new Bold())
                    }
                );
                body.Append(titleParagraph);

                // Подзаголовок с датами
                string dateSubtitleText;
                if (minDate == maxDate)
                {
                    dateSubtitleText = $"Отчет работы книжного фонда за {minDate:dd.MM.yyyy}";
                }
                else
                {
                    dateSubtitleText = $"Отчет работы книжного фонда с {minDate:dd.MM.yyyy} по {maxDate:dd.MM.yyyy}";
                }
                var dateSubtitleParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text(dateSubtitleText))
                );
                body.Append(dateSubtitleParagraph);

                // Разделитель перед таблицей
                body.Append(new Paragraph(new Run(new Text(""))));

                // Создаем таблицу
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

                // Заголовки таблицы
                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("Фамилия сотрудника")))),
                    new TableCell(new Paragraph(new Run(new Text("Событие")))),
                    new TableCell(new Paragraph(new Run(new Text("Описание")))),
                    new TableCell(new Paragraph(new Run(new Text("Дата исполнения"))))
                );
                table.Append(headerRow);

                // Заполняем таблицу данными
                foreach (var order in model)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(order.ModerSurname)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Action)))),
                        new TableCell(new Paragraph(new Run(new Text(order.Description)))),
                        new TableCell(new Paragraph(new Run(new Text(order.ExecutionDate.ToString("dd.MM.yyyy")))))
                    );
                    table.Append(row);
                }
                body.Append(table);

                // Добавляем сводную информацию – абзац с общим количеством событий и периодом
                var summaryParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text(summaryText))
                    {
                        RunProperties = new RunProperties(new Bold())
                    }
                );
                body.Append(summaryParagraph);

                // Абзац с самым активным сотрудником
                var activeEmployeeParagraph = new Paragraph(
                    new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                    new Run(new Text($"Самый активный сотрудник: {mostActiveEmployee}"))
                );
                body.Append(activeEmployeeParagraph);

                // Разделитель перед подписью
                body.Append(new Paragraph(new Run(new Text(string.Empty))));

                // Подпись и дата в футере документа
                var footerTable = new Table();
                footerTable.AppendChild(new TableRow(
                    new TableCell(new Paragraph(new Run(new Text($"Отчет создан сотрудником: {_authService.GetName()} Подпись: ____________")))),
                    new TableCell(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Right }),
                        new Run(new Text($" Дата: {DateTime.Now:dd.MM.yyyy}"))
                    ))
                ));
                body.Append(footerTable);

                // Сохраняем документ
                mainPart.Document.Save();
            }

            return File(
                System.IO.File.ReadAllBytes("Report.docx"),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Report.docx"
            );
        }


    }
}
