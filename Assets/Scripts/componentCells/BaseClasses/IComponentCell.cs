using System;
using System.Collections.Generic;
using UnityEngine;

namespace componentCells.BaseClasses
{
    public interface IComponentCell
    {
        public void ONSimulationStopping();
        public void Activate(int pulseId);
        public void DeActivate(int pulseId, bool shutdown = false);
        public List<GameObject> GetComponents();
        public void RemoveFromEventListener();
    }
}