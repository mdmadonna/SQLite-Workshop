using System;
using System.Configuration;

using static SQLiteWorkshop.Common;

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
        internal const string CFG_DFLTSQLDIR        = "DEFAULTSQLDIR";              //Default Directory for SQL Files
        internal const string CFG_REGISTEREDDBS     = "REGISTEREDDBS";              //Registered Databases
        internal const string CFG_TEMPLATESVISIBLE  = "TEMPLATESVISIBLE";           //Template tree visible
        internal const string CFG_PROPSVISIBLE      = "PROPSVISIBLE";               //Properties visible
        internal const string CFG_TSPLITP           = "TSPLITP";                    //Template (Right Panel) splitter position
        internal const string CFG_PSPLITP           = "PSPLITP";                    //Property splitter position
        internal const string CFG_ANALYZEWARN       = "ANALYZEWARN";                //Analyzer warning message
        internal const string CFG_COLUMNEDITWARN    = "COLUMNEDITWARN";             //Column editor warning
        internal const string CFG_ROWEDITWARN       = "ROWEDITWARN";                //Row editor warning
        internal const string CFG_RECOVERDBWARN     = "RECOVERDBWARN";              //Recover Database warning
        internal const string CFG_LANGUAGE          = "LANGUAGE";                   //Default Language
        internal const string CFG_TOOLSLOCATION     = "TOOLSLOCATION";              //Download location for SQLite tools
        internal const string CFG_KEYPHRASE         = "KEYPHRASE";                  //Security Key
        internal const string CFG_FUNCNOLOAD        = "FUNCNOLOAD";                 //Built-In functions to not Load when Connection is opened
        internal const string CFG_OPENLASTDB        = "OPENLASTDB";                 //Open the last used database at startup
        internal const string CFG_IGNOREIMPERRORS   = "IGNOREIMPORTERRORS";         //Ignore All Import Errors
        internal const string CFG_MAXIMPERRORS      = "MAXIMPORTERRORS";            //Maximum number of allowed import errors
        internal const string CFG_SAVEIMPORT        = "SAVEIMPORT";                 //Save Import Credentials
        internal const string CFG_IMPHISTORY        = "IMPHIST";                    //Saved Import History
        internal const string CFG_IMPTEXT           = "IMPTEXT";                    //Imported Text File History
        internal const string CFG_IMPEXCEL          = "IMPEXCEL";                   //Imported Excel File History
        internal const string CFG_IMPSQLITE         = "IMPSQLITE";                  //Imported SQLite Database History
        internal const string CFG_IMPMYSQL          = "IMPMYSQL";                   //Imported MySql Database History
        internal const string CFG_IMPSQLSERVER      = "IMPSQLSERVER";               //Imported SQL Server Database History
        internal const string CFG_IMPMSACCESS       = "IMPMSACCESS ";               //Imported MSAccess Database History
        internal const string CFG_IMPODBC           = "IMPODBC";                    //Imported ODBC Database History
        internal const string CFG_IMPSQL            = "IMPSQL";                     //Imported SQL File History


        //Configuration File
        const string CONFIGFILENAME = "SQLite_Workshop.config";
        string _configFile;
        Configuration cfg;
        static bool ConfigModified = false;

        internal Config()
        {
            ReloadConfig();
        }

        internal string appSetting(string setting)
        {
            return cfg.AppSettings.Settings[setting]?.Value;
        }

        internal void setSetting(string setting, string value)
        {
            try
            {
                if (appSetting(setting) == null)
                {
                    cfg.AppSettings.Settings.Add(setting, value);
                }
                else
                {
                    cfg.AppSettings.Settings[setting].Value = value;
                }
                cfg.Save(ConfigurationSaveMode.Minimal, true);
                ConfigurationManager.RefreshSection("appSettings");
                ConfigModified = false;
            }
            catch (Exception ex)
            {
                // If another instance or external application has updated the 
                // configuration file, reload it and retry the update.
                if (ex.Message.StartsWith(ERR_CONFIGCHANGED))
                {
                    if (!ConfigModified)
                    {
                        ReloadConfig();
                        ConfigModified = true;
                        setSetting(setting, value);
                        ConfigModified = false;
                    }
                    return;
                }
                ShowMsg(string.Format("Error Saving SQLite Workshop Configuration Data\r\n{0}", ex.Message));
            }
            finally
            {
                Properties.Settings.Default.Reload();
            }
            return;

        }

        private void ReloadConfig()
        {
            _configFile = string.Format(@"{0}\{1}\{2}", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite_Workshop", CONFIGFILENAME);
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = _configFile;
            cfg = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            Properties.Settings.Default.Reload();
        }
    }
}
