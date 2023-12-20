using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests;
class EndtoEnd
{
    [Parallelizable(ParallelScope.Self)]
    [Test]
    public static async Task EndtoEndTest()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        });

        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5273/");

        await page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();

        await page.GetByLabel("Username or email address").ClickAsync();

        await page.GetByLabel("Username or email address").FillAsync("spammer@jonaskjodt.com");

        await page.GetByLabel("Password").ClickAsync();

        await page.GetByLabel("Password").FillAsync("og=)¤GHKhrg5");

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in", Exact = true }).ClickAsync();

        await page.Locator("#Message").ClickAsync();

        await page.Locator("#Message").FillAsync("Hey this is the end-to-end test.");

        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await page.GetByText("Hey this is the end-to-end test.").ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Upvote" }).First.ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await page.Locator("li").Filter(new() { HasText = "Hey this is the end-to-end test."}).GetByRole(AriaRole.Button).Nth(1).ClickAsync();

        await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01/08/2023 13.17.39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();

        await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine — 01/08/2023 13.17.39 Starbuck now is what we hear the" }).GetByRole(AriaRole.Button).First.ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.Locator("li").Filter(new() { HasText = "Hey this is the end-to-end test. Delete" }).GetByRole(AriaRole.Button).ClickAsync();

        await page.Locator("#NewEmail_Email").ClickAsync();

        await page.Locator("#NewEmail_Email").FillAsync("Example@1.com");

        await page.GetByRole(AriaRole.Button, new() { Name = "Add Email" }).ClickAsync();

        await page.GetByText("Email successfully updated").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await page.GotoAsync("https://localhost:5273/?page=21");

        await page.GetByText("Hello, BDSA students!").ClickAsync();

        await page.GetByRole(AriaRole.Link, new() { Name = "[UI-tester-bdsa] profile" }).ClickAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

    }
}