using System;
using System.Diagnostics;
using IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;

namespace GlobalScripts.StartScreen
{
    public class FileButton: MonoBehaviour,  IPointerClickHandler
    {
        private string _fileName;
        private string _fileLocation;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            SaveManager.Current.OnLoad(_fileLocation);
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;
        }

        public void SetFileLocation(string fileLocation)
        {
            _fileLocation = fileLocation;
        }
    }
}