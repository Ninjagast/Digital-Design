using System.Collections.Generic;

namespace IO
{
    [System.Serializable]
    public class SaveData
    {
        private static SaveData _current;
        private static readonly object Padlock = new object();
        public static SaveData Current
        {
            get
            {
                lock (Padlock)
                {
                    if (_current == null)
                    {
                        _current = new SaveData();
                    }
                    return _current;
                }
            }
        }

        public List<ComponentData> components;
        public List<ButtonData> buttons;

    }
}