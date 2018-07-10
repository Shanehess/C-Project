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
using StateWeb;
using System.Diagnostics.CodeAnalysis;


public enum ContextType
{
    NATIVE_APP, WEBVIEW, VISUAL, UNKNOWN
}

public class RemoteWebDriverExtended : RemoteWebDriver
{
    private ContextType context = ContextType.UNKNOWN;
    private DesiredCapabilities capabilities;
    public ContextType previousContext;




    public string testObjects;
    public string host;
    public string testName;

    public ContextType Context
    {
        get
        {
           
            try
            {
                return context = ConvertString(Execute("getContext", null).Value.ToString());
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to get Context!");
                return context;
            }
        }
        set
        {
            previousContext = Context;

            Execute("setContext", new Dictionary<string, object> { { "name", value.ToString() } });
            context = value;
            Debug.WriteLine("Set Context: " + value);
        }
    }


    private static ContextType ConvertString(string ctx)
    {
        Debug.WriteLine("Got Context: " + ctx);
        if (ctx.ToLower().Contains("webview"))
            return ContextType.WEBVIEW;
        if (ctx.ToLower().Contains("native"))
            return ContextType.NATIVE_APP;
        if (ctx.ToLower().Contains("visual"))
            return ContextType.VISUAL;
        return ContextType.UNKNOWN;
    }


    public void keyboardSendText(string text)
    {
        Debug.WriteLine("keyboardSendText(" + text + ")");

        Keyboard.SendKeys(text);

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
            Debug.WriteLine(capString);
        }
    }

    private Dictionary<string, object> DeviceProperties
    {
        get
        {
            var properties = new Dictionary<string, object>();
            Object test = ExecuteScript("mobile:handset:info", new Dictionary<string, object> { { "property", "ALL" } });
            String[] props = test.ToString().Split(']')[0].Split('[')[1].Split(',');
            for (int i = 1; i < props.Length; i += 2)
            {
                properties.Add(props[i - 1].Trim(), props[i].Trim());
            }
            return properties;
        }
    }

    public TimeSpan WaitTimeOut { get; set; }
    public TimeSpan implicitTimeOut { get; set; }
    public TimeSpan pageTimeOut { get; set; }

    /**
    * Set page and implicit timeout to zero so they dont interfere with other
    * waits
    */
    public void ZeroTimeouts()
    {
        Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(1);
        Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
    }

    public RemoteWebDriverExtended(String host, DesiredCapabilities capabilities, int waitTimeoutSec)
        : base(new HttpAuthenticatedCommandExecutor(new Uri("http://" + host + "/nexperience/perfectomobile/wd/hub"), TimeSpan.FromSeconds(120), true, capabilities), capabilities)
    {
        this.host = host;
        Capabilities = (DesiredCapabilities)base.Capabilities;
        ZeroTimeouts();
        WaitTimeOut = TimeSpan.FromSeconds(waitTimeoutSec);        
    }


    public new object ExecuteScript(string command, params object[] parameters)
    {
        Debug.WriteLine("Executing Command: " + command + ", with parameters: "
            + ToDebugString(parameters[0] as Dictionary<string, object>));
        return base.ExecuteScript(command, parameters[0]);
    }

    public void GetNative()
    {
        Context = ContextType.NATIVE_APP;
    }

    public void GetVisual()
    {
        Context = ContextType.VISUAL;
    }

    public void GetWeb()
    {
        Context = ContextType.WEBVIEW;
    }

    public new ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        Debug.WriteLine("Finding Elements: " + by + ", with Context: " + Context);
        return base.FindElements(by);
    }

    public void CleanBrowser()
    {
        if (Capabilities.GetCapability("os") + "" != "Android") return;
        ExecuteScript("mobile:browser:clean", new Dictionary<string, object>());
    }



    public void clickImage(string imagePath)
    {
        GetVisual();

        string command = "mobile:button-image:click";
        Dictionary<String, object> parameters = new Dictionary<String, Object>();
        parameters.Add("label", imagePath);
        parameters.Add("timeout", "20");
        parameters.Add("threshold", "95");
        parameters.Add("match", "bounded");
        parameters.Add("imageBounds.needleBound", "60");
        ExecuteScript(command, parameters);

        GetPreviousContext();
    }


    public void goToUrl(string url)
    {

        try
        {
            Navigate().GoToUrl(url);           
        }
        catch (Exception ex)
        {


        }

    }

    //public void waitOnPageLoad()
    //{
    //    if (testType == "Web")
    //    {
    //        if (type == "Mobile")
    //        {
    //            if (Context.ToString().Contains("WEB"))
    //            {
    //                Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
    //                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this, TimeSpan.FromSeconds(30.00));
    //                wait.Until(driver1 => ((IJavaScriptExecutor)this).ExecuteScript("return document.readyState").Equals("complete"));
    //                Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(implicitWaitTime));
    //            }
    //        }
    //        else
    //        {
    //            Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
    //            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this, TimeSpan.FromSeconds(30.00));
    //            wait.Until(driver1 => ((IJavaScriptExecutor)this).ExecuteScript("return document.readyState").Equals("complete"));
    //            Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(implicitWaitTime));
    //        }
    //    }
    //}


    public string getUrl()
    {
        string url = "can't find url";
        try
        {
            url = Url.ToString();
        }
        catch (Exception ex)
        {


        }

        return url;
    }



    public static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        try
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value)) + "}";
        }
        catch
        {
            return "Dictionary convert to string failed.";
        }
    }

    public void GetPreviousContext()
    {
        if (previousContext == ContextType.WEBVIEW)
        {
            if (!(Context == ContextType.WEBVIEW))
            {
                Debug.WriteLine("restoreContext: " + ContextType.WEBVIEW);
                GetWeb();
            }
        }
        else if (previousContext == ContextType.NATIVE_APP)
        {
            if (!(Context == ContextType.NATIVE_APP))
            {
                GetNative();
                Debug.WriteLine("restoreContext: " + ContextType.NATIVE_APP);
            }
        }
        else if (previousContext == ContextType.VISUAL)
        {
            if (!(Context == ContextType.VISUAL))
            {
                GetVisual();
                Debug.WriteLine("restoreContext: " + ContextType.VISUAL);
            }
        }
    }

    public string getOS()
    {
        return Capabilities.GetCapability("os").ToString();
    }

    public string getOSVersion()
    {
        return Capabilities.GetCapability("osVersion").ToString();
    }

    public string getManufacturer()
    {
        return Capabilities.GetCapability("manufacturer").ToString();
    }

    public string getModel()
    {
        return Capabilities.GetCapability("model").ToString();
    }

    public string getDescription()
    {
        return Capabilities.GetCapability("description").ToString();
    }



    public string getBrowser()
    {
        return Capabilities.BrowserName;
    }

    public string getHost()
    {
        return Capabilities.GetCapability("host").ToString();
    }

    public string getDeviceID()
    {
        return Capabilities.GetCapability("deviceName").ToString();
    }

    


    public void pressEnter()
    {

        if (getOS() == "Android")
        {
            sendKeyEvent("66");
        }
        else if (getOS() == "iOS")
        {
            clickImage("PUBLIC:Jeremy\\iOS\\KeyboardSearch.png");
        }
        else
        {
            Keyboard.SendKeys(Keys.Enter);
        }
    }

    public void uploadMedia(string path, string repositoryKey, Boolean overwrite)
    {
        string urlStr = "https://" + host + "/services/repositories/media/" + repositoryKey + "?operation=upload&overwrite=" + overwrite.ToString() + "&user=" + Capabilities.GetCapability("user") + "&password=" + Capabilities.GetCapability("password");
        uploadFile(urlStr, path);
    }

    public void uploadFile(string url, string filename)
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


    public void sendKeyEvent(string key)
    {

        string command = "mobile:key:event";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("key", key);
        ExecuteScript(command, parameters);

    }

    public void visualSetText(string label, string labelOffset, string text)
    {
        GetVisual();
        string command = "mobile:edit-text:set";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("label", label);
        parameters.Add("text", text);
        parameters.Add("label.offset", labelOffset);
        ExecuteScript(command, parameters);

        GetPreviousContext();
    }


    public string getWindowSize()
    {


        try
        {
            Manage().Window.Size.ToString();
        }
        catch (Exception ex)
        {


        }

        return Manage().Window.Size.ToString();
    }

    public string getWindowPosition()
    {


        try
        {
            Manage().Window.Position.ToString();
        }
        catch (Exception ex)
        {


        }

        return Manage().Window.Position.ToString();
    }

    public void maximizeWindow()
    {


        try
        {
            Manage().Window.Maximize();
        }
        catch (Exception ex)
        {


        }
    }

    public void setWindowPosition(int x, int y)
    {


        try
        {
            Manage().Window.Position = new Point(x, y);
        }
        catch (Exception ex)
        {


        }
    }

    public void setWindowSize(int x, int y)
    {


        try
        {
            Manage().Window.Size = new Size(x, y);
        }
        catch (Exception ex)
        {


        }
    }





    public void sleep(int sleeptime)
    {
        System.Threading.Thread.Sleep(sleeptime);
    }


    public enum webInstrument
    {
        noinstrument, instrument
    }

    public enum sensorInstrument
    {
        nosensor, sensor
    }

    /**
* Install app.
*
* @param repoLocation the path of the install file in the Perfecto Cloud Repository
*/
    public void InstallApp(string repoLocation, sensorInstrument sins, webInstrument wins)
    {
        ExecuteScript("mobile:application:install", new Dictionary<string, object> { { "file", repoLocation }, { "instrument", wins.ToString() }, { "sensorInstrument", sins.ToString() } });
    }

    /**
     * Start app.
     *
     * @param appName the app name
     */
    public void OpenApp(string appName)
    {
        ExecuteScript("mobile:application:open", new Dictionary<string, object> { { "name", appName } });
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
            Debug.WriteLine(appName + "could not be closed because of ERROR:\n\t" + e.Message);
        }
    }

    public void uninstallApplication(string appName)
    {
        string command = "mobile:application:uninstall";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("name", appName);
        ExecuteScript(command, parameters);
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
            Debug.WriteLine(appId + "could not be found because of ERROR:\n\t" + e.Message);
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
            Debug.WriteLine(appName + "could not be found because of ERROR:\n\t" + e.Message);
            return false;
        }
    }

    public void cleanApplication(string application)
    {
        string command = "mobile:application:clean";
        Dictionary<string, object> parameters = new Dictionary<string, Object>();
        parameters.Add("name", application);
        ExecuteScript(command, parameters);
        System.Threading.Thread.Sleep(5000);
    }

    public enum androidAppIdentifer
    {
        settings, wifiSettings, runningServices, manageApplications, batteryStats, dateTime, language, security, simcard, dev, appUsage, deviceInfo, userDictionary
    }

    public string getAndroidIdentifer(androidAppIdentifer a)
    {
        switch (a)
        {
            case androidAppIdentifer.settings:
                return "com.android.settings/.Settings";
                break;
            case androidAppIdentifer.wifiSettings:
                return "com.android.settings/.wifi.WifiSettings";
                break;
            case androidAppIdentifer.runningServices:
                return "com.android.settings/.RunningServices";
                break;
            case androidAppIdentifer.manageApplications:
                return "com.android.settings/.ManageApplications";
                break;
            case androidAppIdentifer.batteryStats:
                return "com.android.settings/.fuelgauge.PowerUsageSummary";
                break;
            case androidAppIdentifer.dateTime:
                return "com.android.settings/.DateTimeSettingsSetupWizard";
                break;
            case androidAppIdentifer.language:
                return "com.android.settings/.LanguageSettings";
                break;
            case androidAppIdentifer.security:
                return "com.android.settings/.SecuritySettings";
                break;
            case androidAppIdentifer.simcard:
                return "com.android.settings/.IccLockSettings";
                break;
            case androidAppIdentifer.dev:
                return "com.android.settings/.DevelopmentSettings";
                break;
            case androidAppIdentifer.appUsage:
                return "com.android.settings/.UsageStats";
                break;
            case androidAppIdentifer.deviceInfo:
                return "com.android.settings/.deviceinfo.Status";
                break;
            case androidAppIdentifer.userDictionary:
                return "com.android.settings/.UserDictionarySettings";
                break;
            default:
                return "";
                break;
        }
    }
}
