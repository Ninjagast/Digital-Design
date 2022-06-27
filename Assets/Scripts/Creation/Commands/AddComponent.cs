using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace Creation.Commands
{
    public class AddComponent: Object, ICommand
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private Dictionary<Vector3, string> _gridComponent;
        private int _upperCeiling;
        private int _threshold;
        private Vector3 _placementOffset;
        private bool _createdComponent = false;
        private ComponentCell _componentCell;
        private int _id;

        
        public AddComponent(GameObject componentOn, GameObject componentOff, Dictionary<Vector3, string> gridComponent, int upperCeiling, int threshold, Vector3 placementOffset, int id)
        {
            _componentOn = componentOn;
            _componentOff = componentOff;
            _gridComponent = gridComponent;
            _upperCeiling = upperCeiling;
            _threshold = threshold;
            _placementOffset = placementOffset;
            _id = id;
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
                        destroyedPrefabs = true;
                        GameManager.Current.RemoveComponent(gridComponent.Key);

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
                        _componentCell =
                            new ComponentCell(Instantiate(_componentOn), Instantiate(_componentOff), gridComponent.Key,_upperCeiling, _threshold, _placementOffset, _id);
                        GameManager.Current.Grid.Add(gridComponent.Key, _componentCell);
                        GameManager.Current.AddComponent(gridComponent.Key, _componentCell);
                        _createdComponent = true;
                    }
                    else
                    {
                        GameManager.Current.Grid.Add(gridComponent.Key, _componentCell);
                    }
                }
                else if(gridComponent.Value == "output")
                {
                    _componentCell.SetOutputPos(gridComponent.Key);
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