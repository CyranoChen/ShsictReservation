using System;
using System.Collections.Generic;
using System.Linq;
using Shsict.Core.Utility;

namespace Shsict.Reservation.Mvc.Entities
{
    public static class ConfigGlobal
    {
        static ConfigGlobal()
        {
            Init();
        }

        private static Dictionary<string, string> ConfigDictionary { get; set; }
        public static AssemblyInfo Assembly { get; private set; }

        public static void Refresh()
        {
            Init();
        }

        private static void Init()
        {
            Config.Cache.RefreshCache();
            ConfigDictionary = Config.Cache.GetDictionaryByConfigSystem(ConfigSystem.Reservation);

            Assembly = new AssemblyInfo
            {
                Title = ConfigDictionary["AssemblyTitle"] ?? string.Empty,
                Description = ConfigDictionary["AssemblyDescription"] ?? string.Empty,
                Configuration = ConfigDictionary["AssemblyConfiguration"] ?? string.Empty,
                Company = ConfigDictionary["AssemblyCompany"] ?? string.Empty,
                Product = ConfigDictionary["AssemblyProduct"] ?? string.Empty,
                Copyright = ConfigDictionary["AssemblyCopyright"] ?? string.Empty,
                Trademark = ConfigDictionary["AssemblyTrademark"] ?? string.Empty,
                Culture = ConfigDictionary["AssemblyCulture"] ?? string.Empty,
                Version = ConfigDictionary["AssemblyVersion"] ?? string.Empty,
                FileVersion = ConfigDictionary["AssemblyFileVersion"] ?? string.Empty
            };
        }

        public static bool IsSystemAdmin(string userId)
        {
            if (!string.IsNullOrEmpty(userId) && SystemAdmin.Length > 0)
            {
                return SystemAdmin.Any(a => a.Equals(userId, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        #region Members and Properties

        public static string[] SystemAdmin
        {
            get
            {
                var admins = ConfigDictionary["SystemAdmin"];
                return admins.Split('|');
            }
        }

        public static bool SystemActive
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigDictionary["SystemActive"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool SchedulerActive
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigDictionary["SchedulerActive"]);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool WeChatActive
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(ConfigDictionary["WeChatActive"]);
                }
                catch
                {
                    return true;
                }
            }
        }

        public static string WeChatAppKey => ConfigDictionary["WeChatAppKey"];

        public static string WeChatAppSecret => ConfigDictionary["WeChatAppSecret"];

        public static string WeChatServiceUrl => ConfigDictionary["WeChatServiceUrl"];

        public static short[] MenuDuration
        {
            get
            {
                var strHours = ConfigDictionary["MenuDuration"];

                if (!string.IsNullOrEmpty(strHours))
                {
                    return strHours.Split('|').Select(x => Convert.ToInt16(x)).ToArray();
                }
                else
                {
                    return new short[] { 7, 9, 18, 20 };
                }
            }
        }

        public static short DefaultExcelFontSize
        {
            get
            {
                if (ConfigDictionary.ContainsKey("DefaultExcelFontSize") &&
                    short.TryParse(ConfigDictionary["DefaultExcelFontSize"], out var fontsize))
                {
                    return fontsize;
                }
                else
                {
                    return 18;
                }
            }
        }

        #endregion
    }

    public static class ConfigGlobalSecureNode
    {
        static ConfigGlobalSecureNode()
        {
            Init();
        }

        private static Dictionary<string, string> ConfigDictionary { get; set; }

        public static void Refresh()
        {
            Init();
        }

        private static void Init()
        {
            Config.Cache.RefreshCache();
            ConfigDictionary = Config.Cache.GetDictionaryByConfigSystem(ConfigSystem.SecureNode);
        }

        #region Members and Properties

        public static short[] ShiftDuration
        {
            get
            {
                var strHours = ConfigDictionary["ShiftDuration"];

                if (!string.IsNullOrEmpty(strHours))
                {
                    return strHours.Split('|').Select(x => Convert.ToInt16(x)).ToArray();
                }
                else
                {
                    return new short[] { 8, 19 };
                }
            }
        }

        public static short DailyCheckLimit
        {
            get
            {
                if (ConfigDictionary.ContainsKey("DailyCheckLimit") &&
                    short.TryParse(ConfigDictionary["DailyCheckLimit"], out var limit))
                {
                    return limit;
                }
                else
                {
                    return 3;
                }
            }
        }

        #endregion
    }

}