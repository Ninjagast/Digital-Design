using System;
using System.Collections.Generic;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace componentCells
{
    public class ButtonComponent: ComponentBaseClass, IComponentCell
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Dictionary<int, Vector3> _cellsToCheck;
        private bool _isOn = false;
        private Vector3 _pos;

        public ButtonComponent(GameObject componentOn, GameObject componentOff, Vector3 position, Vector3 placementOffset)
        {
            _componentOn = componentOn;
            _componentOff = componentOff;
            _pos = position;
            
            _cellsToCheck = new Dictionary<int, Vector3>
            {
                {0, new Vector3(-1f, 0f, 0)},
                {1, new Vector3(1f, 0f, 0)},
                {2, new Vector3(0f, -1f, 0)},
                {3, new Vector3(0f, 1f, 0)}
            };
            
            _componentOn.SetActive(false);
            _componentOff.transform.position = position + placementOffset;
            _componentOn.transform.position = position + placementOffset;
            
            EventManager.Current.ONSimulationStopping += ONSimulationStopping;
        }

        public void ONSimulationStopping()
        {
            _componentOff.SetActive(true);
            _componentOn.SetActive(false);
            _isOn = false;
        }

        public void Activate(int pulseId, Vector3 pos)
        {
            if (pulseId == -17)
            {
                OnClick();                
            }
            return;
        }

        public void DeActivate(int pulseId, Vector3 pos, bool shutdown = false)
        {
            return;
        }

        public List<GameObject> GetComponents()
        {
            List<GameObject> returnList = new List<GameObject> {_componentOn, _componentOff};
            return returnList;
        }

        public void RemoveFromEventListener()
        {
            EventManager.Current.ONSimulationStopping -= ONSimulationStopping;
            GameManager.Current.RemoveButtonComponent(_pos);
        }

        public new ComponentTypes GetType()
        {
            return ComponentTypes.Component;
        }

        public KeyValuePair<int, int> GetComponentData()
        {
            return new KeyValuePair<int, int>();
        }

        public int GetId()
        {
            return -1;
        }

        private void OnClick()
        {
            GameManager.Current.pulseId += 1;
            if (_isOn)
            {
                _componentOff.SetActive(true);
                _componentOn.SetActive(false);
                CheckCells(false, GameManager.Current.pulseId, _pos, _cellsToCheck);
            }
            else
            {
                _componentOff.SetActive(false);
                _componentOn.SetActive(true);
                CheckCells(true, GameManager.Current.pulseId, _pos, _cellsToCheck);
            }
            _isOn = !_isOn;
        }
    }
}