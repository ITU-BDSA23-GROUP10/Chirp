using Microsoft.Playwright;

namespace PlaywrightTests;
//run these test
//remember in order to use the pwsh command you need to update powershell (with net8.0) with: dotnet tool update --global PowerShell
//pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5273 --ignore-https-errors

// [Parallelizable(ParallelScope.Self)]
// [TestFixture]
// class Program
// {
    // TODO: Dockerise tests so they run properly.

    /*[Test]
    public static async Task LoginWithUser()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
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

    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
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

        await page.Locator("#NewCheep_Message").ClickAsync();

        await page.Locator("#NewCheep_Message").FillAsync("this is a user test from the UI test github user");

        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await page.GetByText("this is a user test from the UI test github user").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "logout [UI-tester-bdsa]" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "UI-tester-bdsa" }).ClickAsync();
        
        await page.GetByText("this is a user test from the UI test github user").ClickAsync();

    }*/
[TestFixture]
class UITesting 
{
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task LoginWithUser()
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
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task Main()
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

        await page.Locator("#NewCheep_Message").ClickAsync();

        await page.Locator("#NewCheep_Message").FillAsync("this is a user test from the UI test github user");

        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await page.GetByText("this is a user test from the UI test github user").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "logout [UI-tester-bdsa]" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "UI-tester-bdsa" }).ClickAsync();
        
        await page.GetByText("this is a user test from the UI test github user").ClickAsync();

    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailAddTest() 
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

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateDuplicateError() 
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

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync(); 

        await page.GetByText("Email already exists ").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateFormattingError() 
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

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();

        await page.Locator("#NewEmail_Email").FillAsync("Example");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email formatting is incorrect ").ClickAsync();
    }
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EmailUpdateChangeSuccessful() 
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
            Headless = true,
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

        //LoginWithUser();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await page.GotoAsync("https://localhost:5273/UI-tester-bdsa");

        await page.GetByText("User UI-tester-bdsa does not exist").ClickAsync();

        await page.GetByText("Go back to the home page").ClickAsync();
    } 
}
