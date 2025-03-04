
public class TemperatureCelsius
{
    public TemperatureCelsius()
    {

    }

    public double getTemp()
    {
        return 25.0;
    }
}

public interface iTemperatureService
{
    double getTemperature();
}

public class TemperatureAdapter : iTemperatureService
{
    private readonly TemperatureCelsius temperatureCelsius;

    public TemperatureAdapter (TemperatureCelsius temperatureCelsius)
    {
        this.temperatureCelsius = temperatureCelsius;
    }

    public double getTemperature()
    {
        //Implementation of adaption
        double celsius = temperatureCelsius.getTemp();
        return (celsius * 9 / 5) + 32;
    }
}


class Program
{
    static void Main()
    {
        TemperatureCelsius temperatureCelsius = new TemperatureCelsius();

        TemperatureAdapter temperatureAdapter = new TemperatureAdapter(temperatureCelsius);
        Console.WriteLine("Temperature in Celsius: " + temperatureCelsius.getTemp());
        Console.WriteLine("Temperature in Farenheit: " + temperatureAdapter.getTemperature());
    }
}