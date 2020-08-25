using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    [CompilerGenerated]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

        public static Settings Default => defaultInstance;


        [DebuggerNonUserCode]
        [DefaultSettingValue("10")]
        [UserScopedSetting]
        public int UpdateDayTradesCount
        {
            get
            {
                return (int)this["UpdateDayTradesCount"];
            }
            set
            {
                this["UpdateDayTradesCount"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("Persian")]
        [UserScopedSetting]
        public string Language
        {
            get
            {
                return (string)this["Language"];
            }
            set
            {
                this["Language"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool ExportDaysWithoutTrade
        {
            get
            {
                return (bool)this["ExportDaysWithoutTrade"];
            }
            set
            {
                this["ExportDaysWithoutTrade"] = value;
            }
        }

        [DefaultSettingValue("True")]
        [DebuggerNonUserCode]
        [UserScopedSetting]
        public bool ShowHeaders
        {
            get
            {
                return (bool)this["ShowHeaders"];
            }
            set
            {
                this["ShowHeaders"] = value;
            }
        }

        [DebuggerNonUserCode]
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string StorageLocation
        {
            get
            {
                return (string)this["StorageLocation"];
            }
            set
            {
                this["StorageLocation"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        [UserScopedSetting]
        public string FileName
        {
            get
            {
                return (string)this["FileName"];
            }
            set
            {
                this["FileName"] = value;
            }
        }

        [DebuggerNonUserCode]
        [UserScopedSetting]
        [DefaultSettingValue("csv")]
        public string FileExtension
        {
            get
            {
                return (string)this["FileExtension"];
            }
            set
            {
                this["FileExtension"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        [UserScopedSetting]
        public string Encoding
        {
            get
            {
                return (string)this["Encoding"];
            }
            set
            {
                this["Encoding"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue(",")]
        [UserScopedSetting]
        public char Delimeter
        {
            get
            {
                return (char)this["Delimeter"];
            }
            set
            {
                this["Delimeter"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("1380/01/01")]
        [UserScopedSetting]
        public string StartDate
        {
            get
            {
                return (string)this["StartDate"];
            }
            set
            {
                this["StartDate"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool UseProxy
        {
            get
            {
                return (bool)this["UseProxy"];
            }
            set
            {
                this["UseProxy"] = value;
            }
        }

        [DefaultSettingValue("")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }

        [DebuggerNonUserCode]
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string Port
        {
            get
            {
                return (string)this["Port"];
            }
            set
            {
                this["Port"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string UserName
        {
            get
            {
                return (string)this["UserName"];
            }
            set
            {
                this["UserName"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        [DebuggerNonUserCode]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool AutomaticOpenFolder
        {
            get
            {
                return (bool)this["AutomaticOpenFolder"];
            }
            set
            {
                this["AutomaticOpenFolder"] = value;
            }
        }

        [DefaultSettingValue("False")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public bool RemoveOldFiles
        {
            get
            {
                return (bool)this["RemoveOldFiles"];
            }
            set
            {
                this["RemoveOldFiles"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        [UserScopedSetting]
        public bool ExcelOutput
        {
            get
            {
                return (bool)this["ExcelOutput"];
            }
            set
            {
                this["ExcelOutput"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        [DebuggerNonUserCode]
        public int LastInstrumentReceiveDate
        {
            get
            {
                return (int)this["LastInstrumentReceiveDate"];
            }
            set
            {
                this["LastInstrumentReceiveDate"] = value;
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        [DebuggerNonUserCode]
        public bool EnableDecompression
        {
            get
            {
                return (bool)this["EnableDecompression"];
            }
            set
            {
                this["EnableDecompression"] = value;
            }
        }

        [ApplicationScopedSetting]
        [SpecialSetting(SpecialSetting.WebServiceUrl)]
        [DefaultSettingValue("http://service.tsetmc.com/WebService/TseClient.asmx")]
        [DebuggerNonUserCode]
        public string TseClient_ServerSerive_TseClient => (string)this["TseClient_ServerSerive_TseClient"];

        [DefaultSettingValue("http://service.tsetmc.com/tsev2/data/TseClient2.aspx")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public string TseClientServerUrl
        {
            get
            {
                return (string)this["TseClientServerUrl"];
            }
            set
            {
                this["TseClientServerUrl"] = value;
            }
        }

        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        [UserScopedSetting]
        public bool UseWebService
        {
            get
            {
                return (bool)this["UseWebService"];
            }
            set
            {
                this["UseWebService"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool AdjustPrices
        {
            get
            {
                return (bool)this["AdjustPrices"];
            }
            set
            {
                this["AdjustPrices"] = value;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string AdjustedStorageLocation
        {
            get
            {
                return (string)this["AdjustedStorageLocation"];
            }
            set
            {
                this["AdjustedStorageLocation"] = value;
            }
        }



        [DefaultSettingValue("False")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public bool AdjustPricesOnShareIncrement
        {
            get
            {
                return (bool)this["AdjustPricesOnShareIncrement"];
            }
            set
            {
                this["AdjustPricesOnShareIncrement"] = value;
            }
        }

        [DefaultSettingValue("0")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public int AdjustPricesCondition
        {
            get
            {
                return (int)this["AdjustPricesCondition"];
            }
            set
            {
                this["AdjustPricesCondition"] = value;
            }
        }

        [DefaultSettingValue("0")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public int LastDeven
        {
            get
            {
                return (int)this["LastDeven"];
            }
            set
            {
                this["LastDeven"] = value;
            }
        }
    }
}
