using System.Collections.Generic;
using componentCells.BaseClasses;
using UnityEngine;

namespace IO
{
    [System.Serializable]
    public enum ComponentType
    {
        Wire,
        NotComponent,
        Component
    }

    
    [System.Serializable]
    public class ComponentData
    {
        public string id;
        public int upperCeiling;
        public int treshold;

        public ComponentType componentType;
        public Vector3 position;
    }
}