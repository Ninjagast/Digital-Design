using System.Collections.Generic;
using System.Security.Cryptography;
using GlobalScripts;
using UnityEngine;

namespace Creation.Commands
{
    public class AddWiresCommand : Object, ICommand
    {
        private GameObject _componentPrefab;
        private Dictionary<Vector3, GameObject> _blueprintGrid;
        
        public AddWiresCommand(GameObject component, Dictionary<Vector3, GameObject> blueprintGrid)
        {
            _componentPrefab = component;
            _blueprintGrid = blueprintGrid;
        }

        public void Undo()
        {
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    Destroy(GameManager.Current.Grid[bluePrintToBuild.Key]);
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
                    GameObject newObject = Instantiate(_componentPrefab);
                    newObject.transform.position = bluePrintToBuild.Key;
                    GameManager.Current.Grid.Add(bluePrintToBuild.Key, newObject);
                }
            }
        }

        public void Execute()
        {
            foreach (var bluePrintToBuild in _blueprintGrid)
            {
                if (!GameManager.Current.Grid.ContainsKey(bluePrintToBuild.Key))
                {
                    GameObject newObject = Instantiate(_componentPrefab);
                    newObject.transform.position = bluePrintToBuild.Key;
                    GameManager.Current.Grid.Add(bluePrintToBuild.Key, newObject);
                }
                Destroy(bluePrintToBuild.Value);
            }
        }
    }
}