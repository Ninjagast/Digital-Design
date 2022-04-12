using System;
using System.Collections.Generic;
using UnityEngine;

namespace componentCells
{
    public interface IComponentCell
    {
        public void ONSimulationStopping();
        public void Activate(int pulseId);
        public void DeActivate(int pulseId, bool shutdown = false);
        public void CheckCells(Boolean toggleOn);
        public List<GameObject> GetComponents();
        void RemoveFromEventListener();
    }
}