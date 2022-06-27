using System;
using System.Collections.Generic;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace UI
{
//  uses the observer pattern in order to create an event listener which can update individual tabs as if they were an actual group 
    public class TabGroup : MonoBehaviour
    {
        [Header("The clickable tabs")]
        public List<Tab> tabs;
        
        [Header("Tab textures")]
        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;
        
        [Header("The windows each tab opens. Linked on Key")]
        public List<GameObject> objectsToSwap;

        [Header("Reference to the sheetController and differentiates the tab groups")] 
        public bool templateTab;
        
        private Tab _selectedTab;

        private void Awake()
        {
            EventManager.Current.ONClickSheetArea += _hardResetTabs;
        }

        private void _hardResetTabs()
        {
            if (_selectedTab != null)
            {
                _selectedTab = null;
                ResetTabs();
                _updateTabWindows(-1);
            }
        }

//      subscribes a tab to a tabgroup
        public void Subscribe(Tab tab)
        {
            if (tabs == null)
            {
                tabs = new List<Tab>();
            }

            tabs.Add(tab);
        }

//      Gets called when a mouse enters a tab (hover)
        public void OnTabEnter(Tab tab)
        {
            ResetTabs();

            if (_selectedTab == null || tab != _selectedTab)
            {
                tab.background.sprite = tabHover;
            }
        }

//      Gets called when a mouse exits a tab (stops hovering)
        public void OnTabExit(Tab tab)
        {
            ResetTabs();
        }

//      Gets called when a tab gets clicked
        public void OnTabSelected(Tab tab)
        {
            SheetAreaController.Current.OnTabOpen(templateTab);

            if (_selectedTab == tab)
            {
                _selectedTab = null;
                ResetTabs();
                _updateTabWindows(-1);
                SheetAreaController.Current.OnTabClose(templateTab);
            }
            else
            {
                _selectedTab = tab;
                ResetTabs();
                tab.background.sprite = tabActive;
                _updateTabWindows(tab.transform.GetSiblingIndex());
            }
        }

        public void ResetTabs()
        {
            foreach (Tab tab in tabs)
            {
                if ((_selectedTab != null && tab == _selectedTab))
                {
                    continue;
                }

                tab.background.sprite = tabIdle;
            }

            if (_selectedTab == null)
            {
                SheetAreaController.Current.OnTabClose(templateTab);
            }
        }

        private void _updateTabWindows(int index = 0)
        {
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                objectsToSwap[i].SetActive(i == index);
            }
        }
    }
}
