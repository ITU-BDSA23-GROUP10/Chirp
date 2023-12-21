using Microsoft.Playwright;

namespace PlaywrightTests;
// Playwright handles UI testing once the program is up and running on localhost
// it open a browser and starts to tests the UI functionality to see if everything is running as it should
// to create new tests: pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5273 --ignore-https-errors


[TestFixture]
class UITesting 
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    
    [OneTimeSetUp]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByLabel("Username or email address").ClickAsync(new LocatorClickOptions
        {
            Modifiers = new[] { KeyboardModifier.Control },
        });

        await page.GetByLabel("Username or email address").FillAsync("spammer@jonaskjodt.com");

        await page.GetByLabel("Password").ClickAsync();

        await page.GetByLabel("Password").FillAsync("og=)¤GHKhrg5");

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        // save the state
        var state = await _context.StorageStateAsync(new()
        {
            Path = "../../../state.json"
        });
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
    }

    [Test]
    public async Task UserCreatesCheep()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.Locator("#Message").ClickAsync();

        await page.Locator("#Message").FillAsync("this is a user test from the UI test github user");

        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await page.GetByText("this is a user test from the UI test github user").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.CloseAsync();
    }
    // Add an email to the users profile
    [Test]
    public async Task EmailAddTest()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.CloseAsync();
    }
    // test the front end error when the user has a duplicate email
    [Test]
    public async Task EmailUpdateDuplicateError()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync(); 

        await page.GetByText("Duplicate email, that email already exists").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.CloseAsync();
    }
    // Tests the front end alert for when the user made a formatting error
    [Test]
    public async Task EmailUpdateFormattingError()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();
        
        await page.Locator("#NewEmail_Email").FillAsync("ExampleExample.com");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email formatting is incorrect").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.CloseAsync();
    }
    
    // tests the alert for when the user updates their email on their profile
    [Test]
    public async Task EmailUpdateChangeSuccessful()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();

        await page.Locator("#NewEmail_Email").FillAsync("Example@itu.dk");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.CloseAsync();
    }
    // logs a user in and deletes the user
    [Test]
    public async Task LoginAndDeleteUser()
    {
        if (_context == null)
        {
            throw new InvalidOperationException("The test context is not initialized.");
        }

        var page = await _context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.GotoAsync("https://localhost:5273/UI-tester-bdsa");

        await page.GetByText("User UI-tester-bdsa does not exist").ClickAsync();

        await page.GetByText("Go back to the home page").ClickAsync();

        await page.CloseAsync();
    }
}