// Define Implementor

public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetVolume(int volume);
}


//Concrete Implementors
public class TV: IDevice
{
    public void TurnOn()
    {
        Console.WriteLine("TV is now ON.");
    }
    public void TurnOff()
    {
        Console.WriteLine("TV is now OFF.");
    }
    public void SetVolume(int volume)
    {
        Console.WriteLine($"TV Volume is set to {volume}");
    }
}

public class Radio : IDevice
{
    public void TurnOn()
    {
        Console.WriteLine("Radio is now ON.");
    }
    public void TurnOff()
    {
        Console.WriteLine("Radio is now OFF.");
    }
    public void SetVolume(int volume)
    {
        Console.WriteLine($"Radio Volume is set to {volume}");
    }
}


// Define Abstraction

public class RemoteControl
{
    protected IDevice device;

    public RemoteControl( IDevice device)
    {
        this.device = device;
    }

    public virtual void TurnOn()
    {
        device.TurnOn();
    }
    public virtual void TurnOff()
    {
        device.TurnOff();
    }
}


// Create a refined abstaction 

public class AdvancedRemoteControl : RemoteControl
{
    public AdvancedRemoteControl(IDevice device) : base(device)
    {

    }

    public void SetVolume(int volume)
    {
        device.SetVolume(volume);
    }
}


class Program
{
    static void Main()
    {
        // Use a TV with a basic remote
        IDevice tv = new TV();
        RemoteControl basicRemote = new RemoteControl(tv);
        basicRemote.TurnOn();
        basicRemote.TurnOff();

        Console.WriteLine();

        // Use a Radio with an advanced remote
        IDevice radio = new Radio();
        AdvancedRemoteControl advancedRemote = new AdvancedRemoteControl(radio);
        advancedRemote.TurnOn();
        advancedRemote.SetVolume(10);
        advancedRemote.TurnOff();
    }
}
