// Decompiled with JetBrains decompiler
// Type: PerfectoLab.VisualStudioPerfectoLabServer.VisualStudioPerfectoLabServerProxy
// Assembly: PerfectoLab, Version=10.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 531781DB-4194-4A8D-B8E1-DB2C0966BF16
// Assembly location: C:\Users\under\Documents\Visual Studio 2015\Projects\PerfectoLabSeleniumTestProject4\packages\PerfectoLab.10.3.0.0\lib\PerfectoLab.dll

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace PerfectoLab.VisualStudioPerfectoLabServer
{
    public class VisualStudioPerfectoLabServerProxy : ClientBase<IVisualStudioPerfectoLabServer>, IVisualStudioPerfectoLabServer
    {
        public VisualStudioPerfectoLabServerProxy(EndpointAddress endpointAddress)
          : base(new ServiceEndpoint(ContractDescription.GetContract(typeof(IVisualStudioPerfectoLabServer)), (Binding)new NetNamedPipeBinding(), endpointAddress))
        {
        }

        public string GetExecutionId()
        {
            return this.Channel.GetExecutionId();
        }

        public string GetPluginCloudeAddress()
        {
            return this.Channel.GetPluginCloudeAddress();
        }
    }
}
