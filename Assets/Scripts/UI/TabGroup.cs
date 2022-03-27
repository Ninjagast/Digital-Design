using System.Collections.Generic;
using UnityEngine;

namespace UI
{
//  uses the observer pattern in order to create an event listener which can update individual tabs as if they were an actual group 
    public class TabGroup : MonoBehaviour
    {
        public List<Tab> tabs;
        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;
        public Tab selectedTab;
        public List<GameObject> objectsToSwap;

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

            if (selectedTab == null || tab != selectedTab)
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
            if (selectedTab == tab)
            {
                selectedTab = null;
                ResetTabs();
                UpdateTabWindows(-1);
            }
            else
            {
                selectedTab = tab;
                ResetTabs();
                tab.background.sprite = tabActive;
                UpdateTabWindows(tab.transform.GetSiblingIndex());
            }
        }

        public void ResetTabs(bool hardReset = false)
        {
            if (hardReset) selectedTab = null;
            
            foreach (Tab tab in tabs)
            {
                if ((selectedTab != null && tab == selectedTab))
                {
                    continue;
                }

                tab.background.sprite = tabIdle;
            }
        }

        private void UpdateTabWindows(int index = 0)
        {
            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                objectsToSwap[i].SetActive(i == index);
            }
        }
    }
}
