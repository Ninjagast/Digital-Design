using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace Creation.Commands
{
    public class AddButtonComponent: Object, ICommand
    {
        private GameObject _componentOn;
        private GameObject _componentOff;
        private KeyValuePair<Vector3, string> _pos;
        private Vector3 _placementOffset;
        private bool _createdComponent = false;
        private ButtonComponent _button;
        private int _buttonId;

        public AddButtonComponent(GameObject componentOn, GameObject componentOff, KeyValuePair<Vector3, string> position, Vector3 placementOffset, int buttonId)
        {
            _componentOn = componentOn;
            _componentOff = componentOff;
            _pos = position;
            _placementOffset = placementOffset;
            _buttonId = buttonId;
        }

        public void Undo()
        {
            _createdComponent = false;
            IComponentCell componentCell = GameManager.Current.Grid[_pos.Key];
            List<GameObject> components = componentCell.GetComponents();
            Destroy(components[0]);
            Destroy(components[1]);
            componentCell.RemoveFromEventListener();
            GameManager.Current.Grid.Remove(_pos.Key);
        }

        public void Redo()
        {
            Execute();
        }

        public void Execute()
        {
            if (!_createdComponent)
            {
                _button =
                    new ButtonComponent(Instantiate(_componentOn), Instantiate(_componentOff), _pos.Key, Vector3.zero);
                GameManager.Current.Grid.Add(_pos.Key, _button);
                _createdComponent = true;
                GameManager.Current.AddButtonComponent(_pos.Key, _button);
            }
            else
            {
                GameManager.Current.Grid.Add(_pos.Key, _button);
                GameManager.Current.AddButtonComponent(_pos.Key, _button);
            }
        }
    }
}