using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeleniumExtensions;

namespace StateWeb.Utility
{
    class DeviceHelpers
    {
        private RemoteWebDriverExtended driver;

        [FindsBy(How = How.XPath, Using = "//*[@resource-id=\"com.android.chrome:id/terms_accept\"]")]
        //[CacheLookup]
        private IWebElement Accept { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[@resource-id=\"com.android.chrome:id / negative_button\"]")]
        //[CacheLookup]
        private IWebElement Thanks { get; set; }

        [FindsBy(How = How.XPath, Using = "//*[@text='Done']")]
        //[CacheLookup]
        private IWebElement Done { get; set; }

        public DeviceHelpers(TestBase b)
        {
            this.driver = b.driver;
            SeleniumExtensions.SeleniumExtensions.setLibrary(b);
            PageFactory.InitElements(driver, this);
        }


        public void initializeChrome()
        {

            //if (driver.getOS() == "Android")
            //{
                driver.GetNative();
            Console.Out.WriteLine(driver.Context);


            try
            {
                System.Threading.Thread.Sleep(10000);
                        Accept.Click();
                Thanks.Click();
                Done.Click();
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
                  
                       
                  

                


                driver.GetPreviousContext();
            //}
        }
    }
}