using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;
using System.Web;
using System.IO;
using Reportium.test;
using Reportium.test.Result;
using Reportium.client;
using Reportium.model;
using System.Web.Script.Serialization;
using PerfectoFrameworkLibrary;
using System.Diagnostics;
using OpenQA.Selenium;
using ExportAPI;
using System.Runtime.CompilerServices;

namespace PerfectoFramework
{

    public class TestBase
    {
        public RemoteWebDriverExtended driver;
        public string testName;
        public Properties testProperties;
        public static Properties mainProperties;
        public Dictionary<string, string> properties = new Dictionary<string, string>();
        private string os;
        private string version;
        private string manufacturer;
        private string description;
        private string model;
        private string browser;
        private string deviceID;
        private string host;
        private string user;
        private string password;
        private string snapshotDirectory;
        private string reportDirectory;
        private string automationName;
        private int waitTime;
        private RemoteWebDriverExtended.SensorInstrument sensorInstrument;
        private RemoteWebDriverExtended.WebInstrument webInstrument;
        public ReportiumClient reportiumClient;
        public static Dictionary<string, string> locators = new Dictionary<string, string>();

        public TestBase(string prop, string testName)
        {
            this.testName = testName;
            setup(prop);
            
        }

        public TestBase()
        { }

        private ReportiumClient CreateReportingClient(RemoteWebDriverExtended driver)
        {
            PerfectoExecutionContext perfectoExecutionContext = new PerfectoExecutionContext.PerfectoExecutionContextBuilder()
                .withProject(new Project("My first project", "v1.0")) //optional 
                .withContextTags(new[] { "sample tag1", "sample tag2", "c#" }) //optional 
                .withJob(new Job("Job name", 12345)) //optional 
                .withWebDriver(driver)
                .build();
            return PerfectoClientFactory.createPerfectoReportiumClient(perfectoExecutionContext);
        }

        public enum testProp
        {
            configurationDirectory, additionalCapabilities, description, pageObjects, deviceid, os, version, manufacturer, model, browser, automationName
        }

        public enum mainProp
        {
            host, user, pass, snapshotDirectory, reportDirectory, waitTime, securityToken, downloadReports
        }


        public string getTestProp(testProp propName)
        {
            return testProperties.get(propName.ToString());
        }

        public string getMainProp(mainProp propName)
        {
            return mainProperties.get(propName.ToString());
        }



        private void loadLocators(string directory)
        {



            foreach (string fileName in Directory.GetFiles(".\\" + directory + "\\PageProperties"))
            {
                foreach (String line in System.IO.File.ReadAllLines(fileName))
                {
                    if ((!String.IsNullOrEmpty(line)) &&
                        (!line.StartsWith(";")) &&
                        (!line.StartsWith("#")) &&
                        (!line.StartsWith("'")) &&
                        (line.Contains('=')))
                    {
                        int index = line.IndexOf('=');
                        String key = line.Substring(0, index).Trim();
                        String value = line.Substring(index + 1).Trim();

                        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                            (value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        try
                        {
                            locators.Add(key, value);
                        }
                        catch { }
                    }
                }
            }



        }

        private void loadProperties(string directory)
        {

            foreach (string fileName in Directory.GetFiles(".\\" + directory + "\\PageProperties"))
            {
                foreach (String line in System.IO.File.ReadAllLines(fileName))
                {
                    if ((!String.IsNullOrEmpty(line)) &&
                        (!line.StartsWith(";")) &&
                        (!line.StartsWith("#")) &&
                        (!line.StartsWith("'")) &&
                        (line.Contains('=')))
                    {
                        int index = line.IndexOf('=');
                        String key = line.Substring(0, index).Trim();
                        String value = line.Substring(index + 1).Trim();

                        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                            (value.StartsWith("'") && value.EndsWith("'")))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        try
                        {
                            properties.Add(key, value);
                        }
                        catch { }
                    }
                }
            }

        }

        public string getProperties(String property)
        {
            return properties[property].ToString();
        }



        private void setup(string prop)
        {
            mainProperties = new Properties("main.prop");
            testProperties = new Properties(prop);
            loadLocators(getTestProp(testProp.configurationDirectory));
            loadProperties(getTestProp(testProp.configurationDirectory));
            host = getMainProp(mainProp.host);
            user = (getMainProp(mainProp.user));
            password = getMainProp(mainProp.pass);           
            os = (getTestProp(testProp.os) == "") ? null : getTestProp(testProp.os);
            version = (getTestProp(testProp.version) == "") ? null : getTestProp(testProp.version);
            manufacturer = (getTestProp(testProp.manufacturer) == "") ? null : getTestProp(testProp.manufacturer);
            automationName = (getTestProp(testProp.automationName) == "") ? null : getTestProp(testProp.automationName);
            model = (getTestProp(testProp.model) == "") ? null : getTestProp(testProp.model);
            browser = (getTestProp(testProp.browser) == "") ? null : getTestProp(testProp.browser);
            deviceID = (getTestProp(testProp.deviceid) == "") ? "" : getTestProp(testProp.deviceid);          
            snapshotDirectory = (getMainProp(mainProp.snapshotDirectory) == "") ? null : getMainProp(mainProp.snapshotDirectory);            
            reportDirectory = (getMainProp(mainProp.reportDirectory) == "") ? null : getMainProp(mainProp.reportDirectory);
            waitTime = (getMainProp(mainProp.waitTime) == "") ? 0 : int.Parse(getMainProp(mainProp.waitTime));
            description = (getTestProp(testProp.description) == "") ? "" : getTestProp(testProp.description);


            connectLab();

            CreateReportingClient(driver);
            reportiumClient = CreateReportingClient(driver);
            reportiumClient.testStart("iPhoneSearch", new Reportium.test.TestContextTags());

        }

        public String getCurrentTime()
        {

            DateTime saveNow = DateTime.Now;

            DateTime myDt;
            myDt = DateTime.SpecifyKind(saveNow, DateTimeKind.Local);

            String datePatt = @"yyyyMMdd_HHmmss";
            DateTime dispDt = saveNow;
            String dtString = dispDt.ToString(datePatt);

            return dtString;
        }


        private void connectLab()
        {

            DesiredCapabilities capabilities = new DesiredCapabilities();

            capabilities.SetCapability("user", user);
            capabilities.SetCapability("password", password);

            if (deviceID.Trim() == "")
            {
                capabilities.SetCapability("platformName", os);
                capabilities.SetCapability("platformVersion", version);
                capabilities.SetCapability("browserName", browser);
                capabilities.SetCapability("manufacturer", manufacturer);
                capabilities.SetCapability("model", model);
                capabilities.SetCapability("description", description);
            }
            else
            {
                capabilities.SetCapability("deviceName", deviceID);
                capabilities.SetCapability("browserName", browser);
            }

            capabilities.SetCapability("automationName", automationName);


            capabilities.SetPerfectoLabExecutionId(host);

            capabilities = additionalCapabilities(capabilities);

            driver = new RemoteWebDriverExtended(host, capabilities, waitTime, this);

            SeleniumExtensions.SetLibrary(this);

        }

        private DesiredCapabilities additionalCapabilities(DesiredCapabilities capabilities)
        {
            string capabilitiesArray = getTestProp(testProp.additionalCapabilities);

            if (getTestProp(testProp.additionalCapabilities) != "")
            {
                Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(capabilitiesArray);


                foreach (KeyValuePair<string, string> entry in dict)
                {
                    capabilities.SetCapability(entry.Key.ToString(), entry.Value.ToString());
                }
            }

            return capabilities;
        }

        

        public void close()
        {           
            try
            {
                driver.Close();
            }
            catch (Exception)
            {

            }


            if (getMainProp(mainProp.downloadReports).ToLower() == "true")
            {


                try
                {
                    Reporting.initiateDownload(getMainProp(mainProp.securityToken), getMainProp(mainProp.reportDirectory), getMainProp(mainProp.host).Split('.')[0]);
                }
                catch (Exception)
                {

                }
            }


            try
            {
                driver.Quit();
            }
            catch (Exception)
            {

            }

        }

        private void cleanAllDirs()
        {
            cleanDir(snapshotDirectory);
            cleanDir(reportDirectory);
        }

        private void cleanDir(string location)
        {
            if (Directory.Exists(location))
            {
                DirectoryInfo dir = new DirectoryInfo(location);
                foreach (FileInfo item in dir.GetFiles())
                {
                    item.Delete();
                }
                Directory.Delete(location);
            }

        }

        private void takeScreenStep(string name)        {

            takeScreenShot(name);
        }

        public void takeScreenShot(string name)
        {
            if (!Directory.Exists(snapshotDirectory))
            {
                Directory.CreateDirectory(snapshotDirectory);
            }

            log(snapshotDirectory);

            String now = getCurrentTime();
            Screenshot screen = driver.GetScreenshot();
            String newName = snapshotDirectory + now + "_" + testName + "_" + name  + ".png";
            screen.SaveAsFile(newName, OpenQA.Selenium.ScreenshotImageFormat.Png);
            log("Screenshot: " + newName);

        }

        public void methodError(Exception ex)
        {

            // Get the line number from the stack frame
            var st = new StackTrace(ex, true);

            // Get the line number from the stack frame

            string erroroutput = "Error during execution " + System.Environment.NewLine;


            var frame = st.GetFrame(st.FrameCount - 1);
            erroroutput = erroroutput + System.Environment.NewLine + " " + frame.GetFileLineNumber() + " " + frame.GetMethod() + " " + frame.ToString() + System.Environment.NewLine + System.Environment.NewLine + ex.ToString();

            takeScreenShot("failure");

            Assert.Fail(erroroutput);

        }

        public void log(string msg)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.WriteLine(testName  + "_" + msg);
            Console.WriteLine(testName  + "_" + msg);
            Debug.WriteLine(testName  + "_" + msg);
        }


        public void LogMe(IWebElement element, Exception ex = null)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            if (ex == null)
            {
                log(sf.GetMethod().Name + "(" + element.ToString() + ")");
            }
            else
            {
                log(sf.GetMethod().Name + "Exception: " + sf.ToString());
                methodError(ex);
            }
        }

        public void LogMe(string element, Exception ex = null)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            if (ex == null)
            {
                log(sf.GetMethod().Name + "(" + element + ")");
            }
            else
            {
                log(sf.GetMethod().Name + "Exception: " + sf.ToString());
                methodError(ex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

    }
}