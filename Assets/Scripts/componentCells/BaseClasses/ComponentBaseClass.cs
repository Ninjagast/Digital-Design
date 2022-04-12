using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;

namespace componentCells.BaseClasses
{
    public abstract class ComponentBaseClass
    {
        public void CheckCells(bool toggleOn, int lastPulse, Vector3 outputPos, Dictionary<int, Vector3> cellsToCheck)
        {
            foreach (KeyValuePair<int, Vector3> cellPos in cellsToCheck)
            {
                Vector3 cellToCheck = outputPos + cellPos.Value;
                if (GameManager.Current.Grid.ContainsKey(cellToCheck) && GameManager.Current.Grid[cellToCheck] != null)
                {
                    if (toggleOn)
                    {
                        GameManager.Current.Grid[cellToCheck].Activate(lastPulse);
                    }
                    else
                    {
                        GameManager.Current.Grid[cellToCheck].DeActivate(lastPulse);
                    }
                }
            }
        }
    }
}