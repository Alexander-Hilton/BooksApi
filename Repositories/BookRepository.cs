
using BooksApi.Models;

namespace BooksApi.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly AppDbContext _context;

        // private readonly List<Book> _books;

        public BookRepository(AppDbContext context)
        {
            _context = context;
            // _books = new List<Book>{
            // new Book {Id=1, Title="Title1", Author="Author1"},
            // new Book {Id=2, Title="Title2", Author="Author2"}
            // };
        }

        public IEnumerable<Book> GetAllBooks()
        {

            return _context.Books.ToList();
            // return _books;
        }

        public Book GetBookById(int id)
        {

            return _context.Books.Find(id);
            // return _books.Find(b => b.Id == id);
        }

        public Book AddBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return book;
            // book.Id = _books.Max(b => b.Id) + 1;
            // _books.Add(book);
            // return book;
        }

        public Book UpdateBook(int id, Book book)
        {

            var bookToUpdate = _context.Books.Find(id);

            if (bookToUpdate == null) return null;

            bookToUpdate.Title = book.Title;
            bookToUpdate.Author = book.Author;
            bookToUpdate.ISBN = book.ISBN;
            bookToUpdate.PublishedDate = book.PublishedDate;

            _context.SaveChanges();
            return book;

            // Book existingBook = _books.First(b => b.Id == id);
            // if (existingBook == null) return null;

            // existingBook.Title = book.Title;
            // existingBook.Author = book.Author;

            // return existingBook;
        }

        public bool DeleteBook(int id)
        {
            var bookToDelete = _context.Books.Find(id);
            if (bookToDelete == null) return false;

            _context.Books.Remove(bookToDelete);
            _context.SaveChanges();
            return true;
            // Book bookToDelete = _books.First(b => b.Id == id);
            // if (bookToDelete == null) return false;

            // _books.Remove(bookToDelete);
            // return true;
        }

    }
}