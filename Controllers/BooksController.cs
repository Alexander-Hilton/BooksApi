using BooksApi.Models;
using BooksApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            var books = _bookRepository.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be a positive integer.");
            }
            Book returnedBook = _bookRepository.GetBookById(id);
            if (returnedBook == null)
            {
                return NotFound($"No book with ID {id} found");
            }
            return Ok(returnedBook);
        }

        [HttpPost]
        public ActionResult<Book> AddBook(Book book)
        {
            if (book == null)
            {
                return BadRequest("Book cannot be null");
            }

            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
            {
                return BadRequest("Book title and author are required");
            }

            Book addedBook = _bookRepository.AddBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = addedBook.Id }, addedBook);
        }

        [HttpPut]
        public ActionResult<Book> UpdateBook(int id, Book book)
        {

            if (book == null)
            {
                return BadRequest("Book cannot be null");
            }

            var updatedBook = _bookRepository.UpdateBook(id, book);
            if (updatedBook == null)
            {
                return NotFound($"No book with ID {id} found");
            }
            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID must be a positive integer.");
            }
            var deletedBook = _bookRepository.DeleteBook(id);
            if (!deletedBook) return NotFound($"No book with ID {id} found");
            return NoContent();

        }
    }
}