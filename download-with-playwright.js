const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

async function downloadDotNetSDK() {
    console.log('Starting Playwright download process...');

    const browser = await chromium.launch({
        headless: false,  // Show browser so you can see what's happening
        downloadsPath: __dirname
    });

    try {
        const context = await browser.newContext({
            acceptDownloads: true
        });

        const page = await context.newPage();

        console.log('Navigating to .NET SDK download page...');

        // Try the direct download link first
        const downloadUrl = 'https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.415-windows-x64-installer';

        await page.goto(downloadUrl, { waitUntil: 'networkidle', timeout: 60000 });

        console.log('Page loaded. Waiting for automatic download or clicking download link...');

        // Wait a bit for auto-download to trigger
        await page.waitForTimeout(3000);

        // Try to find and click download link if auto-download didn't work
        try {
            const downloadLink = await page.locator('a:has-text("click here")').first();
            if (await downloadLink.isVisible()) {
                console.log('Found manual download link, clicking...');

                const downloadPromise = page.waitForEvent('download', { timeout: 120000 });
                await downloadLink.click();
                const download = await downloadPromise;

                console.log('Download started!');
                const downloadPath = path.join(__dirname, 'dotnet-sdk-8.0.415-win-x64.exe');
                await download.saveAs(downloadPath);

                console.log(`SUCCESS! File saved to: ${downloadPath}`);
                const stats = fs.statSync(downloadPath);
                console.log(`File size: ${(stats.size / 1024 / 1024).toFixed(2)} MB`);

                return downloadPath;
            }
        } catch (e) {
            console.log('No manual link found, checking for auto-download...');
        }

        // Check if download started automatically
        try {
            const download = await page.waitForEvent('download', { timeout: 10000 });
            console.log('Auto-download detected!');

            const downloadPath = path.join(__dirname, 'dotnet-sdk-8.0.415-win-x64.exe');
            await download.saveAs(downloadPath);

            console.log(`SUCCESS! File saved to: ${downloadPath}`);
            const stats = fs.statSync(downloadPath);
            console.log(`File size: ${(stats.size / 1024 / 1024).toFixed(2)} MB`);

            return downloadPath;
        } catch (e) {
            console.log('Auto-download did not trigger within timeout.');
        }

        // Alternative: Try direct link navigation
        console.log('Trying alternative download method...');
        const directUrl = 'https://download.visualstudio.microsoft.com/download/pr/b8cf3a22-c62d-4ac2-a835-c03b60cf65db/f14c00b4ec7f5c1d7085abf0e62bb9ee/dotnet-sdk-8.0.415-win-x64.exe';

        const downloadPromise = page.waitForEvent('download', { timeout: 120000 });
        await page.goto(directUrl);
        const download = await downloadPromise;

        console.log('Download started via direct URL!');
        const downloadPath = path.join(__dirname, 'dotnet-sdk-8.0.415-win-x64.exe');
        await download.saveAs(downloadPath);

        console.log(`SUCCESS! File saved to: ${downloadPath}`);
        const stats = fs.statSync(downloadPath);
        console.log(`File size: ${(stats.size / 1024 / 1024).toFixed(2)} MB`);

        return downloadPath;

    } catch (error) {
        console.error('Error during download:', error.message);
        throw error;
    } finally {
        await browser.close();
    }
}

// Run the download
downloadDotNetSDK()
    .then((filePath) => {
        console.log('\n=====================================');
        console.log('DOWNLOAD COMPLETED SUCCESSFULLY!');
        console.log('=====================================');
        console.log(`File location: ${filePath}`);
        console.log('\nNext steps:');
        console.log('1. Run the installer: dotnet-sdk-8.0.415-win-x64.exe');
        console.log('2. After installation, run: build.bat');
        process.exit(0);
    })
    .catch((error) => {
        console.error('\n=====================================');
        console.error('DOWNLOAD FAILED');
        console.error('=====================================');
        console.error('Error:', error.message);
        console.error('\nPlease try downloading manually from:');
        console.error('https://dotnet.microsoft.com/download/dotnet/8.0');
        process.exit(1);
    });
