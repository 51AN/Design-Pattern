
//Subscriber Class
public interface ISubscriber
{
    void Update(string message);
}


//Event manager class
public class Newsletter
{
    private List<ISubscriber> subscribers = new List<ISubscriber>();

    public void Subscribe(ISubscriber subscriber)
    {
        subscribers.Add(subscriber);
        Console.WriteLine($"Subscriber added.");
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        subscribers.Remove(subscriber);
        Console.WriteLine("Subscriber removed.");
    }

    public void NotifySubscribers(string message)
    {
        foreach(var subscriber in subscribers)
        {
            subscriber.Update(message);
        }
    }
}

//Concrete observer class

public class EmailSubscriber : ISubscriber
{
    public string name;

    public EmailSubscriber(string name)
    {
        this.name = name;
    }

    public void Update(string message)
    {
        Console.WriteLine($"{name} received email notification: {message}");
    }
}

public class SMSsubscriber : ISubscriber
{
    public string phonenumber;

    public SMSsubscriber(string phonenumber)
    {
        this.phonenumber = phonenumber;
    }

    public void Update(string message)
    {
        Console.WriteLine($"{phonenumber} received email notification: {message}");
    }
}

class Program
{
    static void Main()
    {
        Newsletter newsletter = new Newsletter();

        ISubscriber alice = new EmailSubscriber("Alice");
        ISubscriber bob = new SMSsubscriber("+123456789");

        newsletter.Subscribe(alice);
        newsletter.Subscribe(bob);

        Console.WriteLine("\nNew article published!");
        newsletter.NotifySubscribers("New C# Observer Pattern Tutorial is out!");

        newsletter.Unsubscribe(bob);

        Console.WriteLine("\nNew article published!");
        newsletter.NotifySubscribers("Advanced C# Patterns are now available!");
    }
}
