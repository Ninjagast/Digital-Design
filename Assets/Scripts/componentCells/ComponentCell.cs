using System.Collections.Generic;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace componentCells
{
    public class ComponentCell: ComponentBaseClass, IComponentCell 
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Dictionary<int, Vector3> _cellsToCheck;
        private int _upperCeiling;
        private int _threshold;
        private int _lastPulse = -999;
        private int _strength = 0; //The number of times this wire has been activated
        private bool _isOn = false;
        private Vector3 _outputPos;
        private ComponentTypes _type = ComponentTypes.Component;
        private int _id;
        
        public ComponentCell(GameObject componentOn, GameObject componentOff, Vector3 position, int upperCeiling, int threshold, Vector3 placementOffset, int id)
        {
            _componentOn = componentOn; 
            _componentOff = componentOff; 
            _upperCeiling = upperCeiling; 
            _threshold = threshold;
            _id = id;
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
            DeActivate(-1, Vector3.zero,true);
        }

        public void Activate(int pulseId, Vector3 pos)
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

        public void DeActivate(int pulseId, Vector3 pos, bool shutdown = false)
        {
            if (!shutdown)
            {
                if(pulseId != _lastPulse)
                {
                    _strength -= 1;
                    if (_upperCeiling != -1 && _strength < _upperCeiling && _strength >= _threshold)
                    {
                        if (!_isOn)
                        {
                            _turnCellOn(pulseId);
                        }
                    }
                    if (_strength < _threshold)
                    {                    
                        if (_isOn)
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

        private void _turnCellOff(int pulseId)
        {
            _lastPulse = pulseId;
//          turn this cell off
            _isOn = false;
            _componentOff.SetActive(true);
            _componentOn.SetActive(false);
            CheckCells(false, _lastPulse, _outputPos, _cellsToCheck);
        }
        
        private void _turnCellOn(int pulseId)
        {
            _lastPulse = pulseId;
//          turn the cell on
            _isOn = true;
            _componentOff.SetActive(false);
            _componentOn.SetActive(true);
            CheckCells(true, _lastPulse, _outputPos, _cellsToCheck);
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

        public new ComponentTypes GetType()
        {
            return _type;
        }

        public KeyValuePair<int, int> GetComponentData()
        {
            return new KeyValuePair<int, int>(_upperCeiling, _threshold);
        }

        public int GetId()
        {
            return _id;
        }
    }
}