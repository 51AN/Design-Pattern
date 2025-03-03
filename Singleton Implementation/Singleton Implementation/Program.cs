// See https://aka.ms/new-console-template for more information

using System;

public class Singleton
{
    //holds the single instance of the class
    private static Singleton instance;
    //ensuring thread safety in a multithread system
    private static readonly object instanceLock = new object();
    //non-accesible constructor for the Singleton class
    private Singleton()
    { }
    public static Singleton Instance
    {
        get
        { 
            if (instance == null)
            {
                lock (instanceLock)
                {
                    //double check lock thread safety, i.e. check for null after locking
                    if (instance == null)
                    {
                        instance = new Singleton();
                    }
                }
            }
            return instance;
        }
    }

    public void ShowMessage()
    {
        Console.WriteLine("Instance Created");
    }

}
class Program
{
    static void Main()
    {
        Singleton instance1 =  Singleton.Instance;
        Singleton instance2 = Singleton.Instance;

        instance1.ShowMessage();

        //will return true if the instances are the same
        Console.WriteLine(instance1 ==  instance2);



    }
}
