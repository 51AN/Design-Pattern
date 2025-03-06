public class Book
{
    public string Title { get; }
    public Book(string _title)
    {
        Title = _title;
    }
}

//Define iterator interface

public interface IBookIterator
{
    bool MoveNext();
    Book Next();
}

// Define Collection Interface

public interface IBookCollection
{
    IBookIterator CreateIterator();
}

// Creat Concrete Iterator

public class BookIterator : IBookIterator
{
    private readonly List<Book> books;
    private int currentIndex = 0;

    public BookIterator(List<Book> books)
    {
        //this.books = books ?? new List<Book>(); In this case, we were violating the rule of SRP. Concrete iterator should not modify books list by making it a new list if null
        this.books = books;

    }

    public bool MoveNext()
    {
        return books != null && currentIndex < books.Count;
    }

    public Book Next()
    {
        if (!MoveNext())
        {
            throw new InvalidOperationException("No more books to iterate over.");
        }
        return books[currentIndex++];
    }
}

// Create concrete book iterator class
public class BookCollection : IBookCollection
{
    private readonly List<Book> books = new List<Book>();

    public void AddBook (Book book)
    {
        books.Add(book);
    }

    public IBookIterator CreateIterator()
    {
        return new BookIterator(books);
    }
}

class Program
{
    static void Main()
    {
        BookCollection library = new BookCollection();
        library.AddBook(new Book("Design Patterns in C#"));
        library.AddBook(new Book("Clean Code"));
        library.AddBook(new Book("The Pragmatic Programmer"));
        library.AddBook(new Book("Twilight Saga"));

        IBookIterator bookIterator = library.CreateIterator();

        while (bookIterator.MoveNext())
        {
            Book book = bookIterator.Next();
            Console.WriteLine($"Book: {book.Title}");
        }

    }
}