using GlobalScripts;
using UnityEditor.Animations;
using UnityEngine;

namespace Creation.Commands
{
    public class RemoveComponentCommand: Object, ICommand
    {
        private Vector3 _pos;
        private int _componentId;

        public RemoveComponentCommand(Vector3 pos, int componentId)
        {
            _pos = pos;
            _componentId = componentId;
        }

        public void Undo()
        {
            if (!GameManager.Current.Grid.ContainsKey(_pos))
            {
                // GameObject newObject = Instantiate(CreationManager.Current.buildableComponents[_componentId]);
                // newObject.transform.position = _pos;
                // GameManager.Current.Grid.Add(_pos, newObject);
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
                // Destroy(GameManager.Current.Grid[_pos]);
                // GameManager.Current.Grid.Remove(_pos);
            }
        }
    }
}