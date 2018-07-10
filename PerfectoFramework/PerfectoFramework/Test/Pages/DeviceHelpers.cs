using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerfectoFramework
{
    class DeviceHelpers
    {

        TestBase tb;
        RemoteWebDriverExtended driver;

        public DeviceHelpers(TestBase tb)
        {
            this.tb = tb;
            this.driver = tb.driver;
        }


        public void InitializeChrome()
        {

            if (driver.GetOS() == "Android")
            {

                driver.GetNative();

                try
                {
                    driver.FindElementByLocator("DeviceHelper.Chrome.Accept").WaitForElement(5000).Click();
                }
                catch (Exception ex)
                {

                }

                try
                {
                    driver.FindElement(LocatorBy.Locator("DeviceHelper.Chrome.NoThanks")).Click();
                    
                }
                catch (Exception ex)
                {

                }

               
                

                driver.GetPreviousContext();
            }
        }
    }
}