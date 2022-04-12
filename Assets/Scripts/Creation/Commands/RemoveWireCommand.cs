using System;
using System.Collections.Generic;
using System.ComponentModel;
using componentCells;
using GlobalScripts;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Creation.Commands
{
    public class RemoveWireCommand: Object, ICommand
    {
        private Vector3 _pos;
        private int _wireId;
        
        public RemoveWireCommand(Vector3 pos, int wireId)
        {
            _pos = pos;
            _wireId = wireId;
        }

        public void Undo()
        {
            if (!GameManager.Current.Grid.ContainsKey(_pos))
            {
                WireCell wireCell = new WireCell(Instantiate(CreationManager.Current.wires[_wireId + 1]),Instantiate(CreationManager.Current.wires[_wireId]),_pos);
                GameManager.Current.Grid.Add(_pos, wireCell);
            }
        }

        public void Redo()
        {
            Execute();
        }

        public void Execute()
        {
            if (GameManager.Current.Grid.ContainsKey(_pos))
            {
                IComponentCell componentCell = GameManager.Current.Grid[_pos];

                List<GameObject> components = componentCell.GetComponents();
                Destroy(components[0]);
                Destroy(components[1]);
                componentCell.RemoveFromEventListener();
                GameManager.Current.Grid.Remove(_pos);
                Console.WriteLine("test");
            }
        }
    }
}