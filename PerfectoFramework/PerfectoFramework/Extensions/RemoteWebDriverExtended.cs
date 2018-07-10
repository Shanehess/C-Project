using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Drawing;
using System.Net.Http;
using System.IO;
using PerfectoFramework;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.Events;
using System.Runtime.CompilerServices;

public enum ContextType
{
    NATIVE_APP, WEBVIEW, VISUAL, UNKNOWN
}

public class RemoteWebDriverExtended : RemoteWebDriver
{
    private ContextType context = ContextType.UNKNOWN;
    private DesiredCapabilities capabilities;
    public ContextType previousContext;
    private string TestObjects;
    private string host;
    private string TestName;
    private TestBase tb;




    public IWebElement FindElementByLocator(string locator)
    {
        tb.LogMe("Finding Element: " + LocatorBy.Locator(locator) + ", with Context: " + Context);
        var wait = new DefaultWait<RemoteWebDriverExtended>(this);
        wait.Timeout = TimeSpan.FromSeconds(WaitTimeOut.TotalSeconds);
        wait.PollingInterval = TimeSpan.FromMilliseconds(500);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        return wait.Until<IWebElement>(ExpectedConditions.ElementExists(LocatorBy.Locator(locator)));
    }

    public new ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        tb.LogMe("Finding Element: " + by + ", with Context: " + Context);
        var wait = new DefaultWait<RemoteWebDriverExtended>(this);
        wait.Timeout = TimeSpan.FromSeconds(WaitTimeOut.TotalSeconds);
        wait.PollingInterval = TimeSpan.FromMilliseconds(500);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        return wait.Until<ReadOnlyCollection<IWebElement>>(ExpectedConditions.PresenceOfAllElementsLocatedBy(by));
    }

    public new IWebElement FindElement(By by)
    {
        tb.LogMe("Finding Element: " + by + ", with Context: " + Context);
        var wait = new DefaultWait<RemoteWebDriverExtended>(this);
        wait.Timeout = TimeSpan.FromSeconds(WaitTimeOut.TotalSeconds);
        wait.PollingInterval = TimeSpan.FromMilliseconds(500);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        return wait.Until<IWebElement>(ExpectedConditions.ElementExists(by));
    }

    public ContextType Context
    {
        get
        {


            return context = ConvertString(Execute("getContext", null).Value.ToString());


        }
        set
        {
            previousContext = Context;

            Execute("setContext", new Dictionary<string, object> { { "name", value.ToString() } });
            context = value;
            tb.LogMe(value.ToString());
        }
    }

    public void WaitTimeOutReset()
    {
        WaitTimeOut = TimeSpan.Parse(TestBase.mainProperties.get(TestBase.mainProp.waitTime.ToString()));
    }

    private static ContextType ConvertString(string ctx)
    {

        if (ctx.ToLower().Contains("webview"))
            return ContextType.WEBVIEW;
        if (ctx.ToLower().Contains("native"))
            return ContextType.NATIVE_APP;
        if (ctx.ToLower().Contains("visual"))
            return ContextType.VISUAL;
        return ContextType.UNKNOWN;
    }


    public void KeyboardSendText(string text)
    {

        tb.LogMe("keyboardSendText(" + text + ")");

        try
        {
            Keyboard.SendKeys(text);
        }
        catch (Exception ex)
        {
            tb.log(ex.ToString());
        }
    }

    public new DesiredCapabilities Capabilities
    {
        get { return capabilities; }
        private set
        {
            String capString = "Driver Capabilities:\n\t";
            capabilities = new DesiredCapabilities();
            foreach (var cap in value.ToDictionary())
            {
                capabilities.SetCapability(cap.Key, cap.Value);
                capString += cap.Key + "=" + cap.Value + "|";
            }
            foreach (var prop in DeviceProperties)
            {
                capabilities.SetCapability(prop.Key, prop.Value);
                capString += prop.Key + "=" + prop.Value + "|";
            }
            tb.LogMe(capString);
        }
    }

    private Dictionary<string, object> DeviceProperties
    {
        get
        {
            var properties = new Dictionary<string, object>();
            Object test = ExecuteScript("mobile:handset:info", new Dictionary<string, object> { { "property", "ALL" } });


            var result =
    Regex.Split(test.ToString().Substring(1, test.ToString().Length - 2), @",(?=[^\]]*(?:\[|$))");

            for (int i = 1; i < result.Length; i += 2)
            {
                properties.Add(result[i - 1].Trim(), result[i].Trim());
            }
            return properties;
        }
    }

    public TimeSpan WaitTimeOut { get; set; }


    public RemoteWebDriverExtended(String host, DesiredCapabilities capabilities, int waitTimeoutSec, TestBase tb)
        : base(new HttpAuthenticatedCommandExecutor(new Uri("http://" + host + "/nexperience/perfectomobile/wd/hub"), TimeSpan.FromSeconds(120), true, capabilities), capabilities)
    {
        this.host = host;
        this.tb = tb;
        Capabilities = (DesiredCapabilities)base.Capabilities;
        WaitTimeOut = TimeSpan.FromSeconds(waitTimeoutSec);

        tb.log("Initiating driver connection with capabilities" + capabilities.ToString());
    }


    public new object ExecuteScript(string command, params object[] parameters)
    {
        tb.LogMe("Executing Command: " + command + ", with parameters: "
            + ToDebugString(parameters[0] as Dictionary<string, object>));
        return base.ExecuteScript(command, parameters[0]);
    }

    public void GetNative()
    {
        Context = ContextType.NATIVE_APP;
        tb.log("Switching to native context");
    }

    public void GetWeb()
    {
        Context = ContextType.WEBVIEW;
        tb.log("Switching to web context");
    }



    public void CleanBrowser()
    {
        try
        {
            if (Capabilities.GetCapability("os") + "" != "Android") return;
            OpenApp("Chrome");
            ExecuteScript("mobile:browser:clean", new Dictionary<string, object>());
            OpenApp("Chrome");
        }
        catch (Exception ex)
        {
            tb.LogMe("CleanBrowser", ex);
        }
    }



    public void ClickImage(string imagePath)
    {

        string command = "mobile:button-image:click";
        Dictionary<String, object> parameters = new Dictionary<String, Object>();
        parameters.Add("label", imagePath);
        parameters.Add("timeout", "20");
        parameters.Add("threshold", "95");
        parameters.Add("match", "bounded");
        parameters.Add("imageBounds.needleBound", "60");
        Object obj = ExecuteScript(command, parameters);

        if (obj.ToString() != "OK")
        {

            tb.LogMe("ClickImage", new Exception("Click Image Failed"));
        }

    }


    public void GoToUrl(string url)
    {
        try
        {
            Navigate().GoToUrl(url);
        }
        catch (Exception ex)
        {
            tb.LogMe("GoToURL", ex);
        }

    }

    public void WaitOnWebPageLoad(double time)
    {
        try
        {
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this, TimeSpan.FromSeconds(time));
            wait.Until(driver1 => ((IJavaScriptExecutor)this).ExecuteScript("return document.readyState").Equals("complete"));
        }

        catch (Exception ex)
        {
            tb.LogMe("GoToURL", ex);
        }

    }



    public string GetUrl()
    {
        return Url.ToString();
    }



    private static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {

        return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value)) + "}";

    }

    public void GetPreviousContext()
    {
        if (previousContext == ContextType.WEBVIEW)
        {
            if (!(Context == ContextType.WEBVIEW))
            {
                tb.LogMe("restoreContext: " + ContextType.WEBVIEW);
                GetWeb();
            }
        }
        else if (previousContext == ContextType.NATIVE_APP)
        {
            if (!(Context == ContextType.NATIVE_APP))
            {
                GetNative();
                tb.LogMe("restoreContext: " + ContextType.NATIVE_APP);
            }
        }
    }

    public string GetOS()
    {
        return DeviceProperties["os"].ToString();
    }

    public string GetOSVersion()
    {
        return DeviceProperties["osVersion"].ToString();
    }

    public string GetManufacturer()
    {
        return DeviceProperties["manufacturer"].ToString();
    }

    public string GetModel()
    {
        return DeviceProperties["model"].ToString();
    }

    public string GetDescription()
    {
        return DeviceProperties["description"].ToString();
    }

    public string GetDeviceID()
    {
        return DeviceProperties["deviceName"].ToString();
    }



    public string GetBrowser()
    {
        return Capabilities.BrowserName;
    }

    public string GetHost()
    {
        return Capabilities.GetCapability("host").ToString();
    }


    public void PressEnter()
    {
        try
        {
            if (GetOS() == "Android")
            {
                SendKeyEvent("66");
            }
            else if (GetOS() == "iOS")
            {
                ClickImage("PUBLIC:Jeremy\\iOS\\KeyboardSearch.png");
            }
            else
            {
                Keyboard.SendKeys(Keys.Enter);
            }
        } 
        catch(Exception ex)
        {
            tb.LogMe("PressEnter", ex);
        }
    }

    public void UploadMedia(string path, string repositoryKey, Boolean overwrite)
    {
        try
        {
            string urlStr = "https://" + host + "/services/repositories/media/" + repositoryKey + "?operation=upload&overwrite=" + overwrite.ToString() + "&user=" + Capabilities.GetCapability("user") + "&password=" + Capabilities.GetCapability("password");
            UploadFile(urlStr, path);
        }
         
        catch(Exception ex)
        {
            tb.LogMe("UploadMedia", ex);
        }
    }

    private void UploadFile(string url, string filename)
    {

        Uri uri = new Uri(url);
        using (var stream = File.OpenRead(filename))
        {
            HttpClient client = new HttpClient();
            var response = client.PutAsync(uri, new StreamContent(stream)).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
            }
        }
    }


    public void SendKeyEvent(string key)
    {
        
            string command = "mobile:key:event";
            Dictionary<string, object> parameters = new Dictionary<string, Object>();
            parameters.Add("key", key);
        Object obj = ExecuteScript(command, parameters);

        if (obj.ToString() != "OK")
        {

            tb.LogMe("SendKeyEvent", new Exception("Send Key Event Failed"));
        }

    }

    public void VisualSetText(string label, string labelOffset, string text)
    {

        string command = "mobile:edit-text:set";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("label", label);
        parameters.Add("text", text);
        parameters.Add("label.offset", labelOffset);
        Object obj = ExecuteScript(command, parameters);

        if (obj.ToString() != "OK")
        {

            tb.LogMe("VisualSetText", new Exception("Visual Set Text Command Failed"));
        }
    }


    public string GetWindowSize()
    {

        return Manage().Window.Size.ToString();
    }

    public string GetWindowPosition()
    {

        return Manage().Window.Position.ToString();
    }

    public void MaximizeWindow()
    {



        Manage().Window.Maximize();

    }

    public void SetWindowPosition(int x, int y)
    {


        Manage().Window.Position = new Point(x, y);

    }

    public void SetWindowSize(int x, int y)
    {


        Manage().Window.Size = new Size(x, y);

    }





    public void Sleep(int sleeptime)
    {
        System.Threading.Thread.Sleep(sleeptime);
    }


    public enum WebInstrument
    {
        noinstrument, instrument
    }

    public enum SensorInstrument
    {
        nosensor, sensor
    }

    /**
    * Install app.
    *
    * @param repoLocation the path of the install file in the Perfecto Cloud Repository
*/
    public void InstallApp(string repoLocation, SensorInstrument sins, WebInstrument wins)
    {
        Object obj = ExecuteScript("mobile:application:install", new Dictionary<string, object> { { "file", repoLocation }, { "instrument", wins.ToString() }, { "sensorInstrument", sins.ToString() } });
     
        if (obj.ToString() != "OK")
        {

            tb.LogMe("Install App", new Exception("Install App Failed"));
        }
    }

    /**
     * Start app.
     *
     * @param appName the app name
     */
    public void OpenApp(string appName)
    {
        Object obj = ExecuteScript("mobile:application:open", new Dictionary<string, object> { { "name", appName } });

        if (obj.ToString() != "OK")
        {

            tb.LogMe("Open App", new Exception("Open App Failed"));
        }
    }

    /**
     * Close app.
     *
     * @param appName the app name
     */
    public void CloseApp(String appName)
    {
        try
        {
            ExecuteScript("mobile:application:close", new Dictionary<string, object> { { "name", appName } });
        }
        catch (Exception e)
        {

        }
    }

    public void UninstallApplication(string appName)
    {
        string command = "mobile:application:uninstall";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("name", appName);
        Object obj= ExecuteScript(command, parameters);
        if (obj.ToString() != "OK")
        {

            tb.LogMe("Uninstall App", new Exception("Uninstall App Failed"));
        }
    }

    /**
    * IsAppInstalledById.
    *
    * @param appId the Appium appID
*/
    public bool IsAppInstalledById(String appId)
    {
        try
        {
            Dictionary<String, Object> param = new Dictionary<string, object>();
            param.Add("format", "identifier");
            String appList = (String)ExecuteScript("mobile:application:find", param);

            if (appList.IndexOf(appId) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            tb.LogMe(appId + "could not be found because of ERROR:\n\t" + e.Message);
            return false;
        }
    }

    /**
    * IsAppInstalledByName.
    *
    * @param appName the appName
*/
    public bool IsAppInstalledByName(String appName)
    {
        try
        {
            Dictionary<String, Object> param = new Dictionary<string, object>();
            param.Add("format", "name");
            String appList = (String)ExecuteScript("mobile:application:find", param);

            if (appList.IndexOf(appName) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            tb.LogMe(appName + "could not be found because of ERROR:\n\t" + e.Message);
            return false;
        }
    }

    public void CleanApplication(string application)
    {
        string command = "mobile:application:clean";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("name", application);
        Object obj = ExecuteScript(command, parameters);

        if (obj.ToString() != "OK")
        {

            tb.LogMe("Clean Application", new Exception("Clean App Failed"));
        }



        System.Threading.Thread.Sleep(5000);
    }

    public enum AndroidAppIdentifer
    {
        settings, wifiSettings, runningServices, manageApplications, batteryStats, dateTime, language, security, simcard, dev, appUsage, deviceInfo, userDictionary
    }

    public string GetAndroidIdentifer(AndroidAppIdentifer a)
    {
        switch (a)
        {
            case AndroidAppIdentifer.settings:
                return "com.android.settings/.Settings";
                break;
            case AndroidAppIdentifer.wifiSettings:
                return "com.android.settings/.wifi.WifiSettings";
                break;
            case AndroidAppIdentifer.runningServices:
                return "com.android.settings/.RunningServices";
                break;
            case AndroidAppIdentifer.manageApplications:
                return "com.android.settings/.ManageApplications";
                break;
            case AndroidAppIdentifer.batteryStats:
                return "com.android.settings/.fuelgauge.PowerUsageSummary";
                break;
            case AndroidAppIdentifer.dateTime:
                return "com.android.settings/.DateTimeSettingsSetupWizard";
                break;
            case AndroidAppIdentifer.language:
                return "com.android.settings/.LanguageSettings";
                break;
            case AndroidAppIdentifer.security:
                return "com.android.settings/.SecuritySettings";
                break;
            case AndroidAppIdentifer.simcard:
                return "com.android.settings/.IccLockSettings";
                break;
            case AndroidAppIdentifer.dev:
                return "com.android.settings/.DevelopmentSettings";
                break;
            case AndroidAppIdentifer.appUsage:
                return "com.android.settings/.UsageStats";
                break;
            case AndroidAppIdentifer.deviceInfo:
                return "com.android.settings/.deviceinfo.Status";
                break;
            case AndroidAppIdentifer.userDictionary:
                return "com.android.settings/.UserDictionarySettings";
                break;
            default:
                return "";
                break;
        }
    }
}
