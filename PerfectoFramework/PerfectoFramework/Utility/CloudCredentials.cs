using System.Runtime.Serialization;

namespace PerfectoLab.VisualStudioPerfectoLabServer
{
    [DataContract]
    public class CloudCredentials
    {
        [DataMember]
        public string UserName { get; set; }

        public int VisualStudioInstanceId { get; set; }
    }
}