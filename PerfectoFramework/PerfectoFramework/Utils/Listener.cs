using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reportium.test.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectoFramework
{
    [TestClass]
    public class Listener
    {
        public TestContext TestContext { get; set; }

        public TestBase tb;

        public void setBase(TestBase tb)
        {
            this.tb = tb;

        }



        [TestInitialize]
        public void Initialize()
        {
         
        }
        

        [TestCleanup()]
        public void CleanUp()
        {
            try
            {
                //test success, generates successful reporting 
                if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed)
                {
                    tb.reportiumClient.testStop(TestResultFactory.createSuccess());
                }
                //test fail, generates failure repostiung
                else
                {
                    tb.reportiumClient.testStop(TestResultFactory.createFailure("Test Failed", null));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            tb.close();    
        }

        

    }
}
