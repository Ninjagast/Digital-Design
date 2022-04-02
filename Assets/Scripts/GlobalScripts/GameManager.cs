using System;
using Creation.Commands;
using Enums;
using UnityEngine;

namespace GlobalScripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _current;
        public static GameManager Current => _current;

        public AppState appState;
        public CommandHistory History;
        public float tickSpeed;
        public bool mouseOverSheet;

        private void Awake()
        {
            _current = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Current.ONSimulationStarting += _OnSimulationStarting;
            EventManager.Current.ONSimulationStopping += _OnSimulationStopping;
            History = new CommandHistory();
            appState = AppState.Creation;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void _OnSimulationStarting()
        {
            appState = AppState.Running;
        }
        
        private void _OnSimulationStopping()
        {
            appState = AppState.Creation;
        }
    }
}
