using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Creation.Commands
{
    public class CommandHistory: Object
    {
        private List<ICommand> _history;
        private List<ICommand> _undoList;

        public CommandHistory()
        {
            _history = new List<ICommand>();
            _undoList = new List<ICommand>();
        }

        public void Push(ICommand c)
        {
            c.Execute();
            _history.Add(c);
            _undoList.Clear();
        }

        public void Undo()
        {
            if (_history.Count > 0)
            {
                ICommand returnCommand = _history.Last();
                _history.RemoveAt(_history.Count - 1);
                returnCommand.Undo();
                _undoList.Add(returnCommand); //Save the command since we might want to redo the undo
            }
        }

        public void Redo()
        {
            if (_undoList.Count > 0)
            {
                ICommand lastUndoCommand = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                lastUndoCommand.Redo(); //Basically the same as an Execute
                _history.Add(lastUndoCommand);
            }
        }
    }
}