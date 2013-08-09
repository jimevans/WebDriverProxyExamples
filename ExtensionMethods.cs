using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using OpenQA.Selenium;

namespace HttpStatusCodeExample
{
    static class ExtensionMethods
    {
        private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Navigates to a specified URL, returning the HTTP status code of the navigation.
        /// </summary>
        /// <param name="driver">The driver used to navigate to the URL.</param>
        /// <param name="targetUrl">The URL to navigate to.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int NavigateTo(this IWebDriver driver, string targetUrl)
        {
            return NavigateTo(driver, targetUrl, DefaultTimeout);
        }

        /// <summary>
        /// Navigates to a specified URL, returning the HTTP status code of the navigation.
        /// </summary>
        /// <param name="driver">The driver used to navigate to the URL.</param>
        /// <param name="targetUrl">The URL to navigate to.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> structure for the time out of the navigation.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int NavigateTo(this IWebDriver driver, string targetUrl, TimeSpan timeout)
        {
            int responseCode = 0;
            DateTime endTime = DateTime.Now.Add(timeout);
            SessionStateHandler responseHandler = delegate(Session targetSession)
            {
                if (targetSession.fullUrl == targetUrl)
                {
                    responseCode = targetSession.responseCode;
                }
            };

            FiddlerApplication.AfterSessionComplete += responseHandler;
            driver.Url = targetUrl;
            while (responseCode == 0 && DateTime.Now < endTime)
            {
                System.Threading.Thread.Sleep(100);
            }

            FiddlerApplication.AfterSessionComplete -= responseHandler;
            return responseCode;
        }

        /// <summary>
        /// Clicks on a link that is expected to navigate to a new URL, returning
        /// the HTTP status code of the navigation.
        /// </summary>
        /// <param name="element">The element clicked on to perform the navigation.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int ClickNavigate(this IWebElement element)
        {
            return ClickNavigate(element, DefaultTimeout);
        }

        public static int ClickNavigate(this IWebElement element, TimeSpan timeout)
        {
            int responseCode = 0;
            string urlNavigated = string.Empty;
            SessionStateHandler responseHandler = delegate(Session targetSession)
            {
                // Get the response code of the first HTML response we get.
                // This algorithm could be much more sophisticated based on your needs.
                if (targetSession.oResponse["Content-Type"].Contains("text/html") && responseCode == 0)
                {
                    // If the response code is a redirect, ignore it so that we
                    // get the "final" status code.
                    if (targetSession.responseCode < 300 || targetSession.responseCode >= 400)
                    {
                        responseCode = targetSession.responseCode;
                    }
                }
            };

            FiddlerApplication.AfterSessionComplete += responseHandler;
            DateTime endTime = DateTime.Now.Add(timeout);
            element.Click();
            while (responseCode == 0 && DateTime.Now < endTime)
            {
                System.Threading.Thread.Sleep(100);
            }

            FiddlerApplication.AfterSessionComplete -= responseHandler;
            return responseCode;
        }
    }
}
