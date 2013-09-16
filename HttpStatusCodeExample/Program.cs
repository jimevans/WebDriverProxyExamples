using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;
using OpenQA.Selenium;
using WebDriverProxyUtilities;

namespace HttpStatusCodeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Note that we're using a port of 0, which tells Fiddler to
            // select a random available port to listen on.
            int proxyPort = StartFiddlerProxy(0);

            // We are only proxying HTTP traffic, but could just as easily
            // proxy HTTPS or FTP traffic.
            OpenQA.Selenium.Proxy proxy = new OpenQA.Selenium.Proxy();
            proxy.HttpProxy = string.Format("127.0.0.1:{0}", proxyPort);

            // See the code of the individual methods for the details of how
            // to create the driver instance with the proxy settings properly set.
            IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.IE, proxy);
            //IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.Firefox, proxy);
            //IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.Chrome, proxy);
            //IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.PhantomJS, proxy);

            TestStatusCodes(driver);

            driver.Quit();

            StopFiddlerProxy();
            Console.WriteLine("Complete! Press <Enter> to exit.");
            Console.ReadLine();
        }

        private static void StopFiddlerProxy()
        {
            Console.WriteLine("Shutting down Fiddler proxy");
            FiddlerApplication.Shutdown();
        }

        private static int StartFiddlerProxy(int desiredPort)
        {
            // We explicitly do *NOT* want to register this running Fiddler
            // instance as the system proxy. This lets us keep isolation.
            Console.WriteLine("Starting Fiddler proxy");
            FiddlerCoreStartupFlags flags = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            FiddlerApplication.Startup(desiredPort, flags);
            int proxyPort = FiddlerApplication.oProxy.ListenPort;
            Console.WriteLine("Fiddler proxy listening on port {0}", proxyPort);
            return proxyPort;
        }

        private static void TestStatusCodes(IWebDriver driver)
        {
            // Using Dave Haeffner's the-internet project http://github.com/arrgyle/the-internet,
            // which provides pages that return various HTTP status codes.
            string url = "http://the-internet.herokuapp.com/redirect";
            Console.WriteLine("Navigating to {0}", url);
            int responseCode = driver.NavigateTo(url);
            Console.WriteLine("Navigation to {0} returned response code {1}", url, responseCode);

            // Demonstrates navigating to a 404 page.
            url = "http://the-internet.herokuapp.com/redirector";
            Console.WriteLine("Navigating to {0}", url);
            responseCode = driver.NavigateTo(url);
            Console.WriteLine("Navigation to {0} returned response code {1}", url, responseCode);
            string elementId = "redirect";
            Console.WriteLine("Clicking on element with ID {0}", elementId);
            IWebElement element = driver.FindElement(By.Id(elementId));
            responseCode = element.ClickNavigate();
            Console.WriteLine("Element click returned response code {0}", responseCode);

            // Demonstrates navigating to a 404 page.
            url = "http://the-internet.herokuapp.com/status_codes/404";
            Console.WriteLine("Navigating to {0}", url);
            responseCode = driver.NavigateTo(url);
            Console.WriteLine("Navigation to {0} returned response code {1}", url, responseCode);
        }
    }
}
