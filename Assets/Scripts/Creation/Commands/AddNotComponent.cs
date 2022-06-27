using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace Creation.Commands
{
    public class AddNotComponent: Object, ICommand
    {
        private GameObject _notComponentOn;
        private GameObject _notComponentOff;
        private Dictionary<Vector3, string> _gridComponent;
        private int _upperCeiling;
        private int _threshold;
        private Vector3 _placementOffset;
        private bool _createdComponent = false;
        private NotComponentCell _notComponentCell;
        

        public AddNotComponent(GameObject notComponentOn, GameObject notComponentOff, Dictionary<Vector3, string> gridComponent, int upperCeiling, int threshold, Vector3 placementOffset)
        {
            _notComponentOn = notComponentOn;
            _notComponentOff = notComponentOff;
            _gridComponent = gridComponent;
            _upperCeiling = upperCeiling;
            _threshold = threshold;
            _placementOffset = placementOffset;
        }
        public void Undo()
        {
            _createdComponent = false;
            bool destroyedPrefabs = false;
            foreach (var gridComponent in _gridComponent)
            {
                if (gridComponent.Value == "input")
                {
                    IComponentCell componentCell = GameManager.Current.Grid[gridComponent.Key];
                    if (!destroyedPrefabs)
                    {
                        List<GameObject> components = componentCell.GetComponents();
                        Destroy(components[0]);
                        Destroy(components[1]);
                        componentCell.RemoveFromEventListener();
                        GameManager.Current.RemoveComponent(gridComponent.Key);
                        destroyedPrefabs = true;
                    }
                    GameManager.Current.Grid.Remove(gridComponent.Key);
                }
                else
                {
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
                if (gridComponent.Value == "input")
                {
                    if (!_createdComponent)
                    {
                        _notComponentCell =
                            new NotComponentCell(Instantiate(_notComponentOn), Instantiate(_notComponentOff), gridComponent.Key,_upperCeiling, _threshold, _placementOffset);
                        GameManager.Current.Grid.Add(gridComponent.Key, _notComponentCell);
                        GameManager.Current.AddComponent(gridComponent.Key, _notComponentCell);
                        _createdComponent = true;
                    }
                    else
                    {
                        GameManager.Current.Grid.Add(gridComponent.Key, _notComponentCell);
                    }
                }
                else if(gridComponent.Value == "output")
                {
                    _notComponentCell.SetOutputPos(gridComponent.Key);
                    GameManager.Current.Grid.Add(gridComponent.Key, null);
                }
                else
                {
                    GameManager.Current.Grid.Add(gridComponent.Key, null);
                }
            }
        }
    }
}