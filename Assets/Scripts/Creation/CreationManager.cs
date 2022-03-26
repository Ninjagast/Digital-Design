using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreationManager : MonoBehaviour
{
    private static CreationManager _current;
    
    public GameObject SelectedComponent;
    public Camera MainCamera;
    public Tilemap tilemap;
    private Dictionary<Vector3, GameObject> _grid;


    private static CreationManager Current
    {
        get
        {
            if (_current == null)
            {
                Debug.LogError("CreationManager is null");
            }

            return _current;
        }
    }

    private void Awake()
    {
        _current = this;
        _grid = new Dictionary<Vector3, GameObject>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _handleMouseInput();
    }

    private void _handleMouseInput()
    {    
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = tilemap.WorldToCell(MainCamera.ScreenToWorldPoint(Input.mousePosition));
            pos.x += 0.5f;
            pos.y += 0.5f;
            if (!_grid.ContainsKey(pos))
            {
                GameObject NewObject = Instantiate(SelectedComponent);
                NewObject.transform.position = pos;
                _grid.Add(pos, NewObject);
            }
        }
    }
}

