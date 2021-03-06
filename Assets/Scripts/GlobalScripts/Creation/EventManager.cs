using System;
using UnityEngine;

namespace GlobalScripts.Creation
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager _current;
        public static EventManager Current => _current;

        private void Awake()
        {
            _current = this;
        }

        public event Action ONSimulationStarting;
        public void SimulationStarting()
        {
            ONSimulationStarting?.Invoke();
        }

        public event Action ONSimulationStopping;
        public void SimulationStopping()
        {
            ONSimulationStopping?.Invoke();
        }

        public event Action ONClickSheetArea;

        public void ClickSheetArea()
        {
            ONClickSheetArea?.Invoke();
        }
    }
}
