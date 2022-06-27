using UnityEngine;

//todo Need to grab hotkeys/speed values from the user options save
namespace GlobalScripts.Creation
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController _current;
        public static CameraController Current => _current;
        
        [Header("Camera options")]
        public float movementSpeed;
        public float scrollMovementSpeed;
        public float zoomSpeed;

        [Header("Reference to camera")]
        public Camera mainCamera;
        
        private float _movementTime = 6;
        private float _planeDistanceZ = -40;
        private Vector3 _newPosition;
        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;
        private Vector3 _distanceFromCamera;
        
        private Plane _plane;

        private void Awake()
        {
            _current = this;
            Vector3 cameraPos = mainCamera.transform.position;
            _newPosition = transform.position;
            _distanceFromCamera = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z - _planeDistanceZ);
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
            if (((Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.LeftControl)) || (scrollSpeed > 0f && !Input.GetKey(KeyCode.LeftShift))) && !Input.GetKey(KeyCode.LeftControl))
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
                if (mainCamera.orthographicSize < 0) //fixes weird 0 size camera or negative camera size shenanigans
                {
                    mainCamera.orthographicSize = 0.1f;
                }
            }
            transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementTime);
        }

        private void _handleMouseInput()
        {
            if (GameManager.Current.MouseOverSheet)
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