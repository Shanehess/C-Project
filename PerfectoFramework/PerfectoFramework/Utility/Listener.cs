using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reportium.test.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateWeb.Utility
{
    public class Listener : Attribute
    {
        public TestContext TestContext { get; set; }
        public TestBase b = new TestBase();
        public RemoteWebDriverExtended driver;

        [TestInitialize]
        public void Initialize()
        {
            b.testName = TestContext.TestName;
            this.driver = b.driver;
        }

        public void setup(string prop)
        {
            b.setup(prop);
        }

        public Listener()
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
                    b.reportiumClient.testStop(TestResultFactory.createSuccess());
                }
                //test fail, generates failure repostiung
                else
                {
                    b.reportiumClient.testStop(TestResultFactory.createFailure("Test Failed", null));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            b.close();
        }



    }
}
