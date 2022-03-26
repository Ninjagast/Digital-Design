using System;
using UnityEngine;

//todo Need to grab hotkeys/speed values from the user options save
namespace UI
{
    public class CameraController : MonoBehaviour
    {
        public float MovementSpeed;
        public float ScrollMovementSpeed;
        public float MovementTime;
        public float ZoomSpeed;
        public float PlaneDistanceZ;
        
        public Camera MainCamera;
        
        private Vector3 _newPosition;
        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;
        
        private Plane _plane;
        private Vector3 _distanceFromCamera;

        void Start()
        {
            Vector3 cameraPos = MainCamera.transform.position;
            
            _newPosition = transform.position;
            _distanceFromCamera = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z - PlaneDistanceZ);
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
                    _newPosition += (transform.up * MovementSpeed);
                }
                else
                {
                    _newPosition += (transform.up * (ScrollMovementSpeed * scrollSpeed));
                }
            }
//          Down | S | Scroll down
            if ((Input.GetKey(KeyCode.S) || (scrollSpeed > 0f && !Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed.Equals(0))
                {
                    _newPosition += (transform.up * -MovementSpeed);
                }
                else
                {
                    _newPosition += (transform.up * (ScrollMovementSpeed * scrollSpeed));
                }
            }
//          Right | D | Scroll down + LeftShift
            if ((Input.GetKey(KeyCode.D) || (scrollSpeed > 0f && Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed == 0)
                {
                    _newPosition += (transform.right * MovementSpeed);
                }
                else
                {
                    _newPosition += -(transform.right * (ScrollMovementSpeed * scrollSpeed));
                }
            }
//          Left | A | Scroll up + LeftShift
            if ((Input.GetKey(KeyCode.A) || (scrollSpeed < 0f && Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
            {
                if (scrollSpeed == 0)
                {
                    _newPosition += (transform.right * -MovementSpeed);
                }
                else
                {
                    _newPosition += -(transform.right * (ScrollMovementSpeed * scrollSpeed));
                }
            }
//          Zoom | Scroll + LeftControl
            if ((scrollSpeed > 0f || scrollSpeed < 0f) && Input.GetKey(KeyCode.LeftControl))
            {
                MainCamera.orthographicSize -= (ZoomSpeed * scrollSpeed);
            }

            transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * MovementTime);
            
        }

        private void _handleMouseInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);

                if (_plane.Raycast(ray, out var entry))
                {
                    _dragStartPosition = ray.GetPoint(entry);
                }
            }
            if (Input.GetMouseButton(1))
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);

                if (_plane.Raycast(ray, out var entry))
                {
                    _dragCurrentPosition = ray.GetPoint(entry);
                }
                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }
}
