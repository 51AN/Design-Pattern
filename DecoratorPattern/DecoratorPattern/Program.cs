//First we make the base coffee interface to determine the behavior

using System.Transactions;

public interface Icoffee
{
    string getDescription();
    double getPrice();

}

//Now create the basic coffee class

public class BasicCoffee : Icoffee
{
    public string getDescription()
    {
        return "Simple Coffee";
    }

    public double getPrice()
    {
        return 20.0;
    }
}

// Now create the decorator abstract class as the decoration middlepoint

public abstract class CoffeeDecorator : Icoffee
{
    private readonly Icoffee _coffee;

    public CoffeeDecorator(Icoffee coffee)
    {
        _coffee = coffee;
    }

    public virtual string getDescription()
    {
        return _coffee.getDescription();
    }

    public virtual double getPrice()
    {
        return _coffee.getPrice();
    }

}

// Now to make specific decorators

public class MilkDecorator : CoffeeDecorator
{

    public MilkDecorator(Icoffee coffee) : base(coffee)
    {

    }

    public override string getDescription()
    {
        return base.getDescription() + " + Milk";
    }

    public override double getPrice()
    {
        return base.getPrice() + 10;
    }
}

public class SugarDecorator : CoffeeDecorator
{

    public SugarDecorator(Icoffee coffee) : base(coffee)
    {

    }

    public override string getDescription()
    {
        return base.getDescription() + " + Sugar";
    }

    public override double getPrice()
    {
        return base.getPrice() + 5;
    }
}


class Program
{
    static void Main()
    {
        Icoffee coffee = new BasicCoffee();

        Console.WriteLine("Coffee Type: " + coffee.getDescription() + " and  price: " + coffee.getPrice());

        coffee = new SugarDecorator(coffee);

        Console.WriteLine("Coffee Type: " + coffee.getDescription() + " and  price: " + coffee.getPrice());

        coffee = new MilkDecorator(coffee);

        Console.WriteLine("Coffee Type: " + coffee.getDescription() + " and  price: " + coffee.getPrice());



    }
}
