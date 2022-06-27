using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;

namespace IO
{
    public class SaveManager
    {
        private static SaveManager _current;
        private static readonly object Padlock = new object();
        public static SaveManager Current
        {
            get
            {
                lock (Padlock)
                {
                    if (_current == null)
                    {
                        _current = new SaveManager();
                    }
                    return _current;
                }
            }
        }
        
        private readonly string _saveDir = Application.persistentDataPath + "/saves";

        public void OnSave()
        {
            
        }

        public bool CreateNewSave(string projectName)
        {
            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }
            
            return Serializer.Save(projectName, SaveData.Current);
        }

        public List<KeyValuePair<string, string>> GetLoadFiles()
        {
            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }
            
            DirectoryInfo di = new DirectoryInfo(_saveDir);
            FileSystemInfo[] files = di.GetFileSystemInfos();

            if (files.Length < 1)
            {
                return new List<KeyValuePair<string, string>>();
            }
            
            var orderedFiles = files.OrderByDescending(f => f.LastAccessTime);
            List<KeyValuePair<string, string>> returnArray = new List<KeyValuePair<string, string>>();
            
            foreach (var file in orderedFiles)
            {
                string[] titleArray = file.Name.Split("/");
                returnArray.Add(new KeyValuePair<string,string>(titleArray[^1], file.FullName));
            }

            return returnArray;
        }

        public void OnLoad(string fileLocation)
        {
            
        }
    }
}