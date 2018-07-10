using PerfectoLab;
using PerfectoLab.Properties;
using PerfectoLab.VisualStudioPerfectoLabServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace OpenQA.Selenium.Remote
{




    public static class PerfectoLabUtils
    {


        public static void SetPerfectoLabExecutionId(this DesiredCapabilities source, string host)
        {
            string executionId;
            string hostAddress;
            PerfectoLabUtils.GetPluginData(out executionId, out hostAddress);
            if (string.IsNullOrEmpty(executionId) || !host.ToLower().Contains(hostAddress.ToLower()))
                return;
            source.SetCapability(Settings.Default.ExecutionIdCapabilityName, (object)executionId);
        }




        private static void GetPluginData(out string executionId, out string hostAddress)
        {
            executionId = (string)null;
            hostAddress = (string)null;
            using (VisualStudioPerfectoLabServerProxy perfectoLabServerProxy = new VisualStudioPerfectoLabServerProxy(new EndpointAddress(string.Format(Settings.Default.PerfectoLabPluginServiceAddress, (object)ProccessService.GetVisualStudioId()))))
            {
                bool flag = false;
                try
                {
                    executionId = perfectoLabServerProxy.GetExecutionId();
                    hostAddress = perfectoLabServerProxy.GetPluginCloudeAddress();
                    if (perfectoLabServerProxy.State == CommunicationState.Faulted)
                        return;
                    PerfectoLabUtils.CloseConnection((object)perfectoLabServerProxy);
                    flag = true;
                }
                catch
                {
                }
                finally
                {
                    if (!flag)
                        perfectoLabServerProxy.Abort();
                }
            }
        }

        private static void CloseConnection(object service)
        {
            if (!(service is ICommunicationObject))
                return;
            ICommunicationObject communicationObject = service as ICommunicationObject;
            try
            {
                communicationObject.Close();
            }
            catch (FaultException ex)
            {
                communicationObject.Abort();
            }
            catch (CommunicationException ex)
            {
                communicationObject.Abort();
            }
            catch (TimeoutException ex)
            {
                communicationObject.Abort();
            }
        }


    }
}
