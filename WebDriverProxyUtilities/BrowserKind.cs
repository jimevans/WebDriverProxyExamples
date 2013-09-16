// <copyright file="BrowserKind.cs" company="Jim Evans">
// Copyright © 2013 Jim Evans
// Licensed under the MIT license, as found in the LICENSE file accompanying this source code.
// </copyright>

namespace WebDriverProxyUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Types of browser available for proxy examples.
    /// </summary>
    public enum BrowserKind
    {
        /// <summary>
        /// Internet Explorer
        /// </summary>
        InternetExplorer,

        /// <summary>
        /// Internet Explorer
        /// </summary>
        IE = InternetExplorer,

        /// <summary>
        /// Mozilla Firefox
        /// </summary>
        Firefox,

        /// <summary>
        /// Google Chrome
        /// </summary>
        Chrome,

        /// <summary>
        /// PhantomJS headless browser
        /// </summary>
        PhantomJS
    }
}
