using System;
using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;

namespace Creation
{
    public class ComponentPrefab : MonoBehaviour
    {
        public int _activated;
        public GameObject gameObjectOn;
        public GameObject gameObjectOff;
        
        private Dictionary<int, Vector3> _cellsToCheck;

        public ComponentPrefab()
        {
            _cellsToCheck = new Dictionary<int, Vector3>();
            _cellsToCheck.Add(0, new Vector3(-1.5f,0.5f,0));
            _cellsToCheck.Add(1, new Vector3(1.5f,0.5f,0));
            _cellsToCheck.Add(2, new Vector3(0.5f,-1.5f,0));
            _cellsToCheck.Add(3, new Vector3(0.5f,1.5f,0));
        }
        

        public void Activate()
        {
            _activated += 1;

            if (_activated == 1)
            {
                GameObject go = Instantiate(gameObjectOn);
                go.transform.position = this.gameObject.transform.position;
                _checkCells(true);
                Destroy(this.gameObject);
            }
        }

        public void DeActivate()
        {
            _activated -= 1;

            if (_activated == 0)
            {
                Instantiate(gameObjectOff, this.gameObject.transform);
                _checkCells(false);
                Destroy(this.gameObject);
            }
        }

        private void _checkCells(Boolean toggleOn)
        {
            Vector3 pos = CreationManager.Current.tilemap.WorldToCell(this.gameObject.transform.position);

            foreach (KeyValuePair<int, Vector3> cellPos in _cellsToCheck)
            {
                Vector3 cellToCheck = pos + cellPos.Value;
                if (GameManager.Current.Grid.ContainsKey(cellToCheck))
                {
                    GameObject component = GameManager.Current.Grid[cellToCheck];
                    if (toggleOn)
                    {
                        component.GetComponent<ComponentPrefab>().Activate();
                    }
                    else
                    {
                        component.GetComponent<ComponentPrefab>().DeActivate();
                    }
                }
            }
        }
    }
}
