using System;
using System.Collections.Generic;
using Creation;
using GlobalScripts;
using UnityEngine;

namespace componentCells
{
    public class WireCell: IComponentCell
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Vector3 _gridPos;
        
        private int _lastPulse = -999;
        private Dictionary<int, Vector3> _cellsToCheck;

        private int _strength = 0; //The number of times this wire has been activated
        
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

        public void ONSimulationStopping()
        {
            DeActivate(-1, true);
        }

        public void Activate(int pulseId)
        {
            if (pulseId != _lastPulse)
            {
                _lastPulse = pulseId;
                _strength += 1;
//              turn the cell on
                _componentOff.SetActive(false);
                _componentOn.SetActive(true);
                CheckCells(true);
            }
        }

        public void DeActivate(int pulseId, bool shutdown = false)
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
                        CheckCells(false);
                    }
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

        public void CheckCells(bool toggleOn) //This can ask for a pulseId if the runtime introduces weirdness
        {
            foreach (KeyValuePair<int, Vector3> cellPos in _cellsToCheck)
            {
                Vector3 cellToCheck = _gridPos + cellPos.Value;
                if (GameManager.Current.Grid.ContainsKey(cellToCheck) && GameManager.Current.Grid[cellToCheck] != null)
                {
                    if (toggleOn)
                    {
                        GameManager.Current.Grid[cellToCheck].Activate(_lastPulse);
                    }
                    else
                    {
                        GameManager.Current.Grid[cellToCheck].DeActivate(_lastPulse);
                    }
                }
            }
        }
    }
}