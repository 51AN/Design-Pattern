using System;

// Mediator Interface
public interface IMediator
{
    void Notify(Component component, string evenType);
}


//Create Base Component
public class Component
{
    protected IMediator _mediator;

    public Component(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }
}

//Concrete Components

//Button Component
public class Button : Component
{
    public Button(IMediator mediator) : base(mediator) { }

    public void Click()
    {
        Console.WriteLine("Button Has Been Clicked.");
        _mediator.Notify(this, "click");
    }
}
// Textbox Component
public class Textbox : Component
{
    public string Text { get; set; }

    public Textbox(IMediator mediator) : base(mediator) { }

    public void InputText(string text)
    {
        Text = text;
        Console.WriteLine($"Textbox input: {Text}");
        _mediator.Notify(this, "input");
    }
}

// Checkbox Component
public class Checkbox : Component
{
    public bool Checked { get; private set; }

    public Checkbox(IMediator mediator) : base(mediator) { }

    public void Check()
    {
        Checked = !Checked;
        Console.WriteLine($"Checkbox state: {Checked}");
        _mediator.Notify(this, "check");
    }
}


// Concrete Mediator

public class AuthenticationDialog : IMediator
{
    public string Title { get; set; }
    private bool loginOrRegister;

    private Textbox loginUsername;
    private Textbox loginPassword;
    private Textbox regUsername;
    private Textbox regPassword;
    private Textbox regEmail;
    private Button ok;
    private Button cancel;
    private Checkbox rememberMe;

    public AuthenticationDialog()
    {
        loginUsername = new Textbox(this);
        loginPassword = new Textbox(this);
        regUsername = new Textbox(this);
        regPassword = new Textbox(this);
        regEmail = new Textbox(this);
        ok = new Button(this);
        cancel = new Button(this);
        rememberMe = new Checkbox(this);
    }

    // Public Methods to Simulate User Actions
    public void ClickOk() => Notify(ok, "click");
    public void ClickCancel() => Notify(cancel, "click");
    public void ToggleRememberMe() => Notify(rememberMe, "check");

    public void EnterLoginUsername(string text)
    {
        loginUsername.InputText(text);
        Notify(loginUsername, "input");
    }

    public void EnterLoginPassword(string text)
    {
        loginPassword.InputText(text);
        Notify(loginPassword, "input");
    }

    public void EnterRegUsername(string text)
    {
        regUsername.InputText(text);
        Notify(regUsername, "input");
    }

    public void EnterRegPassword(string text)
    {
        regPassword.InputText(text);
        Notify(regPassword, "input");
    }

    public void EnterRegEmail(string text)
    {
        regEmail.InputText(text);
        Notify(regEmail, "input");
    }

    // Notify logic 
    public void Notify(Component sender, string eventType)
    {
        if (sender == ok && eventType == "click")
        {
            Console.WriteLine("OK Button clicked. Processing authentication...");
            if (loginOrRegister) // Login mode
            {
                if (string.IsNullOrEmpty(loginUsername.Text) || string.IsNullOrEmpty(loginPassword.Text))
                {
                    Console.WriteLine("Login failed: Username or password cannot be empty.");
                }
                else
                {
                    Console.WriteLine($"Logging in user: {loginUsername.Text}");
                }
            }
            else // Register mode
            {
                if (string.IsNullOrEmpty(regUsername.Text) || string.IsNullOrEmpty(regPassword.Text) || string.IsNullOrEmpty(regEmail.Text))
                {
                    Console.WriteLine("Registration failed: Fields cannot be empty.");
                }
                else
                {
                    Console.WriteLine($"Registering user: {regUsername.Text} with email {regEmail.Text}");
                }
            }
        }
        else if (sender == cancel && eventType == "click")
        {
            Console.WriteLine("Cancel Button clicked. Closing dialog...");
        }
        else if (sender == rememberMe && eventType == "check")
        {
            Console.WriteLine($"Remember Me checkbox state: {rememberMe.Checked}");
        }
        else if (sender == loginUsername && eventType == "input")
        {
            Console.WriteLine($"Login Username entered: {loginUsername.Text}");
        }
        else if (sender == loginPassword && eventType == "input")
        {
            Console.WriteLine("Login Password entered (hidden for security).");
        }
        else if (sender == regUsername && eventType == "input")
        {
            Console.WriteLine($"Registration Username entered: {regUsername.Text}");
        }
        else if (sender == regPassword && eventType == "input")
        {
            Console.WriteLine("Registration Password entered (hidden for security).");
        }
        else if (sender == regEmail && eventType == "input")
        {
            Console.WriteLine($"Registration Email entered: {regEmail.Text}");
        }
    }
}



class Program
{
    static void Main()
    {
        AuthenticationDialog dialog = new AuthenticationDialog();

        // Simulate user actions
        dialog.EnterLoginUsername("user123");
        dialog.EnterLoginPassword("password");
        dialog.ClickOk(); // Clicking OK in login mode

        dialog.EnterRegUsername("newUser");
        dialog.EnterRegPassword("newPass");
        dialog.EnterRegEmail("user@example.com");
        dialog.ClickOk(); // Clicking OK in registration mode

    }
}

