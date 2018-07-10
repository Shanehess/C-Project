using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using PerfectoFrameworkLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PerfectoFrameworkLibrary
{
    public static class SeleniumExtensions
    {

        static RemoteWebDriverExtended driver = null;
        

        public static void setLibrary(TestBase b)
        {
            driver = b.driver;
        }

        public static void logMe(IWebElement element, Exception ex = null)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            if (ex == null)
            {
                //lib.log(sf.GetMethod().Name + "(" + element.ToString() + ")");
            }
            else
            {
                //lib.log(sf.GetMethod().Name + "Exception: " + sf.ToString());
                //lib.methodError(ex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }


        public static IWebElement clickElement(this IWebElement element)
        {
            IWebElement newElement = element;
            logMe(newElement);
            try
            {
                newElement.Click();
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }

            return element;
        }


        public static IWebElement waitForElement(this IWebElement element, int seconds)
        {
            IWebElement newElement = element;
            logMe(element);
            //            Debug.WriteLine(element.ToString());


            try
            {
                var wait = new DefaultWait<RemoteWebDriverExtended>(driver);
                wait.Timeout = TimeSpan.FromSeconds(seconds);
                wait.PollingInterval = TimeSpan.FromMilliseconds(500);
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                     wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

                return wait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable(element));
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                logMe(element, ex);
                return null;
            }
        }



        public static void clearText(this IWebElement element)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                newElement.Clear();
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
        }

        public static void sendText(this IWebElement element, string data, bool clear = false)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                if (clear)
                {
                    newElement.Clear();
                }
                newElement.SendKeys(data);
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
        }

        public static void typeText(this IWebElement element, string data, bool clear = false)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                if (clear)
                {
                    newElement.Clear();
                }

                newElement.Click();

                driver.Keyboard.SendKeys(data);
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
        }


        public static void submitElement(this IWebElement element)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                newElement.Submit();
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }

        }

        public static void setDropDownText(this IWebElement element, string text)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                new SelectElement(newElement).SelectByText(text);
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
        }

        public static void setDropDownValue(this IWebElement element, string text, int timeOut)
        {
            IWebElement newElement = element;
            logMe(newElement);

            try
            {
                new SelectElement(newElement).SelectByValue(text);
            }
            catch (Exception ex)
            {
                logMe(element, ex);
            }
        }


        public static void waitForTitle(int seconds, string title)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
                wait.Until((d) => { return d.Title.ToLower().StartsWith(title); });
            }
            catch (Exception ex)
            {

            }
        }

        public static string getText(this IWebElement element, int timeOut)
        {
            IWebElement newElement = element;

            logMe(newElement);

            string text = null;

            try
            {
                text = newElement.Text.ToString();
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
            return text;
        }

        public static string getValue(this IWebElement element, int timeOut)
        {
            IWebElement newElement = element;
            logMe(newElement);

            string value = null;

            try
            {
                value = newElement.GetAttribute("value");
            }
            catch (Exception ex)
            {
                logMe(newElement, ex);
            }
            return value;
        }

        public static void scrollToElement(this IWebElement element)
        {
            IWebElement newElement = element;
            logMe(newElement);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", newElement);
        }


        public static bool isElementPresent(this IWebElement element)
        {
            IWebElement newElement = element;
            try
            {
                if (newElement.Displayed != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool isElementPresent(this By element)
        {
            By newElement = element;
            try
            {
                if (driver.FindElements(newElement).Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static IWebElement waitForElement(this By element, int seconds)
        {
            By newElement = element;

            //try
            //{

            //    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            //    return wait.Until(drv => drv.FindElement(newElement));


            //}
            //catch (Exception ex)
            //{
            //    logMe(null, ex);
            //    return null;
            //}



            try
            {
                var wait = new DefaultWait<RemoteWebDriverExtended>(driver);
                wait.Timeout = TimeSpan.FromSeconds(seconds);
                wait.PollingInterval = TimeSpan.FromMilliseconds(500);
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

                return wait.Until<IWebElement>(ExpectedConditions.ElementExists(newElement));
            }

            catch (Exception ex)
            {
                return null;
            }
            }

    }
}

