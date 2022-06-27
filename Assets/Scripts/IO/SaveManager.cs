using System;
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

        private string _currentProjectName;
        private readonly string _saveDir = Application.persistentDataPath + "/saves";

        public bool OnSave()
        {
            SavedData saveData = new SavedData
            {
                buttons = SaveData.Current.buttons, 
                components = SaveData.Current.components
            };
            return Serializer.Save(_currentProjectName, saveData);
        }

        public bool CreateNewSave(string projectName)
        {
            if (projectName.Length < 1)
            {
                return false;
            }
            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }

            _currentProjectName = projectName;
            SavedData saveData = new SavedData
            {
                buttons = new List<ButtonData>(), 
                components = new List<ComponentData>()
            };
            return Serializer.Save(projectName, saveData);
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

        public SavedData OnLoad(string fileLocation, string projectName)
        {
            _currentProjectName = projectName.Split(".")[0];
            return Serializer.Load(fileLocation);
            
        }
    }
}