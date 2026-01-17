using System;
using System.Windows.Forms;

namespace SQL_Document_Builder
{
    public static class AISettingsManager
    {
        private static AISettings? _settings;
        public static AISettings Current
        {
            get
            {
                if (_settings == null)
                    _settings = AISettings.Load();
                return _settings;
            }
        }

        public static void Save()
        {
            _settings?.Save();
        }

        public static void Reload()
        {
            _settings = AISettings.Load();
        }
    }
}
