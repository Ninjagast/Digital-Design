using System.Collections.Generic;

namespace IO
{
    [System.Serializable]
    public class SavedData
    {
        public List<ComponentData> components;
        public List<ButtonData> buttons;
    }
}