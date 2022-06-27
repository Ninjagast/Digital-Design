using System;
using System.Collections.Generic;
using UnityEngine;

namespace componentCells.BaseClasses
{
    public interface IComponentCell
    {
        public void ONSimulationStopping();
        public void Activate(int pulseId, Vector3 position);
        public void DeActivate(int pulseId, Vector3 position, bool shutdown = false);
        public List<GameObject> GetComponents();
        public void RemoveFromEventListener();
        public ComponentTypes GetType();
        public KeyValuePair<int, int> GetComponentData();
        public int GetId();
    }
}