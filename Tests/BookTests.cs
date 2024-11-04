using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using BooksApi.Repositories;
using BooksApi.Models;
using BooksApi.Controllers;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.BookTests
{
    public class BookRepositoryTests
    {

        private static AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            return new AppDbContext(options);
        }
        private readonly BookRepository _bookRepository;
        private readonly AppDbContext context;

        public BookRepositoryTests()
        {
            context = GetDbContext();
            Book book1 = new Book { Title = "Book1", Author = "Author1", ISBN = "1", PublishedDate = DateTime.Now };
            Book book2 = new Book { Title = "Book2", Author = "Author2", ISBN = "2", PublishedDate = DateTime.Now };
            context.Books.AddRange(book1, book2);
            _bookRepository = new BookRepository(context);
        }


        [Fact]
        public void GetAllBooks_ReturnsAllBooks()
        {
            var books = _bookRepository.GetAllBooks();

            Assert.NotNull(books);

        }

        [Fact]
        public void GetBookById_ReturnsCorrectBook()
        {
            var id = 3;
            var bookToAdd = new Book { Title = "Book", Author = "Author", ISBN = "12354", PublishedDate = DateTime.Now };
            context.Books.Add(bookToAdd);
            context.SaveChanges();

            var bookToFind = _bookRepository.GetBookById(bookToAdd.Id);

            Assert.Equal(id, bookToFind.Id);
        }

        [Fact]
        public void AddBook_AddsABook()
        {

            var book = new Book { Title = "Added Book", Author = "Added Author", ISBN = "234", PublishedDate = DateTime.Now };
            var addedBook = _bookRepository.AddBook(book);

            Assert.Equal("Added Book", addedBook.Title);
            Assert.Equal(3, addedBook.Id);
        }

        [Fact]
        public void UpdateBook_UpdatesABook()
        {

            int id = 2;
            var book = new Book { Title = "Updated Book1", Author = "Updated Author", ISBN = "2134234", PublishedDate = DateTime.Now };

            _bookRepository.UpdateBook(id, book);
            var updatedBook = context.Books.Find(id);


            Assert.Equal(book.Title, updatedBook.Title);

        }

        [Fact]
        public void DeleteBook_DeletesABook()
        {
            int id = 1;

            var deleted = _bookRepository.DeleteBook(id);
            var book = _bookRepository.GetBookById(id);

            Assert.True(deleted);
            Assert.Null(book);

        }


    }

    public class BookControllerTests
    {

        private readonly Mock<IBookRepository> _bookRepository;

        private readonly BooksController _booksController;

        private List<Book> GetSampleBooks()
        {
            return new List<Book>{
            new Book {Id = 1, Title = "Book One", Author = "Author 1"},
            new Book {Id = 2, Title = "Book Two", Author = "Author 2"}
        };
        }

        public BookControllerTests()
        {
            _bookRepository = new Mock<IBookRepository>();
            _booksController = new BooksController(_bookRepository.Object);
        }
        [Fact]
        public void GetAllBooks_ReturnsListOfBooks_AndOKResult()
        {
            _bookRepository.Setup(repo => repo.GetAllBooks()).Returns(GetSampleBooks());

            var result = _booksController.GetAllBooks();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
            Assert.Equal(2, books.Count());
        }

        [Fact]
        public void GetBookById_ReturnsOkResult_WithCorrectBook()
        {
            int id = 2;
            var book = new Book
            {
                Id = id,
                Title = "Title",
                Author = "Author"
            };
            _bookRepository.Setup(repo => repo.GetBookById(id)).Returns(book);

            var returnedBook = _booksController.GetBookById(id);

            var okResult = Assert.IsType<OkObjectResult>(returnedBook.Result);
            Assert.Equal(book, okResult.Value);
        }

        [Fact]
        public void GetBookById_RetursNotFoundResult_ForIdNotInDb()
        {
            int bookId = 99;
            _bookRepository.Setup(repo => repo.GetBookById(bookId)).Returns((Book)null);

            var result = _booksController.GetBookById(bookId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetBookById_RetursBadRequest_ForInvalidId()
        {
            int bookId = -99;

            var result = _booksController.GetBookById(bookId);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }



        [Fact]
        public void AddBook_ReturnsCreatedAtActionResult_WithNewBook()
        {

            var newBook = new Book { Title = "New Book", Author = "New Author" };
            _bookRepository.Setup(repo => repo.AddBook(newBook)).Returns(new Book { Id = 3, Title = "New Book", Author = "New Author" });

            var result = _booksController.AddBook(newBook);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var book = Assert.IsType<Book>(createdAtActionResult.Value);
            Assert.Equal(3, book.Id);
        }

        [Fact]
        public void AddBook_ReturnsBadRequest_ForInvalidBook()
        {
            var invalidBook = new Book { Title = "Book Title", Author = "" }; //Author is required
            _booksController.ModelState.AddModelError("Author", "Author is required");

            var result = _booksController.AddBook(invalidBook);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddBook_ReturnsBadRequest_ForNullBook()
        {
            Book invalidBook = null;

            var result = _booksController.AddBook(invalidBook);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateBook_ReturnsUpdatedBook_WithOkResult()
        {

            var id = 1;
            var updatedBook = new Book { Title = "UpdatedBook", Author = "UpdatedAuthor" };
            _bookRepository.Setup(repo => repo.UpdateBook(id, updatedBook)).Returns(updatedBook);

            var result = _booksController.UpdateBook(id, updatedBook);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var savedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal("UpdatedBook", savedBook.Title);

        }

        [Fact]
        public void UpdateBook_ReturnsBadRequest_ForNullBook()
        {

            var id = 1;
            Book nullBook = null;

            var result = _booksController.UpdateBook(id, nullBook);

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Fact]
        public void UpdateBook_ReturnsNotFound_ForNonExistingBook()
        {

            var id = 99;
            Book updatedBook = new Book { Title = "UpdatedBook", Author = "UpdatedAuthor" };

            var result = _booksController.UpdateBook(id, updatedBook);

            Assert.IsType<NotFoundObjectResult>(result.Result);

        }

        [Fact]
        public void DeleteBook_ReturnsNoContent_ForValidId()
        {
            var id = 1;
            _bookRepository.Setup(repo => repo.DeleteBook(id)).Returns(true);

            var result = _booksController.DeleteBook(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteBook_ReturnsNotFound_ForIdNotFound()
        {
            var id = 99;
            _bookRepository.Setup(repo => repo.DeleteBook(id)).Returns(false);

            var result = _booksController.DeleteBook(id);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void DeleteBook_ReturnsBadRequest_ForInvalidId()
        {
            var id = -99;
            _bookRepository.Setup(repo => repo.DeleteBook(id)).Returns(false);

            var result = _booksController.DeleteBook(id);

            Assert.IsType<BadRequestObjectResult>(result);
        }



    }
}