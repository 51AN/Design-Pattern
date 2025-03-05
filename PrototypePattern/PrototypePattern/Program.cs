
public abstract class Prototype
{
    public string ID { get; set; }

    public Prototype (string id)
    {
        ID = id;
    }

    // Abstract Clone method
    public abstract Prototype Clone ();
}

public class ConcretePrototype : Prototype
{
    public string Data { get; set; }

    public ConcretePrototype (string id, string data) : base(id)
    {
        Data = data;
    }

    public override Prototype Clone()
    {
        return new ConcretePrototype(this.ID, this.Data);
    }
}

// Scenario of Implementation
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public abstract class Shape
{
    public string ID { get; set; }
    public string color { get; set; }

    public Shape(string iD, string color)
    {
        ID = iD;
        this.color = color;
    }

    public abstract Shape Clone ();
}


public class Circle : Shape
{
    public int radius { get; set; }

    public Circle (string  iD, string color, int _radius) : base (iD, color)
    {
         radius = _radius;
    }

    public override Shape Clone()
    {
        return new Circle (this.ID, this.color, this.radius);
    }
}



class Program
{
    static void Main()
    {
        // Create original object
        ConcretePrototype prototype1 = new ConcretePrototype("101", "Prototype Data");

        // Clone object
        ConcretePrototype clonedObject = (ConcretePrototype)prototype1.Clone(); //Clone() method in the base class (Prototype) returns a reference of type Prototype, but we need it as ConcretePrototype.


        Console.WriteLine($"Original ID: {prototype1.ID}, Data: {prototype1.Data}");
        Console.WriteLine($"Cloned ID: {clonedObject.ID}, Data: {clonedObject.Data}");

        Console.WriteLine(ReferenceEquals(prototype1, clonedObject)); // Output: False (Different objects)

        Circle circle = new Circle("102", "Red", 4);
        Circle circleClone = (Circle)circle.Clone();

        Console.WriteLine($"Original ID: {circle.ID}, Color: {circle.color} and Radius: {circle.radius}");
        Console.WriteLine($"Cloned ID: {circleClone.ID}, Color: {circleClone.color} and Radius: {circleClone.radius}");
        Console.WriteLine(ReferenceEquals(circle, circleClone)); // Output: False (Different objects)
    }
}