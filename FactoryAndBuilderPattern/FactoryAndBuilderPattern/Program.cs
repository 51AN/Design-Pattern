using System;
using System.Collections.Generic;

// Base Car class
public abstract class Car
{
    public string Model { get; set; }
    public string Engine { get; set; }
    public int Seats { get; set; }
    public bool GPS { get; set; }

    public abstract void ShowCar();
}

// Supra class with specific property
public class Supra : Car
{
    public int TopSpeed { get; set; }

    public override void ShowCar()
    {
        Console.WriteLine($"Car Model: {Model}, Engine: {Engine}, Seats: {Seats}, GPS: {GPS}, Top Speed: {TopSpeed} km/h");
    }
}

// Bus class with specific property
public class Bus : Car
{
    public int PassengerCapacity { get; set; }

    public override void ShowCar()
    {
        Console.WriteLine($"Car Model: {Model}, Engine: {Engine}, Seats: {Seats}, GPS: {GPS}, Passenger Capacity: {PassengerCapacity}");
    }
}

// Truck class with specific property
public class Truck : Car
{
    public double Mileage { get; set; }

    public override void ShowCar()
    {
        Console.WriteLine($"Car Model: {Model}, Engine: {Engine}, Seats: {Seats}, GPS: {GPS}, Mileage: {Mileage} km/l");
    }
}

// Builder Interface
public interface ICarBuilder
{
    void SetModel();
    void SetEngine();
    void SetSeats();
    void SetGPS();
    void SetCustomAttributes(); // Each car will set its own attributes
    Car Build();
}

// Concrete Builders
public class SupraBuilder : ICarBuilder
{
    private Supra _car = new Supra();

    public void SetModel() => _car.Model = "Supra";
    public void SetEngine() => _car.Engine = "3.0L Turbocharged";
    public void SetSeats() => _car.Seats = 2;
    public void SetGPS() => _car.GPS = true;
    public void SetCustomAttributes() => _car.TopSpeed = 250; // Supra-specific attribute

    public Car Build() => _car;
}

public class BusBuilder : ICarBuilder
{
    private Bus _car = new Bus();

    public void SetModel() => _car.Model = "Bus";
    public void SetEngine() => _car.Engine = "6.0L Diesel";
    public void SetSeats() => _car.Seats = 40;
    public void SetGPS() => _car.GPS = true;
    public void SetCustomAttributes() => _car.PassengerCapacity = 50; // Bus-specific attribute

    public Car Build() => _car;
}

public class TruckBuilder : ICarBuilder
{
    private Truck _car = new Truck();

    public void SetModel() => _car.Model = "Truck";
    public void SetEngine() => _car.Engine = "5.0L";
    public void SetSeats() => _car.Seats = 2;
    public void SetGPS() => _car.GPS = false;
    public void SetCustomAttributes() => _car.Mileage = 8.0; // Truck-specific attribute

    public Car Build() => _car;
}

// Factory class for product
public class CarFactory
{
    private static readonly Dictionary<string, Func<ICarBuilder>> Builders = new()
    {
        { "Supra", () => new SupraBuilder() },
        { "Bus", () => new BusBuilder() },
        { "Truck", () => new TruckBuilder() }
    };

    public static Car CreateCar(string type)
    {
        if (!Builders.TryGetValue(type, out var builderFactory))
        {
            throw new ArgumentException("Invalid car type");
        }

        ICarBuilder builder = builderFactory();
        builder.SetModel();
        builder.SetEngine();
        builder.SetSeats();
        builder.SetGPS();
        builder.SetCustomAttributes();

        return builder.Build();
    }
}


class Program
{
    static void Main()
    {
        Car supra = CarFactory.CreateCar("Supra");
        supra.ShowCar();

        Car bus = CarFactory.CreateCar("Bus");
        bus.ShowCar();

        Car truck = CarFactory.CreateCar("Truck");
        truck.ShowCar();
    }
}
