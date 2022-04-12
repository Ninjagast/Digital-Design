using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;

namespace componentCells
{
    public class NotComponentCell: IComponentCell
    {
        private GameObject _notComponentOn;
        private GameObject _notComponentOff;
        private Dictionary<int, Vector3> _cellsToCheck;
        private int _upperCeiling;
        private int _threshold;
        private int _lastPulse = -999;
        private int _strength = -1; //The number of times this wire has been activated
        private Vector3 _placementOffset;
        private bool _isOn = false;
        private Vector3 _outputPos;
        
        public NotComponentCell(GameObject notComponentOn, GameObject notComponentOff, Vector3 position, int upperCeiling, int threshold, Vector3 placementOffset)
        {
            _notComponentOn = notComponentOn; 
            _notComponentOff = notComponentOff; 
            _upperCeiling = upperCeiling; 
            _threshold = threshold;
            _placementOffset = placementOffset;
            _cellsToCheck = new Dictionary<int, Vector3>
            {
                {1, new Vector3(1f, 0f, 0)},
            };
            
            _notComponentOn.SetActive(false);
            _notComponentOff.transform.position = position + placementOffset;
            _notComponentOn.transform.position = position + placementOffset;
            
            EventManager.Current.ONSimulationStarting += ONSimulationStarting;
            EventManager.Current.ONSimulationStopping += ONSimulationStopping;
        }

        private void ONSimulationStarting()
        {
            GameManager.Current.PulseId += 1;
            Activate(GameManager.Current.PulseId);
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
                if (_strength >= _upperCeiling)
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
                    if (_strength < _upperCeiling && _strength >= _threshold)
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
                _strength = -1;
                _notComponentOff.SetActive(true);
                _notComponentOn.SetActive(false);
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
            _notComponentOff.SetActive(true);
            _notComponentOn.SetActive(false);
            CheckCells(false);
        }
        
        private void _turnCellOn(int pulseId)
        {
            _lastPulse = pulseId;
//          turn the cell on
            _isOn = true;
            _notComponentOff.SetActive(false);
            _notComponentOn.SetActive(true);
            CheckCells(true);
        }
        
        public List<GameObject> GetComponents()
        {
            List<GameObject> returnList = new List<GameObject> {_notComponentOn, _notComponentOff};
            return returnList;
        }

        public void SetOutputPos(Vector3 outputPos)
        {
            _outputPos = outputPos;
        }
        
        public void RemoveFromEventListener()
        {
            EventManager.Current.ONSimulationStarting -= ONSimulationStarting;
            EventManager.Current.ONSimulationStopping -= ONSimulationStopping;
        }
    }
}