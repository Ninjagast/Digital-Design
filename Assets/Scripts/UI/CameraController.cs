using System;
using UnityEngine;
using UnityEngine.EventSystems;

//todo Need to grab hotkeys/speed values from the user options save
namespace UI
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController _current;
        public static CameraController Current => _current;

        public float movementSpeed;
        public float scrollMovementSpeed;
        public float movementTime;
        public float zoomSpeed;
        public float planeDistanceZ;

        public Camera mainCamera;
        
        private Vector3 _newPosition;
        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;
        private Vector3 _distanceFromCamera;
        
        private Plane _plane;

        private void Awake()
        {
            _current = this;
        }

        void Start()
        {
            Vector3 cameraPos = mainCamera.transform.position;
            
            _newPosition = transform.position;
            _distanceFromCamera = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z - planeDistanceZ);
            _plane = new Plane(Vector3.forward, _distanceFromCamera);
        }

        void Update()
        {
            _handleMouseInput();
            _handleKeyboardInput();
        }

        private void _handleKeyboardInput()
        {
            float scrollSpeed = Input.GetAxis("Mouse ScrollWheel");
            
//          directional movement
//          Up | W | Scroll up
            if ((Input.GetKey(KeyCode.W) || (scrollSpeed < 0f && !Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed == 0)
                {
                    _newPosition += (transform.up * movementSpeed);
                }
                else
                {
                    _newPosition += (transform.up * (scrollMovementSpeed * scrollSpeed));
                }
            }
//          Down | S | Scroll down
            if ((Input.GetKey(KeyCode.S) || (scrollSpeed > 0f && !Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed.Equals(0))
                {
                    _newPosition += (transform.up * -movementSpeed);
                }
                else
                {
                    _newPosition += (transform.up * (scrollMovementSpeed * scrollSpeed));
                }
            }
//          Right | D | Scroll down + LeftShift
            if ((Input.GetKey(KeyCode.D) || (scrollSpeed > 0f && Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed == 0)
                {
                    _newPosition += (transform.right * movementSpeed);
                }
                else
                {
                    _newPosition += -(transform.right * (scrollMovementSpeed * scrollSpeed));
                }
            }
//          Left | A | Scroll up + LeftShift
            if ((Input.GetKey(KeyCode.A) || (scrollSpeed < 0f && Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed == 0)
                {
                    _newPosition += (transform.right * -movementSpeed);
                }
                else
                {
                    _newPosition += -(transform.right * (scrollMovementSpeed * scrollSpeed));
                }
            }
//          Zoom | Scroll + LeftControl
            if ((scrollSpeed > 0f || scrollSpeed < 0f) && Input.GetKey(KeyCode.LeftControl))
            {
                mainCamera.orthographicSize -= (zoomSpeed * scrollSpeed);
            }

            transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementTime);
            
        }

        private void _handleMouseInput()
        {
            if (MouseState.Current.mouseOverSheet)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (_plane.Raycast(ray, out var entry))
                    {
                        _dragStartPosition = ray.GetPoint(entry);
                    }
                }
                if (Input.GetMouseButton(1))
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (_plane.Raycast(ray, out var entry))
                    {
                        _dragCurrentPosition = ray.GetPoint(entry);
                    }
                    _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
                }   
            }
        }
    }
}
