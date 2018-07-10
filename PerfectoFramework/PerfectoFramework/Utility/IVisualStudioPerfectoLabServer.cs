// Decompiled with JetBrains decompiler
// Type: PerfectoLab.VisualStudioPerfectoLabServer.IVisualStudioPerfectoLabServer
// Assembly: PerfectoLab, Version=10.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 531781DB-4194-4A8D-B8E1-DB2C0966BF16
// Assembly location: C:\Users\under\Documents\Visual Studio 2015\Projects\PerfectoLabSeleniumTestProject4\packages\PerfectoLab.10.3.0.0\lib\PerfectoLab.dll

using System.ServiceModel;

namespace PerfectoLab.VisualStudioPerfectoLabServer
{
    [ServiceContract]
    public interface IVisualStudioPerfectoLabServer
    {
        [OperationContract]
        string GetExecutionId();

        [OperationContract]
        string GetPluginCloudeAddress();
    }
}
