using System;
using System.Collections.Generic;
using UnityEngine;

namespace componentCells
{
    public interface IComponentCell
    {
        public void Activate(int pulseId, bool combo = false);
        public void DeActivate(int pulseId, bool combo = false, bool shutdown = false);
        public void CheckCells(Boolean toggleOn);
        public List<GameObject> GetComponents();
    }
}