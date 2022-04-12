using System;
using System.Collections.Generic;
using Creation.Commands;
using Enums;
using GlobalScripts;
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
        public List<GameObject> components; // 0:Not gate | 2:Or gate | 4:And gate 
        
        [Header("References to gameObjects")]
        public GameObject wireBluePrint;
        public Camera mainCamera;
        public Tilemap tilemap;
        public GameObject wireBlueprintsContainer;
        
        private Dictionary<Vector3, GameObject> _blueprintGrid;
        private Vector3 _lastCell;
        private bool _deletedWire = false;
        private bool _deleteTailWire = false;
        
        private void Awake()
        {
            _current = this;
            _blueprintGrid = new Dictionary<Vector3, GameObject>();
        }

        void Update()
        {
            if (GameManager.Current.AppState == AppState.Creation)
            {
                _handleMouseInput();
            }
        }

        private void _handleMouseInput()
        {
            if (GameManager.Current.MouseOverSheet)
            {
//              we have to place a wire
//              Make this based of the SelectedWire
                if (selectedComponent == -1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _handleInitialWireMousePress();
                    }
                    if (Input.GetMouseButton(0) && !_deletedWire)
                    {
                        _handleFancyWireMouseDrag();
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
//                      we build the wires in the blueprint if there are any and if we have not deleted a wire
                        if (!_deletedWire || _blueprintGrid.Count > 0)
                        {
                            AddWiresCommand command = new AddWiresCommand(wires[1], wires[0], 
                            new Dictionary<Vector3, GameObject>(_blueprintGrid));
                            
                            GameManager.Current.History.Push(command);
                            _blueprintGrid.Clear();
                        }
                        _deletedWire = false;
                    }
                }
                else
                {
//                  we have to place a component
                    if (Input.GetMouseButtonDown(0))
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
                                AddNotComponent command = new AddNotComponent(components[1], components[0], gridComponent, 1, 0, new Vector3(0.50f,0,0));
                                GameManager.Current.History.Push(command);
                            }
                        }
                        else if (selectedComponent == 2) // or gate
                        {
                            Dictionary<Vector3, string> gridComponent = _generateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[3], components[2],
                                    gridComponent, -1, 1, new Vector3(1, 1, 0));
                                GameManager.Current.History.Push(command);
                            }

                        }
                        else if (selectedComponent == 4) // and gate
                        {
                            Dictionary<Vector3, string> gridComponent = _generateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[5], components[4],
                                    gridComponent, -1, 2, new Vector3(1, 1, 0));
                                GameManager.Current.History.Push(command);
                            }
                        }
                        else if (selectedComponent == 6) // xor gate
                        {
                            Dictionary<Vector3, string> gridComponent = _generateGridComponent(pos);
                            if(_placeInGrid(gridComponent))
                            {
                                AddComponent command = new AddComponent(components[7], components[6],
                                    gridComponent, 2, 1, new Vector3(1, 1, 0));
                                GameManager.Current.History.Push(command);
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<Vector3, string> _generateGridComponent(Vector3 pos, int id = 0)
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
                GameManager.Current.History.Push(command);
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
    }
}

