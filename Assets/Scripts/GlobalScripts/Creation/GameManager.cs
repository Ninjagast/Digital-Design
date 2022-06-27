using System;
using System.Collections.Generic;
using componentCells;
using componentCells.BaseClasses;
using Creation;
using Creation.Commands;
using Enums;
using IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GlobalScripts.Creation
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _current;
        public static GameManager Current => _current;

        public GameObject saveCompletedPopUp;
        public GameObject startSimulationPopUp;
        public GameObject stopSimulationPopUp;
        public GameObject saveFailedPopUp;
        [NonSerialized] public AppState appState;
        [NonSerialized] public CommandHistory history;
        [NonSerialized] public float tickSpeed;
        [NonSerialized] public bool mouseOverSheet;
        [NonSerialized] public Dictionary<Vector3, IComponentCell> Grid;
        [NonSerialized] public int pulseId = 0;

        private int _popUpTimer = 0;
        private Dictionary<Vector3, IComponentCell> _buttons = new Dictionary<Vector3, IComponentCell>();
        private Dictionary<Vector3, IComponentCell> _components = new Dictionary<Vector3, IComponentCell>();
        private Camera _camera;
        private Tilemap _tileMap;
        
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
            history = new CommandHistory();
            appState = AppState.Creation;
            _camera = CameraController.Current.mainCamera;
            _tileMap = CreationManager.Current.tilemap;
            LoadProject();
        }

        // Update is called once per frame
        void Update()
        {
            if (_popUpTimer > 0)
            {
                _popUpTimer -= 1;
                if (_popUpTimer == 0)
                {
                    saveCompletedPopUp.SetActive(false);
                    startSimulationPopUp.SetActive(false);
                    stopSimulationPopUp.SetActive(false);
                    saveFailedPopUp.SetActive(false);
                }
            }
            
            _handleKeyboardInput();
            _handleMouseButtonPress();
        }

        private void LoadProject()
        {
            if (SaveData.Current.buttons.Count > 0 )
            {
                foreach (ButtonData button in SaveData.Current.buttons)
                {
                    ButtonComponent insertButton =
                        new ButtonComponent(Instantiate(CreationManager.Current.components[9]), Instantiate(CreationManager.Current.components[8]), button.position, Vector3.zero);
                    Grid.Add(button.position, insertButton);
                    AddButtonComponent(button.position, insertButton);

                }
            }
            
            if (SaveData.Current.components.Count > 0 )
            {
                foreach (ComponentData component in SaveData.Current.components)
                {
                    switch (component.componentType)
                    {
                        case ComponentTypes.Component:
                            Dictionary<Vector3, string> gridComponent = CreationManager.GenerateGridComponent(component.position, 0);
                            
                            AddComponent componentCommand = new AddComponent(CreationManager.Current.components[component.id + 1], CreationManager.Current.components[component.id],
                                gridComponent, component.upperCeiling, component.treshold, new Vector3(1, 1, 0), component.id);
                            componentCommand.Execute();
                            break;
                        
                        case ComponentTypes.Wire:
                            WireCell wireCell = new WireCell(Instantiate(CreationManager.Current.wires[1]), Instantiate(CreationManager.Current.wires[0]), component.position);
                            Current.Grid.Add(component.position, wireCell);
                            break;
                        
                        case ComponentTypes.NotComponent:
                            Dictionary<Vector3, string> gridNotComponent = new Dictionary<Vector3, string>();
                            gridNotComponent.Add(component.position, "input");
                            component.position.x += 1f;
                            gridNotComponent.Add(component.position, "output");

                            AddNotComponent notComponentCommand = new AddNotComponent(CreationManager.Current.components[1], CreationManager.Current.components[0], 
                                gridNotComponent, 1, 0, new Vector3(0.50f,0,0));
                            notComponentCommand.Execute();
                            break;
                        
                        case ComponentTypes.SevenSegment:
                            Dictionary<Vector3, string> gridSevenSegment = CreationManager.GenerateGridComponent(component.position, 1);
                            AddSevenSegment command = new AddSevenSegment(CreationManager.Current.components[10], CreationManager.Current.sevenSegmentBulbs,
                                gridSevenSegment, new Vector3(5.5f, 6.0f, 0f), 0);
                            command.Execute();
                            break;
                    }
                }
            }
        }
        
        private void _handleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.L) && appState != AppState.Running)
            {
                EventManager.Current.SimulationStarting();
                _popUpTimer = 500;
                startSimulationPopUp.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.P) && appState != AppState.Creation)
            {
                EventManager.Current.SimulationStopping();
                _popUpTimer = 500;
                stopSimulationPopUp.SetActive(true);
            }
            
            if (appState == AppState.Creation)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    HandleQuickSave();
                }
                
                if (Input.GetKeyDown(KeyCode.Z) ) //&& Input.GetKey(KeyCode.LeftControl)
                {
                    history.Undo();
                }

                if (Input.GetKeyDown(KeyCode.Y)) // && Input.GetKey(KeyCode.LeftControl)
                {
                    history.Redo();
                }
            }
        }

        private void HandleQuickSave()
        {
            SaveData.Current.buttons = new List<ButtonData>();
            SaveData.Current.components = new List<ComponentData>();

            foreach (var button in _buttons)
            {
                ButtonData inputButton = new ButtonData
                {
                    id = Guid.NewGuid().ToString(), 
                    position = button.Key, 
                    type = 1
                };
                SaveData.Current.buttons.Add(inputButton);
            }

            foreach (var gridComponent in _components)
            {
                //upperCeiling, threshold
                KeyValuePair<int, int> componentData = gridComponent.Value.GetComponentData();

                ComponentData inputComponent = new ComponentData
                {
                    id = gridComponent.Value.GetId(), 
                    upperCeiling = componentData.Key,
                    treshold = componentData.Value,
                    position = gridComponent.Key, 
                    componentType = gridComponent.Value.GetType()
                };
                SaveData.Current.components.Add(inputComponent);
            }

            if (SaveManager.Current.OnSave())
            {
                saveCompletedPopUp.SetActive(true);
                _popUpTimer = 500;
            }
            else
            {
                saveFailedPopUp.SetActive(true);
                _popUpTimer = 500;
                //todo might give the user a bit more info on why saving failed
            }
        }

        private void _handleMouseButtonPress()
        {
            if (appState == AppState.Running)
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
                            button.Value.Activate(-17, pos);
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

        public void AddComponent(Vector3 pos, IComponentCell button)
        {
            _components.Add(pos, button);
        }
        
        public void RemoveComponent(Vector3 pos)
        {
            _components.Remove(pos);
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
