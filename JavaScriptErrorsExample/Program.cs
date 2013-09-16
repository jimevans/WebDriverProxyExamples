// <copyright file="Program.cs" company="Jim Evans">
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

namespace JavaScriptErrorsExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fiddler;
    using OpenQA.Selenium;
    using WebDriverProxyUtilities;

    /// <summary>
    /// Class containing the main application entry point and supporting methods.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments of the application.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args", Justification = "Allowing main method to have arguments.")]
        internal static void Main(string[] args)
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
            ////IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.Firefox, proxy);
            ////IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.Chrome, proxy);
            ////IWebDriver driver = WebDriverFactory.CreateWebDriverWithProxy(BrowserKind.PhantomJS, proxy);

            TestJavaScriptErrors(driver);

            driver.Quit();

            StopFiddlerProxy();
            Console.WriteLine("Complete! Press <Enter> to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Starts the Fiddler proxy using the desired port.
        /// </summary>
        /// <param name="desiredPort">The port on which the proxy is to listen.
        /// Use zero (0) to have Fiddler select a port.</param>
        /// <returns>The port on which the Fiddler proxy is listening.</returns>
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

        /// <summary>
        /// Stops the Fiddler proxy.
        /// </summary>
        private static void StopFiddlerProxy()
        {
            Console.WriteLine("Shutting down Fiddler proxy");
            FiddlerApplication.Shutdown();
        }

        /// <summary>
        /// Executes the code to test collection of JavaScript errors on the page.
        /// </summary>
        /// <param name="driver">The driver to use with the browser.</param>
        private static void TestJavaScriptErrors(IWebDriver driver)
        {
            // Using Dave Haeffner's the-internet project http://github.com/arrgyle/the-internet,
            // which provides pages that return various HTTP status codes.
            string url = "http://the-internet.herokuapp.com/javascript_error";
            Console.WriteLine("Navigating to {0}", url);
            driver.NavigateTo(url);
            IList<string> javaScriptErrors = driver.GetJavaScriptErrors();
            if (javaScriptErrors == null)
            {
                Console.WriteLine("Could not access JavaScript errors collection. This is a catastrophic failure.");
            }
            else
            {
                if (javaScriptErrors.Count > 0)
                {
                    Console.WriteLine("Found the following JavaScript errors on the page:");
                    foreach (string javaScriptError in javaScriptErrors)
                    {
                        Console.WriteLine(javaScriptError);
                    }
                }
                else
                {
                    Console.WriteLine("No JavaScript errors found on the page.");
                }
            }
        }
    }
}
