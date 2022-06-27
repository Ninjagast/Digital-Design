using System;
using System.Collections.Generic;
using Creation.Commands;
using Enums;
using GlobalScripts;
using GlobalScripts.Creation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Creation
{
    public class CreationManager : MonoBehaviour
    {
        private static CreationManager _current;
        public static CreationManager Current => _current;
        
        [Header("Selection Options")]
        public int selectedComponent = 0;
        public int selectedWire = 0;
        
        [Header("Buildable Components/wires")]
        public List<GameObject> wires;
        public List<GameObject> components; // 0:Not gate | 2:Or gate | 4:And gate | 6:XOR gate | 8:Toggle Button
        
        [Header("References to gameObjects")]
        public GameObject wireBluePrint;
        public Camera mainCamera;
        public Tilemap tilemap;
        public GameObject wireBlueprintsContainer;
        public List<GameObject> sevenSegmentBulbs;
        
        private Dictionary<Vector3, GameObject> _blueprintGrid;
        private Vector3 _lastCell;
        private bool _deletedWire = false;
        private bool _deleteTailWire = false;
        private bool _closedTabs = false;
        
        private void Awake()
        {
            _current = this;
            _blueprintGrid = new Dictionary<Vector3, GameObject>();
        }

        void Update()
        {
            if (GameManager.Current.appState == AppState.Creation)
            {
                _handleMouseInput();
            }
        }

        private void _handleMouseInput()
        {
            if (GameManager.Current.mouseOverSheet)
            {
//              we have to place a wire
//              Make this based of the SelectedWire
                if (selectedComponent == -1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!_resetTabsIfOpen())
                        {
                            _handleInitialWireMousePress();
                        }
                        else
                        {
                            _closedTabs = true;
                        }
                    }
                    if (Input.GetMouseButton(0) && !_deletedWire && !_closedTabs)
                    {
                        _handleFancyWireMouseDrag();
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
//                      we build the wires in the blueprint if there are any and if we have not deleted a wire
                        if (!_deletedWire || _blueprintGrid.Count > 0 && !_closedTabs)
                        {
                            AddWiresCommand command = new AddWiresCommand(wires[1], wires[0], 
                            new Dictionary<Vector3, GameObject>(_blueprintGrid));
                            
                            GameManager.Current.history.Push(command);
                            _blueprintGrid.Clear();
                        }

                        _closedTabs = false;
                        _deletedWire = false;
                    }
                }
                else
                {
//                  we have to place a component
                    if (Input.GetMouseButtonDown(0) && !_resetTabsIfOpen())
                    {
                        Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
                        pos.x += 0.5f;
                        pos.y += 0.5f;
                        
                        
                        if (selectedComponent == 0) // not gate
                        {
                            Dictionary<Vector3, string> gridComponent = new Dictionary<Vector3, string>();
                            gridComponent.Add(pos, "input");
                            pos.x += 1f;
                            gridComponent.Add(pos, "output");

                            if (_placeInGrid(gridComponent))
                            {
                                AddNotComponent command = new AddNotComponent(components[1], components[0], 
                                    gridComponent, 1, 0, new Vector3(0.50f,0,0));
                                GameManager.Current.history.Push(command);
                            }
                        }
                        else if (selectedComponent == 2) // or gate
                        {
                            Dictionary<Vector3, string> gridComponent = GenerateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[3], components[2],
                                    gridComponent, -1, 1, new Vector3(1, 1, 0), 2);
                                GameManager.Current.history.Push(command);
                            }
                        }
                        else if (selectedComponent == 4) // and gate
                        {
                            Dictionary<Vector3, string> gridComponent = GenerateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[5], components[4],
                                    gridComponent, -1, 2, new Vector3(1, 1, 0), 4);
                                GameManager.Current.history.Push(command);
                            }
                        }
                        else if (selectedComponent == 6) // xor gate
                        {
                            Dictionary<Vector3, string> gridComponent = GenerateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[7], components[6],
                                    gridComponent, 2, 1, new Vector3(1, 1, 0), 6);
                                GameManager.Current.history.Push(command);
                            }
                        }
                        else if (selectedComponent == 8) //toggle Button
                        {
                            KeyValuePair<Vector3, string> gridComponent = new KeyValuePair<Vector3, string>(pos, "");
                            if (_placeInGrid(gridComponent))
                            {
                                AddButtonComponent command = new AddButtonComponent(components[9], components[8],
                                    gridComponent, Vector3.zero, 1);
                                GameManager.Current.history.Push(command);
                            }
                        }
                        else if (selectedComponent == 10)
                        {
                            Dictionary<Vector3, string> gridComponent = GenerateGridComponent(pos, 1);
                            if (_placeInGrid(gridComponent))
                            {
                                AddSevenSegment command = new AddSevenSegment(components[10], sevenSegmentBulbs,
                                    gridComponent, new Vector3(5.5f, 6.0f, 0f), 0);
                                GameManager.Current.history.Push(command);
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<Vector3, string> GenerateGridComponent(Vector3 pos, int id = 0)
        {
            Dictionary<Vector3, string> gridComponent = new Dictionary<Vector3, string>();
            if (id == 0)
            {
                gridComponent.Add(pos, "input");
                gridComponent.Add(pos + new Vector3(1,0,0), "claimed");
                gridComponent.Add(pos + new Vector3(2,0,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,1,0), "claimed");
                gridComponent.Add(pos + new Vector3(1,1,0), "claimed");
                gridComponent.Add(pos + new Vector3(2,1,0), "output");
                gridComponent.Add(pos + new Vector3(0,2,0), "input");
                gridComponent.Add(pos + new Vector3(1,2,0), "claimed");
                gridComponent.Add(pos + new Vector3(2,2,0), "claimed");
                return gridComponent;
            }
            else if (id == 1)
            {
                gridComponent.Add(pos, "a");
                gridComponent.Add(pos + new Vector3(0,1,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,2,0), "b");
                gridComponent.Add(pos + new Vector3(0,3,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,4,0), "c");
                gridComponent.Add(pos + new Vector3(0,5,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,6,0), "d");
                gridComponent.Add(pos + new Vector3(0,7,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,8,0), "e");
                gridComponent.Add(pos + new Vector3(0,9,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,10,0), "f");
                gridComponent.Add(pos + new Vector3(0,11,0), "claimed");
                gridComponent.Add(pos + new Vector3(0,12,0), "g");

                for (int y = 0; y < 13; y++)
                {
                    for (int x = 1; x < 12; x++)
                    {
                        gridComponent.Add(pos + new Vector3(x,y,0), "claimed");
                    }
                }
                return gridComponent;
            }
            else
            {
                throw new Exception($"this grid component: {id} does not exist");
            }
        }
        
        private void _handleInitialWireMousePress()
        {
            Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            pos.x += 0.5f;
            pos.y += 0.5f;
                        
            if (GameManager.Current.Grid.ContainsKey(pos))
            {
                RemoveWireCommand command = new RemoveWireCommand(pos, 0);
                GameManager.Current.history.Push(command);
                _deletedWire = true;
                return;
            }
            if (!_blueprintGrid.ContainsKey(pos))
            {
                GameObject newObject = Instantiate(wireBluePrint, wireBlueprintsContainer.transform, true);
                newObject.transform.position = pos;
                _blueprintGrid.Add(pos, newObject);
                _lastCell = pos;
            }
            else
            {
                Destroy(_blueprintGrid[pos]);
                _blueprintGrid.Remove(pos);
                _lastCell = pos;
            }
        }
        private void _handleFancyWireMouseDrag()
        {
            Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            pos.x += 0.5f;
            pos.y += 0.5f;
                        
            if (pos != _lastCell)
            {
                if (!_blueprintGrid.ContainsKey(pos))
                {
                    GameObject newObject = Instantiate(wireBluePrint, wireBlueprintsContainer.transform, true);
                    newObject.transform.position = pos;
                    _blueprintGrid.Add(pos, newObject);
                    _deleteTailWire = true;
                }
                else
                {
                    if (_deleteTailWire)
                    {
                        _deleteTailWire = false;
                        Destroy(_blueprintGrid[_lastCell]);
                        _blueprintGrid.Remove(_lastCell);
                    }
                    Destroy(_blueprintGrid[pos]);
                    _blueprintGrid.Remove(pos);
                }
                _lastCell = pos;
            }
        }
        private bool _placeInGrid(Dictionary<Vector3, string> gridComponent)
        {
            foreach (var keyValuePair in gridComponent)
            {
                if (GameManager.Current.Grid.ContainsKey(keyValuePair.Key))
                {
                    return false;
                }
            }

            return true;
        }

        private bool _placeInGrid(KeyValuePair<Vector3, string> keyValuePair)
        {
            return !GameManager.Current.Grid.ContainsKey(keyValuePair.Key);
        }
        
        private bool _resetTabsIfOpen()
        {
            if (SheetAreaController.Current.isOpenTab)
            {
                EventManager.Current.ClickSheetArea();
                return true;
            }

            return false;
        }
    }
}

