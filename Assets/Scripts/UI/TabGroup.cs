using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        public List<Tab> tabs;
        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;
        public Tab selectedTab;
        public List<GameObject> objectsToSwap;

        public void Subscribe(Tab tab)
        {
            if (tabs == null)
            {
                tabs = new List<Tab>();
            }

            tabs.Add(tab);
        }

        public void OnTabEnter(Tab tab)
        {
            ResetTabs();

            if (selectedTab == null || tab != selectedTab)
            {
                tab.background.sprite = tabHover;
            }
        }

        public void OnTabExit(Tab tab)
        {
            ResetTabs();

        }

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

        public void ResetTabs()
        {
            foreach (Tab tab in tabs)
            {
                if (selectedTab != null && tab == selectedTab) { continue; }

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
