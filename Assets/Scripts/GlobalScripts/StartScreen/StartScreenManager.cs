using System;
using System.Collections.Generic;
using IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GlobalScripts.StartScreen
{
    public class StartScreenManager : MonoBehaviour
    {
        public Button newProjectButton;
        public Button openProjectButton;
        public Button createProjectButton;
        public GameObject recentLocationPrefab;
        public GameObject fileSpace;
        public GameObject newProjectPopUp;
        public GameObject projectNameField;

        //short name and complete name
        private List<KeyValuePair<string, string>> _files = new List<KeyValuePair<string, string>>();

        private void Start()
        {
            _files = SaveManager.Current.GetLoadFiles();
            FillRecentLocationPrefab();
            newProjectButton.onClick.AddListener(newProjectButtonClick);
            openProjectButton.onClick.AddListener(openProjectButtonClick);
            createProjectButton.onClick.AddListener(CreateNewProject);
        }

        private void CreateNewProject()
        {
            if (SaveManager.Current.CreateNewSave(projectNameField.GetComponent<TMP_InputField>().text))
            {
                SceneManager.LoadScene(sceneName: "CreationScene");
            }
            else
            {
                
            }
        }
        
        private void FillRecentLocationPrefab()
        {
            if (_files.Count < 1)
            {
                return;
            }
            
            foreach (KeyValuePair<string, string> fileData in _files)
            {
                GameObject newObj = Instantiate(fileSpace, recentLocationPrefab.transform);
                newObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fileData.Key;
                FileButton button = newObj.AddComponent<FileButton>();
                button.SetFileName(fileData.Key);
                button.SetFileLocation(fileData.Value);
            }
        }
        
        private void newProjectButtonClick()
        {
            newProjectPopUp.SetActive(true);
            // SceneManager.LoadScene(sceneName: "CreationScene");
        }

        private void openProjectButtonClick()
        {
            
        }
    }
}
