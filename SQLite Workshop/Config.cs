using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace SQLiteWorkshop
{
    class Config
    {

        internal const string CFG_FTOP              = "FTOP";                       //Form Top
        internal const string CFG_FLEFT             = "FLEFT";                      //Form Left
        internal const string CFG_FWIDTH            = "FWIDTH";                     //Form Width
        internal const string CFG_FHEIGHT           = "FHEIGHT";                    //Form Height
        internal const string CFG_VSPLITP           = "VSPLITP";                    //Vertical Splitter position
        internal const string CFG_HSPLITP           = "HSPLITP";                    //SQLControl Horizontal Splitter position
        internal const string CFG_RECENTDB          = "RECENTDB";                   //Recently Opened Databases
        internal const string CFG_LASTOPEN          = "LASTOPEN";                   //Last directory with a DB successfully opened by FileOpen dialog
        internal const string CFG_LASTBKPOPEN       = "LASTBKPOPEN";                //Last directory with a successful backup DB selection
        internal const string CFG_TABLEEDITHSPLITP  = "iCFG_TABLEEDITHSPLITP";      //Table Editor Splitter position


        //Configuration File
        const string CONFIGFILENAME = "SQLite_Workshop.config";
        string _configFile;
        Configuration cfg;

        internal Config()
        {
            _configFile = string.Format(@"{0}\{1}\{2}\{3}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ajmsoft", "SQLite_Workshop", CONFIGFILENAME);
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = _configFile;
            cfg = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            Properties.Settings.Default.Reload();
        }
        internal string appsetting(string setting)
        {
            return cfg.AppSettings.Settings[setting] == null ? null : cfg.AppSettings.Settings[setting].Value;
        }

        internal void SetSetting(string setting, string value)
        {
            if (appsetting(setting) == null)
            {
                cfg.AppSettings.Settings.Add(setting, value);
            }
            else
            {
                cfg.AppSettings.Settings[setting].Value = value;
            }
            cfg.Save(ConfigurationSaveMode.Minimal, true);
            ConfigurationManager.RefreshSection("appSettings");
            Properties.Settings.Default.Reload();
            return;

        }
    }
}
