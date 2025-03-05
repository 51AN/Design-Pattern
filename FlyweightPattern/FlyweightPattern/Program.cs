
public interface Ishape
{
    void Draw(int x, int y);
}

public class Circle : Ishape
{
    public string color;

    public Circle(string _color)
    {
        color = _color;
    }

    public void Draw(int x, int y)
    {
        Console.WriteLine($"Drawing {color} circle at ({x}, {y}).");
    }
}

// Flyweight factory which will reuse previousy same colored circles

public class ShapeFactory
{
    private static readonly Dictionary<string, Ishape> _circleCache = new();

    public static Ishape GetCircle(string _color)
    {
        if(!_circleCache.ContainsKey(_color))
        {
            Console.WriteLine("Creating new " + _color + " circle"); 
            _circleCache[_color] = new Circle(_color); //Put new color circle in dictionary
        }

        return _circleCache[_color]; //Reuse circle if available
    }
}


class Program
{
    static void Main()
    {
        Ishape circle1 = ShapeFactory.GetCircle("Red");
        circle1.Draw(10, 20);

        Ishape circle2 = ShapeFactory.GetCircle("Blue");
        circle2.Draw(30, 40);

        Ishape circle3 = ShapeFactory.GetCircle("Red"); // Reuses existing Red circle
        circle3.Draw(50, 60);

        Ishape circle4 = ShapeFactory.GetCircle("Blue"); // Reuses existing Blue circle
        circle4.Draw(70, 80);
    }
}
