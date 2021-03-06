using System;
using System.Collections.Generic;
using componentCells.BaseClasses;
using Creation;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace componentCells
{
    public class WireCell: ComponentBaseClass, IComponentCell
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Vector3 _gridPos;
        
        private int _lastPulse = -999;
        private Dictionary<int, Vector3> _cellsToCheck;

        private int _strength = 0; //The number of times this wire has been activated
        private ComponentTypes _type = ComponentTypes.Wire;
        
        public WireCell(GameObject wireOn, GameObject wireOff, Vector3 gridPos)
        {
            wireOn.transform.position = gridPos;
            wireOff.transform.position = gridPos;
            wireOn.SetActive(false);
            _componentOn = wireOn;
            _componentOff = wireOff; 
            _gridPos = gridPos;
            
            _cellsToCheck = new Dictionary<int, Vector3>
            {
                {0, new Vector3(-1f, 0f, 0)},
                {1, new Vector3(1f, 0f, 0)},
                {2, new Vector3(0f, -1f, 0)},
                {3, new Vector3(0f, 1f, 0)}
            };

            EventManager.Current.ONSimulationStopping += ONSimulationStopping;
        }

        public List<GameObject> GetComponents()
        {
            List<GameObject> returnList = new List<GameObject> {_componentOn, _componentOff};
            return returnList;
        }

        public void RemoveFromEventListener()
        {
            EventManager.Current.ONSimulationStopping -= ONSimulationStopping;
        }

        public new ComponentTypes GetType()
        {
            return _type;
        }

        public KeyValuePair<int, int> GetComponentData()
        {
            return new KeyValuePair<int, int>();
        }

        public int GetId()
        {
            return -1;
        }

        public void ONSimulationStopping()
        {
            DeActivate(-1, Vector3.zero,true);
        }

        public void Activate(int pulseId, Vector3 pos)
        {
            if (pulseId != _lastPulse)
            {
                _lastPulse = pulseId;
                _strength += 1;
//              turn the cell on
                _componentOff.SetActive(false);
                _componentOn.SetActive(true);
                CheckCells(true, _lastPulse, _gridPos, _cellsToCheck);
            }
        }

        public void DeActivate(int pulseId, Vector3 pos, bool shutdown = false)
        {
            if (!shutdown)
            {
                if (pulseId != _lastPulse)
                {
                    _lastPulse = pulseId;
                    _strength -= 1;

                    if (_strength < 1) // if less than one output is powering this wire turn off
                    {
//                      turn the cell off
                        _componentOff.SetActive(true);
                        _componentOn.SetActive(false);
                    }
                    CheckCells(false, _lastPulse, _gridPos, _cellsToCheck);
                }
            }
            else
            {
//              hard reset this cell
                _lastPulse = -999;
                _strength = 0;
                _componentOff.SetActive(true);
                _componentOn.SetActive(false);
            }

        }
    }
}