WebDriverProxyExamples
======================

A series of examples showing how to use a proxy with the WebDriver .NET bindings.
The samples use Eric Lawrence's (now Telerik's) excellent [Fiddler](http://fiddler2.com/)
proxy, specifically the [FiddlerCore](http://fiddler2.com/fiddlercore) component.
Launching of browsers is handled by a factory class in a common referenced assembly.

This repository includes the following examples:

HttpStatusCodeExample
---------------------
A sample project demonstrating how to retrieve HTTP status codes from WebDriver
using a proxy. The project uses .NET extension methods to make it appear as though
the WebDriver objects have methods to include status codes natively. This is the 
project referenced in the [blog post series](http://jimevansmusic.blogspot.com/2013/08/implementing-webdriver-http-status.html)
about HTTP status codes.

JavaScriptErrorsExample
-----------------------
Building on [Alister Scott's suggestion](http://watirmelon.com/2012/12/19/using-webdriver-to-automatically-check-for-javascript-errors-on-every-page/)
on how to check for JavaScript errors, this sample project demonstrates a method to 
retrieve JavaScript errors on a page from WebDriver. Though adding error handling
code directly to the website's source is a far superior approach and should be
preferred whenever possible, the approach outlined here may be particularly useful
in cases where you do not have the ability to add that error handling code. There
have been other solutions proposed to this problem, but many of them either have the
limitation that they [don't work across all browsers](http://mguillem.wordpress.com/2011/10/11/webdriver-capture-js-errors-while-running-tests/),
or that they cannot process JavaScript errors occuring during the page's onload
event. Again, the project uses .NET extension methods to make it appear
as though the WebDriver objects have methods to retrieve these errors natively. 
