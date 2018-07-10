using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using PerfectoFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PerfectoFramework
{
    public static class SeleniumExtensions
    {

        static RemoteWebDriverExtended driver = null;
        static TestBase tb;

        public static void SetLibrary(TestBase b)
        {
            driver = b.driver;
            tb = b;
        }


        public static IWebElement Click(this IWebElement element)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);
            try
            {
                newElement.Click();
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }

            return element;
        }


        public static IWebElement WaitForElement(this IWebElement element, int seconds)
        {
            IWebElement newElement = element;
            tb.LogMe(element);



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
                tb.LogMe(element, ex);
                return newElement;
            }
        }



        public static void ClearText(this IWebElement element)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

            try
            {
                newElement.Clear();
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }
        }

        public static void SendText(this IWebElement element, string data, bool clear = false)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

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
                tb.LogMe(newElement, ex);
            }
        }

        public static void TypeText(this IWebElement element, string data, bool clear = false)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

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
                tb.LogMe(newElement, ex);
            }
        }


        public static void SubmitElement(this IWebElement element)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

            try
            {
                newElement.Submit();
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }

        }

        public static void SetDropDownText(this IWebElement element, string text)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

            try
            {
                new SelectElement(newElement).SelectByText(text);
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }
        }

        public static void SetDropDownValue(this IWebElement element, string text, int timeOut)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

            try
            {
                new SelectElement(newElement).SelectByValue(text);
            }
            catch (Exception ex)
            {
                tb.LogMe(element, ex);
            }
        }


        public static void WaitForTitle(int seconds, string title)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
                wait.Until((d) => { return d.Title.ToLower().StartsWith(title); });
            }
            catch (Exception ex)
            {
                tb.LogMe("WaitForTitle",ex);
            }
        }

        public static string GetText(this IWebElement element, int timeOut)
        {
            IWebElement newElement = element;

            tb.LogMe(newElement);

            string text = null;

            try
            {
                text = newElement.Text.ToString();
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }
            return text;
        }

        public static string GetValue(this IWebElement element, int timeOut)
        {
            IWebElement newElement = element;
            tb.LogMe(newElement);

            string value = null;

            try
            {
                value = newElement.GetAttribute("value");
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }
            return value;
        }

        public static void ScrollToElement(this IWebElement element)
        {
            IWebElement newElement = element;

            tb.LogMe(newElement);
            try
            {
               
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", newElement);
            }
            catch (Exception ex)
            {
                tb.LogMe(newElement, ex);
            }
        }


        public static bool IsElementPresent(this IWebElement element)
        {
            IWebElement newElement = element;

            tb.LogMe(newElement);

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
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsElementPresent(this By element)
        {
            By newElement = element;

            tb.LogMe(newElement.ToString());
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

        

        public static IWebElement WaitForElement(this By element, int seconds)
        {
            By newElement = element;

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
                tb.LogMe(newElement.ToString(), ex);
                return null;
            }
        }

    }
}

