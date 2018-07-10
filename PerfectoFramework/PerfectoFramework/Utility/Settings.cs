// Decompiled with JetBrains decompiler
// Type: PerfectoLab.Properties.Settings
// Assembly: PerfectoLab, Version=10.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 531781DB-4194-4A8D-B8E1-DB2C0966BF16
// Assembly location: C:\Users\under\Documents\Visual Studio 2015\Projects\PerfectoLabSeleniumTestProject4\packages\PerfectoLab.10.3.0.0\lib\PerfectoLab.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PerfectoLab.Properties
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)SettingsBase.Synchronized((SettingsBase)new Settings());

        public static Settings Default
        {
            get
            {
                return Settings.defaultInstance;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("eclipseExecutionId")]
        [ApplicationScopedSetting]
        public string ExecutionIdCapabilityName
        {
            get
            {
                return (string)this[nameof(ExecutionIdCapabilityName)];
            }
        }

        [DefaultSettingValue("net.pipe://localhost/{0}/VisualStudioPerfectoLabServer")]
        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        public string PerfectoLabPluginServiceAddress
        {
            get
            {
                return (string)this[nameof(PerfectoLabPluginServiceAddress)];
            }
        }
    }
}
