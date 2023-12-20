using Microsoft.Playwright;
using Microsoft.Playwright.Core;

namespace PlaywrightTests
{
    public class GlobalSetup
    {
        /*public async Task<StorageState> SetupUserLogin()
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
                Path = "state.json"
            });
            
            // await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = "state.json" });
        
            await browser.CloseAsync();

            return state;
        }*/
    }
    

}