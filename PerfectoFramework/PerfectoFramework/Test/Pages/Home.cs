using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PerfectoFramework;
using Newtonsoft.Json;

namespace PerfectoFramework.search
{
    class Home
    {

        TestBase tb;
        RemoteWebDriverExtended driver;

        public Home(TestBase tb)
        {
            this.tb = tb;
            this.driver = tb.driver;
        }

       
        public void LaunchSite()
        {
            

            driver.CleanBrowser();
            DeviceHelpers dh = new DeviceHelpers(tb);
            dh.InitializeChrome();

            driver.GoToUrl(tb.getProperties("site"));

        }
    }
}
