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

        
        [NonSerialized] public AppState AppState;
        [NonSerialized] public CommandHistory History;
        [NonSerialized] public float TickSpeed;
        [NonSerialized] public bool MouseOverSheet;
        [NonSerialized] public Dictionary<Vector3, IComponentCell> Grid;
        [NonSerialized] public int PulseId = 0;
        
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
            AppState = AppState.Creation;
        }

        // Update is called once per frame
        void Update()
        {
            _handleKeyboardInput();
        }

        private void _handleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.L) && AppState != AppState.Running)
            {
                EventManager.Current.SimulationStarting();
            }
            if (Input.GetKeyDown(KeyCode.P) && AppState != AppState.Creation)
            {
                EventManager.Current.SimulationStopping();
            }
            
            if (AppState == AppState.Creation)
            {
                if (Input.GetKeyDown(KeyCode.Z) ) //&& Input.GetKey(KeyCode.LeftControl)
                {
                    History.Undo();
                }

                if (Input.GetKeyDown(KeyCode.Y)) // && Input.GetKey(KeyCode.LeftControl)
                {
                    History.Redo();
                }
            }
        }
        
        private void _OnSimulationStarting()
        {
            AppState = AppState.Running;
        }
        
        private void _OnSimulationStopping()
        {
            AppState = AppState.Creation;
        }
    }
}
