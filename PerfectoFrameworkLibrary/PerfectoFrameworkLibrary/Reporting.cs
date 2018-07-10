﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ExportAPI
{
    public class Reporting
    {
        // The Perfecto Continuous Quality Lab you work with
        private static string CQL_NAME;

        // The reporting Server address depends on the location of the lab. Please refer to the documentation at
        // http://developers.perfectomobile.com/display/PD/Reporting#Reporting-ReportingserverAccessingthereports to find your relevant address
        // For example the following is used for US:
        private static string REPORTING_SERVER_URL = "https://{0}.reporting.perfectomobile.com";

        // See http://developers.perfectomobile.com/display/PD/Using+the+Reporting+Public+API on how to obtain an Offline Token
        // Please Insert Offline Token in the ver.settings file
        public static string OFFLINE_TOKEN;

        private static string CQL_SERVER_URL = "";

        public static void initiateDownload(string token, string location, string host)
        {

            OFFLINE_TOKEN = token;
            CQL_NAME = host;

            JObject executions = retrieveTestExecutions();
            JToken resources = executions["resources"];
            if (null == resources | 0 == resources.Count())
            {
                Console.WriteLine(); //TODO: add message
            }
            else
            {
                JToken testExecution = resources.First;
                string testId = testExecution["id"].ToString();
                string driverExecutionId = testExecution["externalId"].ToString();

                // Retrieves a list of commands of a single test (as a json)
                //retrieveTestCommands(testId);

                // Download an execution summary PDF report of an execution (may contain several tests)
                downloadExecutionSummaryReport(driverExecutionId, location);

                // Download a PDF report of a single test
                downloadTestReport(testId, location);

                // Download video
                //downloadVideo(testExecution);

                // Download attachments such as device logs, vitals or network files (relevant for Mobile tests only)
                //downloadAttachments(testExecution);
            }
        }

        private static void downloadAttachments(JToken testExecution, string location)
        {
            Downloader.downloadAttachments(testExecution, location);
        }

        private static void downloadVideo(JToken testExecution, string location)
        {
            Downloader.downloadVideo(testExecution, location);
        }

        private static void downloadTestReport(string testId, string location)
        {
            Console.WriteLine("Downloading test report for test ID: " + testId);
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(buildRequestURI("/export/api/v1/test-executions/pdf/" + testId, null));
            request.Headers.Add("PERFECTO_AUTHORIZATION", OFFLINE_TOKEN);
            Downloader.downloadFile(testId, request, ".pdf", "PDF Test Report", location);
        }

        private static void downloadExecutionSummaryReport(string driverExecutionId, string location)
        {
            Console.WriteLine("Downloading Execution summary for execution ID: " + driverExecutionId);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("externalId[0]", driverExecutionId);
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(buildRequestURI("/export/api/v1/test-executions/pdf", parameters));
            request.Headers.Add("PERFECTO_AUTHORIZATION", OFFLINE_TOKEN);
            Downloader.downloadFile(driverExecutionId, request, ".pdf", "Execution summary PDF report", location);
        }

        private static void retrieveTestCommands(string testId)
        {
            HttpWebRequest request =
                (HttpWebRequest) WebRequest.Create(buildRequestURI("/export/api/v1/test-executions/" + testId + "/commands", null));
            request.Headers.Add("PERFECTO_AUTHORIZATION", OFFLINE_TOKEN);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            JObject commands = JObject.Parse(reader.ReadToEnd());
            Console.WriteLine(commands);
        }

        private static JObject retrieveTestExecutions()
        {
            ulong startTime = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds() - 30 * 24 * 60 * 60 * 1000UL;
            ulong endTime = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("startExecutionTime[0]", startTime.ToString());
            parameters.Add("endExecutionTime[0]", endTime.ToString());

            HttpWebRequest request =
                 (HttpWebRequest) WebRequest.Create(buildRequestURI("/export/api/v1/test-executions", parameters));

            request.Headers.Add("PERFECTO_AUTHORIZATION", OFFLINE_TOKEN);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            JObject executions = JObject.Parse(reader.ReadToEnd());
            return executions;
        }

        private static string buildRequestURI(string suffix, Dictionary<string, string> parameters)
        {
            string uri = String.Format(REPORTING_SERVER_URL, CQL_NAME) +  suffix + "?";
            if (null != parameters)
            {
                foreach (var keyValuePair in parameters)
                {
                    uri += keyValuePair.Key + "=" + keyValuePair.Value + "&";
                }
                uri = uri.Substring(0, uri.Length - 1);
            }
            return uri;
        }
    }
}
