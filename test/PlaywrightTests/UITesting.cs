using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests;


//run these test
//remember in order to use the pwsh command you need to update powershell (with net8.0) with: dotnet tool update --global PowerShell
//pwsh bin/Debug/net8.0/playwright.ps1 codegen https://localhost:5273 --ignore-https-errors

[Parallelizable(ParallelScope.Self)]
[TestFixture]
class Program
{
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
}
