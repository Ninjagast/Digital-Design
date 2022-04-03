using System;
using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;

namespace Creation
{
    public class ComponentPrefab : MonoBehaviour
    {
        public int _strength = 0; //The number of times this wire has been activated
        public GameObject gameObjectOn;
        public GameObject gameObjectOff;
        public Vector2 size; //The size of the component in the grid
        public int threshold; //How many wires need to power this component up before it activates
        public int upperCeiling; //The component turns off if the number of active wires exceed this
        
        private Dictionary<int, Vector3> _cellsToCheck;

        public ComponentPrefab()
        {
            _cellsToCheck = new Dictionary<int, Vector3>
            {
                {0, new Vector3(-1.5f, 0.5f, 0)},
                {1, new Vector3(1.5f, 0.5f, 0)},
                {2, new Vector3(0.5f, -1.5f, 0)},
                {3, new Vector3(0.5f, 1.5f, 0)}
            };
        }
        

        public void Activate()
        {
            _strength += 1;
            if (_strength > upperCeiling)
            {
                _strength += 1;
                this.DeActivate();
            }
            else if (_strength >= threshold)
            {
                Instantiate(gameObjectOn, this.gameObject.transform);
                _checkCells(true);
                Destroy(this.gameObject);
            }
        }

        public void DeActivate()
        {
            _strength -= 1;
            if (_strength <= upperCeiling)
            {
                _strength -= 1;
                this.Activate();
            }
            else if (_strength < threshold)
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
