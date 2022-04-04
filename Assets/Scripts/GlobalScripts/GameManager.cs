using System;
using System.Collections.Generic;
using componentCells;
using Creation;
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
        public Dictionary<Vector3, IComponentCell> Grid;
        public int pulseId = 0;
        
        private void Awake()
        {
            _current = this;
            Grid = new Dictionary<Vector3, IComponentCell>();
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
//          Todo Remove this function from this place
            if (Input.GetKeyDown(KeyCode.L) && appState != AppState.Running)
            {
                EventManager.Current.SimulationStarting();
            }
            if (Input.GetKeyDown(KeyCode.P) && appState != AppState.Creation)
            {
                EventManager.Current.SimulationStopping();
            }
            
            if (appState == AppState.Creation)
            {
                if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
                {
                    History.Undo();
                }

                if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
                {
                    History.Redo();
                }
            }
        }
        
        private void _OnSimulationStarting()
        {
            appState = AppState.Running;
            foreach (var comp in Current.Grid)
            {
                comp.Value.Activate(-9876);
                break;
            }
        }
        
        private void _OnSimulationStopping()
        {
            appState = AppState.Creation;
        }
    }
}
