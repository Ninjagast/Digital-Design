﻿using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;

namespace componentCells
{
    public class ComponentCell: IComponentCell
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Dictionary<int, Vector3> _cellsToCheck;
        private int _upperCeiling;
        private int _threshold;
        private int _lastPulse = -999;
        private int _strength = 0; //The number of times this wire has been activated
        private Vector3 _placementOffset;
        private bool _isOn = false;
        private Vector3 _outputPos;
        
        public ComponentCell(GameObject componentOn, GameObject componentOff, Vector3 position, int upperCeiling, int threshold, Vector3 placementOffset)
        {
            _componentOn = componentOn; 
            _componentOff = componentOff; 
            _upperCeiling = upperCeiling; 
            _threshold = threshold;
            _placementOffset = placementOffset;
            _cellsToCheck = new Dictionary<int, Vector3>
            {
                {1, new Vector3(1f, 0f, 0)},
            };
            
            _componentOn.SetActive(false);
            _componentOff.transform.position = position + placementOffset;
            _componentOn.transform.position = position + placementOffset;
            
            EventManager.Current.ONSimulationStopping += ONSimulationStopping;
        }
        
        public void ONSimulationStopping()
        {
            DeActivate(-1, true);
        }

        public void Activate(int pulseId)
        {
            if (pulseId != _lastPulse)
            {
                _strength += 1;
                if (_upperCeiling != -1 && _strength >= _upperCeiling)
                {
                    if (_isOn)
                    {
                        _turnCellOff(pulseId);
                    }
                }
                else if (_strength >= _threshold)
                {
                    if (!_isOn)
                    {
                        _turnCellOn(pulseId);
                    }
                }
            }
        }

        public void DeActivate(int pulseId, bool shutdown = false)
        {
            if (!shutdown)
            {
                if(pulseId != _lastPulse)
                {
                    _strength -= 1;
                    if (_upperCeiling != -1 && _strength < _upperCeiling && _strength >= _threshold)
                    {
                        if (_isOn)
                        {
                            _turnCellOn(pulseId);
                        }
                    }
                    if (_strength < _threshold)
                    {                    
                        if (!_isOn)
                        {
                            _turnCellOff(pulseId);
                        }
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
                _isOn = false;
            }
        }

        public void CheckCells(bool toggleOn)
        {
            foreach (KeyValuePair<int, Vector3> cellPos in _cellsToCheck)
            {
                Vector3 cellToCheck = _outputPos + cellPos.Value;
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
        
        private void _turnCellOff(int pulseId)
        {
            _lastPulse = pulseId;
//          turn this cell off
            _isOn = false;
            _componentOff.SetActive(true);
            _componentOn.SetActive(false);
            CheckCells(false);
        }
        
        private void _turnCellOn(int pulseId)
        {
            _lastPulse = pulseId;
//          turn the cell on
            _isOn = true;
            _componentOff.SetActive(false);
            _componentOn.SetActive(true);
            CheckCells(true);
        }

        public List<GameObject> GetComponents()
        {
            List<GameObject> returnList = new List<GameObject> {_componentOn, _componentOff};
            return returnList;
        }


        public void SetOutputPos(Vector3 outputPos)
        {
            _outputPos = outputPos;
        }
        
        public void RemoveFromEventListener()
        {
            EventManager.Current.ONSimulationStopping -= ONSimulationStopping;
        }
    }
}