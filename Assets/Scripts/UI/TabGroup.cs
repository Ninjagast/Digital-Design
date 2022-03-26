using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.UI
{
    public class TabGroup : MonoBehaviour
    {
        public List<Tab> Tabs;
        public Sprite TabIdle;
        public Sprite TabHover;
        public Sprite TabActive;
        public Tab SelectedTab;
        public List<GameObject> ObjectsToSwap;

        public void Subscribe(Tab tab)
        {
            if (Tabs == null)
            {
                Tabs = new List<Tab>();
            }

            Tabs.Add(tab);
        }

        public void OnTabEnter(Tab tab)
        {
            ResetTabs();

            if (SelectedTab == null || tab != SelectedTab)
            {
                tab.Background.sprite = TabHover;
            }
        }

        public void OnTabExit(Tab tab)
        {
            ResetTabs();

        }

        public void OnTabSelected(Tab tab)
        {
            if (SelectedTab == tab)
            {
                SelectedTab = null;
                ResetTabs();
                UpdateTabWindows(-1);
            }
            else
            {
                SelectedTab = tab;
                ResetTabs();
                tab.Background.sprite = TabActive;
                UpdateTabWindows(tab.transform.GetSiblingIndex());
            }
        }

        public void ResetTabs()
        {
            foreach (Tab tab in Tabs)
            {
                if (SelectedTab != null && tab == SelectedTab) { continue; }

                tab.Background.sprite = TabIdle;
            }
        }

        private void UpdateTabWindows(int index = 0)
        {
            for (int i = 0; i < ObjectsToSwap.Count; i++)
            {
                ObjectsToSwap[i].SetActive(i == index);
            }
        }
    }
}
