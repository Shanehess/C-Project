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

namespace StateWeb
{

    public partial class TestBase
    {
        public RemoteWebDriverExtended driver;
        public TestContext TestContext { get; set; }
        public string testName;
        public Properties testProperties;
        public Properties mainProperties;
        public Dictionary<string, string> properties = new Dictionary<string, string>();
        public string type;
        public string os;
        public string version;
        public string manufacturer;
        public string description;
        public string model;
        public string browser;
        public string testType;
        public string deviceID;
        public string applicationName;
        public string repositoryKey;
        public string uploadLocation;
        public string host;
        public string user;
        public string password;
        public string snapshotDirectory;
        public string reportDirectory;
        public string attachmentDirectory;
        public bool overwriteUpload;
        public int waitTime;
        public bool appInstalled = false;
        public RemoteWebDriverExtended.sensorInstrument sensorInstrument;
        public RemoteWebDriverExtended.webInstrument webInstrument;
        public ReportiumClient reportiumClient;
        public static Dictionary<string, string> locators = new Dictionary<string, string>();

        public TestBase(string prop)
        {
            setup(prop);
        }



        public ReportiumClient CreateReportingClient(RemoteWebDriverExtended driver)
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
            additionalCapabilities, description, overwriteUpload, sensorInstrument, webInstrument, pageObjects, uploadLocation, type, deviceid, os, version, manufacturer, model, browser, testType, applicationName, repositoryKey
        }

        public enum mainProp
        {
            host, user, pass, snapshotDirectory, attachmentDirectory, reportDirectory, waitTime
        }


        public string getTestProp(testProp propName)
        {
            return testProperties.get(propName.ToString());
        }

        public string getMainProp(mainProp propName)
        {
            return mainProperties.get(propName.ToString());
        }

        

        public void loadLocators()
        {

            foreach (string dirFile in Directory.GetDirectories(".\\"))
            {


                foreach (string fileName in Directory.GetFiles(dirFile + "\\PageProperties"))
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

        }

        public void loadProperties()
        {

            foreach (string dirFile in Directory.GetDirectories(".\\"))
            {


                foreach (string fileName in Directory.GetFiles(dirFile + "\\PageProperties"))
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
        }

        public string getProperties(String property)
        {
            return properties[property].ToString();
        }



        public void setup(string prop)
        {



            mainProperties = new Properties("main.prop");
            testProperties = new Properties(prop);
            loadLocators();

            loadProperties();
            host = getMainProp(mainProp.host);
            user = (getMainProp(mainProp.user));
            password = getMainProp(mainProp.pass);
            type = getTestProp(testProp.type);
            os = (getTestProp(testProp.os) == "") ? null : getTestProp(testProp.os);
            version = (getTestProp(testProp.version) == "") ? null : getTestProp(testProp.version);
            manufacturer = (getTestProp(testProp.manufacturer) == "") ? null : getTestProp(testProp.manufacturer);
            model = (getTestProp(testProp.model) == "") ? null : getTestProp(testProp.model);
            browser = (getTestProp(testProp.browser) == "") ? null : getTestProp(testProp.browser);
            deviceID = (getTestProp(testProp.deviceid) == "") ? "" : getTestProp(testProp.deviceid);
            testType = (getTestProp(testProp.testType) == "") ? null : getTestProp(testProp.testType);
            applicationName = (getTestProp(testProp.applicationName) == "") ? null : getTestProp(testProp.applicationName);
            repositoryKey = (getTestProp(testProp.repositoryKey) == "") ? null : getTestProp(testProp.repositoryKey);
            uploadLocation = (getTestProp(testProp.uploadLocation) == "") ? null : getTestProp(testProp.uploadLocation);
            snapshotDirectory = (getMainProp(mainProp.snapshotDirectory) == "") ? null : getMainProp(mainProp.snapshotDirectory);
            attachmentDirectory = (getMainProp(mainProp.attachmentDirectory) == "") ? null : getMainProp(mainProp.attachmentDirectory);
            reportDirectory = (getMainProp(mainProp.reportDirectory) == "") ? null : getMainProp(mainProp.reportDirectory);
            overwriteUpload = (getTestProp(testProp.overwriteUpload) == "") ? false : bool.Parse(getTestProp(testProp.overwriteUpload));
            waitTime = (getMainProp(mainProp.waitTime) == "") ? 0 : int.Parse(getMainProp(mainProp.waitTime));
            description = (getTestProp(testProp.description) == "") ? "" : getTestProp(testProp.description);

            if (getTestProp(testProp.webInstrument) == "true")
            {
                webInstrument = RemoteWebDriverExtended.webInstrument.instrument;
            }
            else if (getTestProp(testProp.webInstrument) == "false")
            {
                webInstrument = RemoteWebDriverExtended.webInstrument.noinstrument;
            }

            if (getTestProp(testProp.sensorInstrument) == "true")
            {
                sensorInstrument = RemoteWebDriverExtended.sensorInstrument.sensor;
            }
            else if (getTestProp(testProp.sensorInstrument) == "false")
            {
                sensorInstrument = RemoteWebDriverExtended.sensorInstrument.nosensor;
            }

            connectLab();

            CreateReportingClient(driver);
            reportiumClient = CreateReportingClient(driver);
            reportiumClient.testStart("iPhoneSearch", new Reportium.test.TestContextTags());

            if (os == "Android" && testType == "Web")
            {
                driver.cleanApplication("chrome");
            }

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


        public void connectLab()
        {
            if (type != "Mobile")
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();

                if (browser == "Internet Explorer")
                {
                    capabilities.SetCapability("ignoreProtectedModeSettings", true);
                }

                capabilities.SetCapability("browserName", browser);

                driver = new RemoteWebDriverExtended(host, capabilities, waitTime);

            }
            else
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

                capabilities.SetCapability("automatonName", "Appium");


                capabilities.SetPerfectoLabExecutionId(host);

                capabilities = additionalCapabilities(capabilities);

                driver = new RemoteWebDriverExtended(host, capabilities, waitTime);

            }
        }

        public DesiredCapabilities additionalCapabilities(DesiredCapabilities capabilities)
        {
            string capabilitiesArray = getTestProp(testProp.additionalCapabilities);

            Dictionary<string, string> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(capabilitiesArray);


            foreach (KeyValuePair<string, string> entry in dict)
            {
                capabilities.SetCapability(entry.Key.ToString(), entry.Value.ToString());
            }

            return capabilities;
        }

        public void initializeApp()
        {
            appInstalled = driver.IsAppInstalledByName(applicationName);
            if (testType == "Application" && uploadLocation != "" && appInstalled == false)
            {
                driver.uploadMedia(uploadLocation, repositoryKey, overwriteUpload);
                driver.InstallApp(repositoryKey, sensorInstrument, webInstrument);
                appInstalled = true;
                driver.cleanApplication(applicationName);
            }
        }

        public void close()
        {
            if (testType == "Application")
            {
                driver.uninstallApplication(applicationName);
            }

            try
            {
                driver.Close();
            }
            catch (Exception)
            {

            }

            if (type == "Mobile")
            {
                try
                {
                    //downloadReport("pdf", "Report", ".pdf");
                }
                catch (Exception)
                {

                }
            }

            if (type == "Mobile")
            {
                try
                {
                    //downloadAttachment("video", "Video", ".flv");
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

        public void cleanAllDirs()
        {
            cleanDir(snapshotDirectory);
            cleanDir(reportDirectory);
            cleanDir(attachmentDirectory);
        }

        public void cleanDir(string location)
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

    }
}