using Microsoft.Playwright;

namespace PlaywrightTests;
// Playwright handles UI testing once the program is up and running on localhost
// it open a browser and starts to tests the UI functionality to see if everything is running as it should
// to create new tests: pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5273 --ignore-https-errors

[TestFixture]
class UITesting 
{
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public async Task LoginWithUser()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });

        var page = await context.NewPageAsync();

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
        var state = await context.StorageStateAsync(new()
        {
            Path = "../../../state.json"
        });

        //await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = "state.json" });
    
        await browser.CloseAsync();
    }
    
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true,
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.Locator("#Message").ClickAsync();

        await page.Locator("#Message").FillAsync("this is a user test from the UI test github user");

        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        //await page.GetByText("this is a user test from the UI test github user").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "logout [UI-tester-bdsa]" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "UI-tester-bdsa" }).ClickAsync();
        
    }
    
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailAddTest() 
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();
        
        await page.GetByText("Duplicate email, that email already exists").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateDuplicateError() 
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync(); 

        await page.GetByText("Duplicate email, that email already exists").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateFormattingError() 
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");
        
        await page.GetByLabel("Username or email address").ClickAsync(new LocatorClickOptions
        {
            Modifiers = new[] { KeyboardModifier.Control },
        });

        await page.GetByLabel("Username or email address").FillAsync("spammer@jonaskjodt.com");

        await page.GetByLabel("Password").ClickAsync();

        await page.GetByLabel("Password").FillAsync("og=)¤GHKhrg5");

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();

        await page.Locator("#NewEmail_Email").FillAsync("Example");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Duplicate email, that email already exists").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateChangeSuccessful() 
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");
        
        await page.GetByLabel("Username or email address").ClickAsync(new LocatorClickOptions
        {
            Modifiers = new[] { KeyboardModifier.Control },
        });

        await page.GetByLabel("Username or email address").FillAsync("spammer@jonaskjodt.com");

        await page.GetByLabel("Password").ClickAsync();

        await page.GetByLabel("Password").FillAsync("og=)¤GHKhrg5");

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();

        await page.Locator("#NewEmail_Email").FillAsync("Example@itu.dk");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task LoginAndDeleteUser()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            //SlowMo = 10000
        });

        var context = await browser.NewContextAsync(new()
        {
            StorageStatePath = "../../../state.json",
            IgnoreHTTPSErrors = true
        });

        var page = await context.NewPageAsync();
        
        await page.GotoAsync("https://localhost:5273");

        await page.GetByLabel("Username or email address").ClickAsync(new LocatorClickOptions
        {
            Modifiers = new[] { KeyboardModifier.Control },
        });

        await page.GetByLabel("Username or email address").FillAsync("spammer@jonaskjodt.com");

        await page.GetByLabel("Password").ClickAsync();

        await page.GetByLabel("Password").FillAsync("og=)¤GHKhrg5");

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        //LoginWithUser();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.GotoAsync("https://localhost:5273/UI-tester-bdsa");

        await page.GetByText("User UI-tester-bdsa does not exist").ClickAsync();

        await page.GetByText("Go back to the home page").ClickAsync();
    }
}
