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
        
        public int selectedComponent = 0;
        public List<GameObject> buildableComponents; // 0: Wire | 1:Not gate
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

        private void Start()
        {

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
                if (selectedComponent == 0)
                {
                    Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
                    pos.x += 0.5f;
                    pos.y += 0.5f;
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (GameManager.Current.Grid.ContainsKey(pos))
                        {
                            RemoveComponentCommand command = new RemoveComponentCommand(pos, 0);
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
                    if (Input.GetMouseButton(0) && !_deletedWire)
                    {
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
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (!_deletedWire || _blueprintGrid.Count > 0)
                        {
                            AddWiresCommand command = new AddWiresCommand(buildableComponents[selectedComponent], 
                            new Dictionary<Vector3, GameObject>(_blueprintGrid));
                            
                            GameManager.Current.History.Push(command);
                            _blueprintGrid.Clear();
                        }
                        _deletedWire = false;
                    }
                }
                else
                {
                    
                }
            }
        }
    }
}

