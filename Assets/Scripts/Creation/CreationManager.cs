using System;
using System.Collections.Generic;
using GlobalScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Creation
{
    public class CreationManager : MonoBehaviour
    {
        private static CreationManager _current;
        public static CreationManager Current => _current;
        
        public int selectedComponent = 0;
        public List<GameObject> buildableComponents; // 0: Wire | 1:Wire blueprint
        public Camera mainCamera;
        public Tilemap tilemap;
        public GameObject wireBlueprintsContainer;
        
        private Dictionary<Vector3, GameObject> _grid;
        private Dictionary<Vector3, GameObject> _blueprintGrid;
        private Vector3 _lastCell;
        private bool _deletedWire = false;
        private bool _simulationRunning = false;
        
        private void Awake()
        {
            _current = this;
            _grid = new Dictionary<Vector3, GameObject>();
            _blueprintGrid = new Dictionary<Vector3, GameObject>();
        }

        private void Start()
        {
            EventManager.Current.ONSimulationStarting += _OnSimulationStarting;
            EventManager.Current.ONSimulationStopping += _OnSimulationStopping;
        }

        void Update()
        {
            if (!_simulationRunning)
            {
                _handleMouseInput();
            }
        }

        private void _OnSimulationStarting()
        {
            _simulationRunning = true;
        }
        
        private void _OnSimulationStopping()
        {
            _simulationRunning = false;
        }
        
        private void _handleMouseInput()
        {
            if (MouseState.Current.mouseOverSheet)
            {
                if (selectedComponent == 0)
                {
                    Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
                    pos.x += 0.5f;
                    pos.y += 0.5f;
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (_grid.ContainsKey(pos))
                        {
                            Destroy(_grid[pos]);
                            _grid.Remove(pos);
                            _deletedWire = true;
                            return;
                        }
                        if (!_blueprintGrid.ContainsKey(pos))
                        {
                            GameObject newObject = Instantiate(buildableComponents[1], wireBlueprintsContainer.transform, true);
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
                    if (Input.GetMouseButton(0) && !_deletedWire)
                    {
                        if (pos != _lastCell)
                        {
                            if (!_blueprintGrid.ContainsKey(pos))
                            {
                                GameObject newObject = Instantiate(buildableComponents[1], wireBlueprintsContainer.transform, true);
                                newObject.transform.position = pos;
                                _blueprintGrid.Add(pos, newObject);
                            }
                            else
                            {
                                Destroy(_blueprintGrid[pos]);
                                _blueprintGrid.Remove(pos);
                            }
                            _lastCell = pos;
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (!_deletedWire)
                        {
                            foreach (var bluePrintToBuild in _blueprintGrid)
                            {
                                if (!_grid.ContainsKey(bluePrintToBuild.Key))
                                {
                                    GameObject newObject = Instantiate(buildableComponents[0]);
                                    newObject.transform.position = bluePrintToBuild.Key;
                                    _grid.Add(bluePrintToBuild.Key, newObject);
                                }
                                Destroy(bluePrintToBuild.Value);
                            }
                            _blueprintGrid.Clear();
                        }

                        _deletedWire = false;
                    }
                }
            }
        }
    }
}

