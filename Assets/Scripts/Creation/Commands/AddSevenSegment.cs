using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using GlobalScripts.Creation;
using UnityEngine;

namespace Creation.Commands
{
    public class AddSevenSegment: Object, ICommand
    {
        private GameObject _component;
        private List<GameObject> _segments;
        private List<GameObject> _instantiatedSegments = new List<GameObject>();
        private Dictionary<Vector3, string> _gridComponent;
        private Vector3 _placementOffset;
        private bool _createdComponent = false;
        private SevenSegment _sevenSegment;
        private int _id;
        
        public AddSevenSegment(GameObject component, List<GameObject> segments, Dictionary<Vector3, string> gridComponent, Vector3 placementOffset, int id)
        {
            _component = component;
            _segments = segments;
            _gridComponent = gridComponent;
            _placementOffset = placementOffset;
            _id = id;
        }
        public void Undo()
        {
            _createdComponent = false;
            bool destroyedPrefabs = false;
            foreach (var gridComponent in _gridComponent)
            {
                if (gridComponent.Value == "claimed")
                {
                    GameManager.Current.Grid.Remove(gridComponent.Key);
                }
                else
                {
                    IComponentCell componentCell = GameManager.Current.Grid[gridComponent.Key];
                    if (!destroyedPrefabs)
                    {
                        List<GameObject> components = componentCell.GetComponents();
                        foreach (var gameObject in components)
                        {
                            Destroy(gameObject);
                        }

                        _instantiatedSegments = new List<GameObject>();
                        componentCell.RemoveFromEventListener();
                        destroyedPrefabs = true;
                        GameManager.Current.RemoveComponent(gridComponent.Key);

                    }
                    GameManager.Current.Grid.Remove(gridComponent.Key);
                }
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Execute()
        {
            foreach (var gridComponent in _gridComponent)
            {
                if (gridComponent.Value == "claimed")
                {
                    GameManager.Current.Grid.Add(gridComponent.Key, null);
                }
                else
                {
                    if (!_createdComponent)
                    {
                        foreach (var gameObject in _segments)
                        {
                            _instantiatedSegments.Add(Instantiate(gameObject));
                        }                        
                        
                        
                        _sevenSegment =
                            new SevenSegment(Instantiate(_component), _instantiatedSegments, gridComponent.Key, _placementOffset);
                        
                        GameManager.Current.Grid.Add(gridComponent.Key, _sevenSegment);
                        GameManager.Current.AddComponent(gridComponent.Key, _sevenSegment);
                        _createdComponent = true;
                    }
                    else
                    {
                        GameManager.Current.Grid.Add(gridComponent.Key, _sevenSegment);
                    }
                }
            }
        }
    }
}