namespace Creation.Commands
{
    public interface ICommand
    {
        public void Undo();
        public void Redo();
        public void Execute();
    }
}