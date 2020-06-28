using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat
{
    class DiskManager
    {
        string m_settingsFile;
        public DiskManager(string settings)
        {
            m_settingsFile = settings;
        }
        public string LoadUserSettingsFromDisk(string user)
        {
            if (File.Exists(m_settingsFile))
            {
                using (StreamReader sr = File.OpenText(m_settingsFile))
                {
                    string line = "";
                    bool isNeededUser = false;
                    string userData = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("user:"))
                        {
                            isNeededUser = false;
                        }
                        if (line.Contains(user))
                        {
                            isNeededUser = true;
                        }
                        if (isNeededUser)
                        {
                            userData += line;
                        }
                    }
                    return userData;
                }
            }
            else
            {
                File.CreateText(m_settingsFile);
            }
            return "";
        }

        void SaveUserSettingsToDisk(string user, string data)
        {
            if (!File.Exists(m_settingsFile))
            {
                File.CreateText(m_settingsFile);
            }
            string allUsersSettings = "";
            using (StreamReader sr = File.OpenText(m_settingsFile))
            {
                allUsersSettings = sr.ReadToEnd();
            }

        }
    }
}
