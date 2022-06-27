using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using UnityEngine;

namespace IO
{
    
    [System.Serializable]
    public class ComponentData
    {
        public int id;
        public int upperCeiling;
        public int treshold;

        public Vector3 position;
        public ComponentTypes componentType;
    }
}