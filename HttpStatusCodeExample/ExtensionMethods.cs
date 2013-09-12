using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using OpenQA.Selenium;

namespace HttpStatusCodeExample
{
    /// <summary>
    /// A class of extension methods for a WebDriver instance.
    /// </summary>
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
            return NavigateTo(driver, targetUrl, timeout, false);
        }

        /// <summary>
        /// Navigates to a specified URL, returning the HTTP status code of the navigation.
        /// </summary>
        /// <param name="driver">The driver used to navigate to the URL.</param>
        /// <param name="targetUrl">The URL to navigate to.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> structure for the time out of the navigation.</param>
        /// <param name="printDebugInfo"><see langword="true"/> to print debugging information to the console;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int NavigateTo(this IWebDriver driver, string targetUrl, TimeSpan timeout, bool printDebugInfo)
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

            // Attach the event handler, perform the navigation, and wait for
            // the status code to be non-zero, or to timeout. Then detach the
            // event handler and return the response code.
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

        /// <summary>
        /// Clicks on a link that is expected to navigate to a new URL, returning
        /// the HTTP status code of the navigation.
        /// </summary>
        /// <param name="element">The element clicked on to perform the navigation.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> structure for the time out of the navigation.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int ClickNavigate(this IWebElement element, TimeSpan timeout)
        {
            return ClickNavigate(element, timeout, false);
        }

        /// <summary>
        /// Clicks on a link that is expected to navigate to a new URL, returning
        /// the HTTP status code of the navigation.
        /// </summary>
        /// <param name="element">The element clicked on to perform the navigation.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> structure for the time out of the navigation.</param>
        /// <param name="printDebugInfo"><see langword="true"/> to print debugging information to the console;
        /// otherwise, <see langword="false"/>.</param>
        /// <returns>The HTTP status code of the navigation.</returns>
        public static int ClickNavigate(this IWebElement element, TimeSpan timeout, bool printDebugInfo)
        {
            int responseCode = 0;
            string targetUrl = string.Empty;
            SessionStateHandler responseHandler = delegate(Session targetSession)
            {
                // For the first session of the click, the URL should be the URL 
                // requested by the the element click.
                if (string.IsNullOrEmpty(targetUrl))
                {
                    targetUrl = targetSession.fullUrl;
                    if (printDebugInfo)
                    {
                        Console.WriteLine("DEBUG: Element click navigating to {0}", targetUrl);
                    }
                }

                // This algorithm could be much more sophisticated based on your needs.
                // In our case, we'll only look for responses where the content type is
                // HTML, and that the URL of the session matches our current target URL
                // Note that we also only set the response code if it's not already been
                // set.
                if (targetSession.oResponse["Content-Type"].Contains("text/html") && 
                    targetSession.fullUrl == targetUrl &&
                    responseCode == 0)
                {
                    // If the response code is a redirect, get the URL of the redirect,
                    // so that we can look for the next response from the session for that
                    // URL.
                    if (targetSession.responseCode >= 300 && targetSession.responseCode < 400)
                    {
                        targetUrl = targetSession.GetRedirectTargetURL();
                        if (printDebugInfo)
                        {
                            Console.WriteLine("DEBUG: Navigation redirected with code of {0} from {1} to {2}", targetSession.responseCode, targetSession.fullUrl, targetUrl);
                        }
                    }
                    else
                    {
                        responseCode = targetSession.responseCode;
                        if (printDebugInfo)
                        {
                            Console.WriteLine("DEBUG: Got final status code of {0} for URL {1}", targetSession.responseCode, targetUrl);
                        }
                    }
                }
            };

            // Attach the event handler, click the element, and wait for the
            // status code to be non-zero, or to timeout. Then detach the
            // event handler and return the response code.
            // Note that we're using the ResponseHeadersAvailable event so
            // as to avoid a race condition with the browser (per Eric
            // Lawrence).
            FiddlerApplication.ResponseHeadersAvailable += responseHandler;
            DateTime endTime = DateTime.Now.Add(timeout);
            element.Click();
            while (responseCode == 0 && DateTime.Now < endTime)
            {
                System.Threading.Thread.Sleep(100);
            }

            FiddlerApplication.ResponseHeadersAvailable -= responseHandler;
            return responseCode;
        }
    }
}
