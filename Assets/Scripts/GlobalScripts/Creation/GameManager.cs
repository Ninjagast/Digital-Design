using System;
using System.Collections.Generic;
using componentCells.BaseClasses;
using Creation;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GlobalScripts.Creation
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
        
        [NonSerialized] private Dictionary<Vector3, IComponentCell> _buttons = new Dictionary<Vector3, IComponentCell>();
        [NonSerialized] private Camera _camera;
        [NonSerialized] private Tilemap _tileMap;
        
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
            _camera = CameraController.Current.mainCamera;
            _tileMap = CreationManager.Current.tilemap;
        }

        // Update is called once per frame
        void Update()
        {
            _handleKeyboardInput();
            _handleMouseButtonPress();
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

        private void _handleMouseButtonPress()
        {
            if (AppState == AppState.Running)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = _tileMap.WorldToCell(_camera.ScreenToWorldPoint(Input.mousePosition));
                    pos.y += .5f;
                    pos.x += .5f;
                    foreach (KeyValuePair<Vector3, IComponentCell> button in _buttons)
                    {
                        if (button.Key == pos)
                        {
                            button.Value.Activate(-17);
                        }
                    }
                }
            }
        }

        public void AddButtonComponent(Vector3 pos, IComponentCell button)
        {
            _buttons.Add(pos, button);
        }
        
        public void RemoveButtonComponent(Vector3 pos)
        {
            _buttons.Remove(pos);
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
