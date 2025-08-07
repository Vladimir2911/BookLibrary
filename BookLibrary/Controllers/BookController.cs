using BookLibrary.Data;
using BookLibrary.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace BookLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly BookRepository _repo;

        public BookController(ILogger<BookController> logger, BookRepository bookRepository)
        {
            _logger = logger;
            _repo = bookRepository;
        }

        public ActionResult Index()
        {
            var books = _repo.GetAllBooks();

            return View(books);
        }

        public ActionResult Details(int id)
        {
            var book = _repo.GetBookById(id);
            XDocument doc = XDocument.Parse(book.Contents);
            var titles = doc.Descendants("Section")
                          .Select(section => section.Attribute("title")?.Value)
                          .Where(title => title != null);
            book.Contents = string.Join(Environment.NewLine, titles);

            return View(book);
        }

        public ActionResult Create()
        {
            return View("Create", new Book());
        }

        [HttpPost]
        public ActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
                return View(book);

            _repo.InsertBook(book);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var book = _repo.GetBookById(id);
            XDocument doc = XDocument.Parse(book.Contents);
            var titles = doc.Descendants("Section")
                          .Select(section => section.Attribute("title")?.Value)
                          .Where(title => title != null);
            book.Contents = string.Join(Environment.NewLine, titles);

            return View(book);
        }

        [HttpPost]
        public ActionResult Edit(Book book)
        {
            if (!ModelState.IsValid)
                return View(book);

            _repo.UpdateBook(book);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var book = _repo.GetBookById(id);
            return View(book);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            _repo.DeleteBook(id);
            return RedirectToAction("Index");
        }
    }
}
