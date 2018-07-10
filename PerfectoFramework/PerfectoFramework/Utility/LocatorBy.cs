using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateWeb.Utility
{

    public class LocatorBy : OpenQA.Selenium.By
    {
        public LocatorBy(string label)
        {

            ReadOnlyCollection<IWebElement> mockElements = null;

            FindElementMethod = (ISearchContext context) =>
            {

                String[] locate = TestBase.locators[label].Split(new char[] { '=' }, 1);

                return findElement(context, locate);

            };

            FindElementsMethod = (ISearchContext context) =>
            {
                String[] locate = TestBase.locators[label].Split(new char[] { '=' }, 1);

                return findElements(context, locate);
               
            };
        }



        public static By Locator(String label)
        {

            String[] locate = TestBase.locators[label].Split(new char[] { '=' }, 1);
            
            if (locate[0].ToLower() == "name")
            {
                return By.Name(locate[1]);
            }
            else if (locate[0].ToLower() == "id")
            {
                return By.Id(locate[1]);
            }
            else if (locate[0].ToLower() == "xpath")
            {
                return By.XPath(locate[1]);
            }
            else if (locate[0].ToLower() == "css")
            {
                return By.CssSelector(locate[1]);
            }
            else if (locate[0].ToLower() == "link" || locate[0].ToLower() == "linktext")
            {
                return By.LinkText(locate[1]);
            }
            else if (locate[0].ToLower() == "partiallink" || locate[0].ToLower() == "partiallinktext")
            {
                return By.PartialLinkText(locate[1]);
            }
            else if (locate[0].ToLower() == "classname")
            {
                return By.ClassName(locate[1]);
            }
            else if (locate[0].ToLower() == "tagname")
            {
                return By.TagName(locate[1]);
            }
            else
            {
                return By.XPath(locate[0]);
            }

        }

        public IWebElement findElement(ISearchContext context, String[] locate)
        {
            if (locate[0].ToLower() == "name")
            {
                return context.FindElement(By.Name(locate[1]));
            }
            else if (locate[0].ToLower() == "id")
            {
                return context.FindElement(By.Id(locate[1]));
            }
            else if (locate[0].ToLower() == "xpath")
            {
                return context.FindElement(By.XPath(locate[1]));
            }
            else if (locate[0].ToLower() == "css")
            {
                return context.FindElement(By.CssSelector(locate[1]));
            }
            else if (locate[0].ToLower() == "link" || locate[0].ToLower() == "linktext")
            {
                return context.FindElement(By.LinkText(locate[1]));
            }
            else if (locate[0].ToLower() == "partiallink" || locate[0].ToLower() == "partiallinktext")
            {
                return context.FindElement(By.PartialLinkText(locate[1]));
            }
            else if (locate[0].ToLower() == "classname")
            {
                return context.FindElement(By.ClassName(locate[1]));
            }
            else if (locate[0].ToLower() == "tagname")
            {
                return context.FindElement(By.TagName(locate[1]));
            }
            else
            {
                return context.FindElement(By.XPath(locate[0]));
            }
        }

        public ReadOnlyCollection<IWebElement> findElements(ISearchContext context, String[] locate)
        {
             if (locate[0].ToLower() == "name")
                {
                    return context.FindElements(By.Name(locate[1]));
                }
                else if (locate[0].ToLower() == "id")
                {
                    return context.FindElements(By.Id(locate[1]));
                }
                else if (locate[0].ToLower() == "xpath")
                {
                    return context.FindElements(By.XPath(locate[1]));
                }
                else if (locate[0].ToLower() == "css")
                {
                    return context.FindElements(By.CssSelector(locate[1]));
                }
                else if (locate[0].ToLower() == "link" || locate[0].ToLower() == "linktext")
                {
                    return context.FindElements(By.LinkText(locate[1]));
                }
                else if (locate[0].ToLower() == "partiallink" || locate[0].ToLower() == "partiallinktext")
                {
                    return context.FindElements(By.PartialLinkText(locate[1]));
                }
                else if (locate[0].ToLower() == "classname")
                {
                    return context.FindElements(By.ClassName(locate[1]));
                }
                else if (locate[0].ToLower() == "tagname")
                {
                    return context.FindElements(By.TagName(locate[1]));
                }
                else
                {
                    return context.FindElements(By.XPath(locate[0]));
                }
        }


    }
}