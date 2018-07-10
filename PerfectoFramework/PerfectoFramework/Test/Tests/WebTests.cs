using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using PerfectoFramework;

namespace PerfectoFramework 
{
    [TestClass]
    public class Tests : Listener
    {
       
        [TestMethod]
        public void SearchIphoneWeb()
        {
            TestBase tb = new TestBase("iOS\\iphoneSearch.prop",TestContext.TestName);
            setBase(tb);
            Search(tb);
        }

        [TestMethod]
        public void SearchAndroidWeb()
        {
            TestBase tb = new TestBase("Android\\androidSearch.prop", TestContext.TestName);
            setBase(tb);
            Search(tb);
        }

        public void Search(TestBase tb)
        {

            search.Home homePage = new search.Home(tb);
            homePage.LaunchSite();
        }

    }
}
