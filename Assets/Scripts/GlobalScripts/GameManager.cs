using System;
using System.Collections.Generic;
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
        public Dictionary<Vector3, GameObject> Grid;

        private bool _yupppp = true;

        private void Awake()
        {
            _current = this;
            Grid = new Dictionary<Vector3, GameObject>();
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
            if (Input.GetKey(KeyCode.L))
            {
                EventManager.Current.SimulationStarting();
            }
            if (Input.GetKey(KeyCode.P))
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
            else if (appState == AppState.Running)
            {
                // if (_yupppp)
                // {
                //     foreach (var comp in GameManager.Current.Grid)
                //     {
                //         comp.Value.GetComponent<ComponentPrefab>().Activate();
                //         _yupppp = false;
                //         Debug.Log("turned on a wire in the line");
                //         return;
                //     }
                // }
            }
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
