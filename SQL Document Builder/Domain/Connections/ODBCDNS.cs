using System.Collections;
using System.Collections.Generic;

namespace SQL_Document_Builder
{
    internal class ODBCDNS
    {
        private readonly List<string> _systemDsn;
        private readonly List<string> _userDsn;

        public ODBCDNS()
        {
            _systemDsn = new List<string>();
            _userDsn = new List<string>();
            GetSystemDataSourceNames();
            GetUserDataSourceNames();
        }

        public enum DataSourceType
        {
            System,
            User
        }

        /// <summary>
        /// Gets all data source name on the local machine
        /// </summary>
        public SortedList AllDSNs
        {
            get
            {
                var result = new SortedList();
                foreach (var item in _systemDsn)
                {
                    result.Add(item, DataSourceType.System);
                }
                foreach (var item in _userDsn)
                {
                    result.Add(item, DataSourceType.User);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets all system data source name on the local machine
        /// </summary>
        public List<string> SystemDsns
        {
            get { return _systemDsn; }
        }

        /// <summary>
        /// Gets all user data source name on the local machine
        /// </summary>
        public List<string> UserDsns
        {
            get { return _userDsn; }
        }

        /// <summary>
        /// Gets all System data source names for the local machine.
        /// </summary>
        private void GetSystemDataSourceNames()
        {
            try
            {
                // get system dsn's
                Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.LocalMachine).OpenSubKey("Software");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC");
                    if (reg != null)
                    {
                        reg = reg.OpenSubKey("ODBC.INI");
                        if (reg != null)
                        {
                            reg = reg.OpenSubKey("ODBC Data Sources");
                            if (reg != null)
                            {
                                // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                                foreach (string sName in reg.GetValueNames())
                                {
                                    _systemDsn.Add(sName);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception) {/* ignore this exception */}
        }

        /// <summary>
        /// Gets all User data source names for the local machine.
        /// </summary>
        private void GetUserDataSourceNames()
        {
            // get user dsn's
            try
            {
                Microsoft.Win32.RegistryKey reg = (Microsoft.Win32.Registry.CurrentUser).OpenSubKey("Software");
                if (reg != null)
                {
                    reg = reg.OpenSubKey("ODBC");
                    if (reg != null)
                    {
                        reg = reg.OpenSubKey("ODBC.INI");
                        if (reg != null)
                        {
                            reg = reg.OpenSubKey("ODBC Data Sources");
                            if (reg != null)
                            {
                                // Get all DSN entries defined in DSN_LOC_IN_REGISTRY.
                                foreach (string sName in reg.GetValueNames())
                                {
                                    _userDsn.Add(sName);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception) {/* ignore this exception */}
        }
    }
}