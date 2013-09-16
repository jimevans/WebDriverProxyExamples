// <copyright file="WebDriverFactory.cs" company="Jim Evans">
// Copyright © 2013 Jim Evans
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// </copyright>

namespace WebDriverProxyUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.PhantomJS;

    /// <summary>
    /// A static factory class for creating WebDriver instances with proxies.
    /// </summary>
    public static class WebDriverFactory
    {
        /// <summary>
        /// Creates a WebDriver instance for the desired browser using the specified proxy settings.
        /// </summary>
        /// <param name="kind">The browser to launch.</param>
        /// <param name="proxy">The WebDriver Proxy object containing the proxy settings.</param>
        /// <returns>A WebDriver instance using the specified proxy settings.</returns>
        public static IWebDriver CreateWebDriverWithProxy(BrowserKind kind, Proxy proxy)
        {
            IWebDriver driver = null;
            switch (kind)
            {
                case BrowserKind.InternetExplorer:
                    driver = CreateInternetExplorerDriverWithProxy(proxy);
                    break;

                case BrowserKind.Firefox:
                    driver = CreateFirefoxDriverWithProxy(proxy);
                    break;

                case BrowserKind.Chrome:
                    driver = CreateChromeDriverWithProxy(proxy);
                    break;

                default:
                    driver = CreatePhantomJSDriverWithProxy(proxy);
                    break;
            }

            return driver;
        }

        /// <summary>
        /// Creates an InternetExplorerDriver instance using the specified proxy settings.
        /// </summary>
        /// <param name="proxy">The WebDriver Proxy object containing the proxy settings.</param>
        /// <returns>An InternetExplorerDriver instance using the specified proxy settings</returns>
        private static IWebDriver CreateInternetExplorerDriverWithProxy(Proxy proxy)
        {
            InternetExplorerOptions options = new InternetExplorerOptions();
            options.Proxy = proxy;

            // Make IE not use the system proxy, and clear its cache before
            // launch. This makes the behavior of IE consistent with other
            // browsers' behavior.
            options.UsePerProcessProxy = true;
            options.EnsureCleanSession = true;

            IWebDriver driver = new InternetExplorerDriver(options);
            return driver;
        }

        /// <summary>
        /// Creates an FirefoxDriver instance using the specified proxy settings.
        /// </summary>
        /// <param name="proxy">The WebDriver Proxy object containing the proxy settings.</param>
        /// <returns>An FirefoxDriver instance using the specified proxy settings</returns>
        private static IWebDriver CreateFirefoxDriverWithProxy(Proxy proxy)
        {
            // A future version of the .NET Firefox driver will likely move
            // to an "Options" model to be more consistent with other browsers'
            // API.
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetProxyPreferences(proxy);

            IWebDriver driver = new FirefoxDriver(profile);
            return driver;
        }

        /// <summary>
        /// Creates an ChromeDriver instance using the specified proxy settings.
        /// </summary>
        /// <param name="proxy">The WebDriver Proxy object containing the proxy settings.</param>
        /// <returns>An ChromeDriver instance using the specified proxy settings</returns>
        private static IWebDriver CreateChromeDriverWithProxy(Proxy proxy)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.Proxy = proxy;

            IWebDriver driver = new ChromeDriver(chromeOptions);
            return driver;
        }

        /// <summary>
        /// Creates a PhantomJSDriver instance using the specified proxy settings.
        /// </summary>
        /// <param name="proxy">The WebDriver Proxy object containing the proxy settings.</param>
        /// <returns>An InternetExplorerDriver instance using the specified proxy settings</returns>
        private static IWebDriver CreatePhantomJSDriverWithProxy(Proxy proxy)
        {
            // This is an egregiously inconsistent API. Expect this to change
            // so that an actual Proxy object can be passed in.
            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            service.ProxyType = "http";
            service.Proxy = proxy.HttpProxy;

            IWebDriver driver = new PhantomJSDriver(service);
            return driver;
        }
    }
}
