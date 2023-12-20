//using PlaywrightSharp;
using Microsoft.Playwright;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    public class BrowserConfig
    {
       /* public static async Task<IPage> SetupChromium(StorageStatePath state)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });
            
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(directoryPath, "PlaywrightTests/user.json");

            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true, StorageStatePath = state });

            var page = await context.NewPageAsync();

            return page;

            // The rest of your setup code goes here.
        }*/

        // public static async Task<IPage> SetupFirefox()
        // {
        //     using var playwright = await Playwright.CreateAsync();
        //     await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
        //     {
        //         Headless = false,
        //     });

        //     var page = await context.NewPageAsync();

        //     string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        //     string fullPath = Path.Combine(directoryPath, "PlaywrightTests/user.json");

        //     var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true, StorageStatePath = fullPath });

        //     return page;
        //     // The rest of your setup code goes here.
        // }
    }
}

/*import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  projects: [
    // Setup project
    { name: 'setup', testMatch: /.*\.setup\.ts/ },

    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        // Use prepared auth state.
        storageState: 'playwright/.auth/user.json',
      },
      dependencies: ['setup'],
    },

    {
      name: 'firefox',
      use: {
        ...devices['Desktop Firefox'],
        // Use prepared auth state.
        storageState: 'playwright/.auth/user.json',
      },
      dependencies: ['setup'],
    },
  ],
});*/