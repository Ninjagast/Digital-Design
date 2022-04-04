using System.Collections.Generic;
using System.Security.Cryptography;
using componentCells;
using GlobalScripts;
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
                    List<GameObject> wires = GameManager.Current.Grid[bluePrintToBuild.Key].GetComponents();
                    Destroy(wires[0]);
                    Destroy(wires[1]);
                    GameManager.Current.Grid.Remove(bluePrintToBuild.Key);
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
                }
            }
        }

        public void Execute()
        {
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (!GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    WireCell wireCell = new WireCell(Instantiate(_wireOn), Instantiate(_wireOff), bluePrintToBuild.Key);
                    GameManager.Current.Grid.Add(bluePrintToBuild.Key, wireCell);
                }
                Destroy(bluePrintToBuild.Value);
            }
        }
    }
}