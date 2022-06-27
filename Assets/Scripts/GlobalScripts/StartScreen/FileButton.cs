using System;
using System.Diagnostics;
using GlobalScripts.Creation;
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
            SavedData saveObject = SaveManager.Current.OnLoad(_fileLocation, _fileName);

            SaveData.Current.buttons = saveObject.buttons;
            SaveData.Current.components = saveObject.components;

            SceneManager.LoadScene(sceneName:"CreationScene");
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