//Create the service interface
public interface IServiceInterface
{
    void ReadFile();
}

// The Actual Secure Service class (Here as the Database)
public class Database: IServiceInterface
{
    private readonly string _fileName;
    public Database(string fileName)
    {
        _fileName = fileName;
    }

    public void ReadFile()
    {
        Console.WriteLine($"Reading File: {_fileName}");
    }
}

// Create The Proxy Class

public class ProxyService : IServiceInterface
{
    private Database? _database = null!; // This tells the compiler: “I know _secureFile will be initialized later, so don’t warn me.”
    private readonly string _fileName;
    private readonly string _userRole;

    public ProxyService(string fileName, string userRole)
    {
        _fileName = fileName;
        _userRole = userRole;
    }

    public void ReadFile()
    {
        if (_userRole.ToLower() != "admin")
        {
            Console.WriteLine("Access Denied: You must be an admin to read this file.");
            return;
        }

        // Lazy Initialization: Only create Securefile when needed

        if(_database == null)
        {
            _database = new Database(_fileName);
        }

        _database.ReadFile();
    }
}


class Program
{
    static void Main()
    {
        IServiceInterface file1 = new ProxyService("secret.txt", "admin");
        file1.ReadFile(); //Will be allowed to read

        IServiceInterface file2 = new ProxyService("secret.txt", "guest");
        file2.ReadFile(); //Will not be allowed to read


    }
}