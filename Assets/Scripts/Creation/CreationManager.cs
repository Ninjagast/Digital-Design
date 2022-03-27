using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Creation
{
    public class CreationManager : MonoBehaviour
    {
        private static CreationManager _current;
        public static CreationManager Current => _current;
        
        public GameObject selectedComponent;
        public Camera mainCamera;
        public Tilemap tilemap;
        
        private Dictionary<Vector3, GameObject> _grid;

        private void Awake()
        {
            _current = this;
            _grid = new Dictionary<Vector3, GameObject>();
        }
    
        void Update()
        {
            _handleMouseInput();
        }

        private void _handleMouseInput()
        {
            if (MouseState.Current.mouseOverSheet)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = tilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
                    pos.x += 0.5f;
                    pos.y += 0.5f;
                    if (!_grid.ContainsKey(pos))
                    {
                        GameObject NewObject = Instantiate(selectedComponent);
                        NewObject.transform.position = pos;
                        _grid.Add(pos, NewObject);
                    }
                }
            }
        }
    }
}

