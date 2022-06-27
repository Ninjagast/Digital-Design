using System.Collections.Generic;
using System.ComponentModel;
using componentCells;
using componentCells.BaseClasses;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;

namespace Creation.Commands
{
    public class AddWiresCommand : Object, ICommand
    {
        private GameObject _wireOn;
        private GameObject _wireOff;
        private Dictionary<Vector3, GameObject> _blueprintGrid;
        
        public AddWiresCommand(GameObject wireOn, GameObject wireOff, Dictionary<Vector3, GameObject> blueprintGrid)
        {
            _wireOn = wireOn;
            _wireOff = wireOff;
            _blueprintGrid = blueprintGrid;
        }

        public void Undo()
        {
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    IComponentCell wire = GameManager.Current.Grid[bluePrintToBuild.Key];
                    List<GameObject> wires = wire.GetComponents();
                    Destroy(wires[0]);
                    Destroy(wires[1]);
                    wire.RemoveFromEventListener();
                    GameManager.Current.Grid.Remove(bluePrintToBuild.Key);
                    GameManager.Current.RemoveComponent(bluePrintToBuild.Key);
                }
            }
        }

        public void Redo()
        {
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (!GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    WireCell wireCell = new WireCell(Instantiate(_wireOn), Instantiate(_wireOff), bluePrintToBuild.Key);
                    GameManager.Current.Grid.Add(bluePrintToBuild.Key, wireCell);
                    GameManager.Current.AddComponent(bluePrintToBuild.Key, wireCell);
                }
            }
        }

        public void Execute()
        {
            List<Vector3> keysToRemove = new List<Vector3>();
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (!GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    WireCell wireCell = new WireCell(Instantiate(_wireOn), Instantiate(_wireOff), bluePrintToBuild.Key);
                    GameManager.Current.Grid.Add(bluePrintToBuild.Key, wireCell);
                    GameManager.Current.AddComponent(bluePrintToBuild.Key, wireCell);
                }
                else
                {
                    keysToRemove.Add(bluePrintToBuild.Key);
                }
                Destroy(bluePrintToBuild.Value);
            }

            foreach (var key in keysToRemove)
            {
                _blueprintGrid.Remove(key);
            }
        }
    }
}