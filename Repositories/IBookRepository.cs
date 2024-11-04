using BooksApi.Models;

namespace BooksApi.Repositories
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();
        Book GetBookById(int bookId);
        Book AddBook(Book bookToAdd);
        Book UpdateBook(int id, Book updatedBook);
        bool DeleteBook(int bookId);

    }
}