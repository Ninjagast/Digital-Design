using System.Collections.Generic;
using componentCells.BaseClasses;
using GlobalScripts.Creation;
using UnityEngine;

namespace componentCells
{
    public class SevenSegment: ComponentBaseClass, IComponentCell 
    {
        private GameObject _component;
        private Dictionary<string, GameObject> _segments = new Dictionary<string, GameObject>();
        private Dictionary<string, int> _segmentPower = new Dictionary<string, int>();
        private Dictionary<string, Vector3> _inputCells;

        private Vector3 _gridPos;
        private int _lastPulse = -999;
        private ComponentTypes _type = ComponentTypes.SevenSegment;

        public SevenSegment(GameObject component, List<GameObject> segments, Vector3 gridPos, Vector3 placementOffset)
        {
            _component = component;
            _gridPos = gridPos;
            string[] letters = new []{"a", "b", "c", "d", "e", "f", "g"};

            int i = 0;
            foreach (var gameObject in segments)
            {
                _segments.Add(letters[i], gameObject);
                i++;
            }

            _inputCells = new Dictionary<string, Vector3>()
            {
                {"a", new Vector3(0f, 0f, 0)},
                {"b", new Vector3(0f, 2f, 0)},
                {"c", new Vector3(0f, 4f, 0)},
                {"d", new Vector3(0f, 6f, 0)},
                {"e", new Vector3(0f, 8f, 0)},
                {"f", new Vector3(0f, 10f, 0)},
                {"g", new Vector3(0f, 12f, 0)}
            };

            _component.transform.position = gridPos + placementOffset;

            foreach (var segment in _segments)
            {
                _segmentPower.Add(segment.Key, 0);
                segment.Value.transform.position = gridPos + placementOffset + new Vector3(0,0, -1f);
                segment.Value.SetActive(false);
            }
            
            EventManager.Current.ONSimulationStopping += ONSimulationStopping;
            
        }

        public void ONSimulationStopping()
        {
            _segmentPower = new Dictionary<string, int>();
            
            foreach (var segment in _segments)
            {
                _segmentPower.Add(segment.Key, 0);
                segment.Value.SetActive(false);
            }
        }

        public void Activate(int pulseId, Vector3 position)
        {
            _lastPulse = pulseId;
            string segmentToTurnOn = "nope";

            foreach (var segmentLocation in _inputCells)
            {
                if (segmentLocation.Value + _gridPos == position)
                {
                    segmentToTurnOn = segmentLocation.Key;
                    break;
                }
            }

            if (segmentToTurnOn != "nope")
            {
                _segmentPower[segmentToTurnOn] += 1;
                if (_segmentPower[segmentToTurnOn] == 1)
                {
                    _segments[segmentToTurnOn].SetActive(true);
                }
            }
        }

        public void DeActivate(int pulseId, Vector3 position, bool shutdown = false)
        {
            _lastPulse = pulseId;
            string segmentToTurnOff = "nope";

            foreach (var segmentLocation in _inputCells)
            {
                if (segmentLocation.Value + _gridPos == position)
                {
                    segmentToTurnOff = segmentLocation.Key;
                    break;
                }
            }

            if (segmentToTurnOff != "nope")
            {
                _segmentPower[segmentToTurnOff] -= 1;
                if (_segmentPower[segmentToTurnOff] == 0)
                {
                    _segments[segmentToTurnOff].SetActive(false);
                }
            }
        }

        
        public List<GameObject> GetComponents()
        {
            List<GameObject> returnList = new List<GameObject>();
            returnList.Add(_component);

            foreach (var segment in _segments)
            {
                returnList.Add(segment.Value);
            }
            
            return returnList;
        }

        public void RemoveFromEventListener()
        {
            EventManager.Current.ONSimulationStopping -= ONSimulationStopping;
        }

        public ComponentTypes GetType()
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
    }
}