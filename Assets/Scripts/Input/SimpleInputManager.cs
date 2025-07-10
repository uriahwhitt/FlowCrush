using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using FlowCrush.Core;

namespace FlowCrush.Input
{
    /// <summary>
    /// Simple input manager that works with both old and new Unity input systems
    /// Provides basic mouse/touch input for testing and development
    /// </summary>
    public class SimpleInputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float maxSwipeTime = 1f;
        [SerializeField] private bool enableDebug = true;
        
        [Header("Events")]
        public UnityEvent<SwipeDirection> OnSwipeDetected;
        public UnityEvent<Vector2> OnTouchStart;
        public UnityEvent<Vector2> OnTouchEnd;
        
        // Input state
        private Vector2 startPosition;
        private Vector2 endPosition;
        private float touchStartTime;
        private bool isTouching = false;
        
        // Camera reference
        private Camera mainCamera;
        
        // Input System components (not used in current implementation)
        // private PlayerInput playerInput;
        // private InputAction touchAction;
        // private InputAction mouseAction;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("SimpleInputManager: No main camera found!");
            }
            
            // Initialize Input System
            InitializeInputSystem();
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void OnDestroy()
        {
            // Clean up any resources if needed
            // Currently using simple Input System access, no cleanup needed
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            // Use Input System compatible methods
            if (Application.isEditor)
            {
                HandleMouseInput();
            }
            else
            {
                HandleTouchInput();
            }
        }
        
        private void InitializeInputSystem()
        {
            // For now, we'll use a simpler approach that works with Input System
            Debug.Log("SimpleInputManager: Using Input System compatible input handling");
        }
        
        private void HandleMouseInput()
        {
            // Use Input System compatible mouse input
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 position = Mouse.current.position.ReadValue();
                StartTouch(position);
            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed && isTouching)
            {
                Vector2 position = Mouse.current.position.ReadValue();
                UpdateTouch(position);
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame && isTouching)
            {
                Vector2 position = Mouse.current.position.ReadValue();
                EndTouch(position);
            }
        }
        
        private void HandleTouchInput()
        {
            // Use Input System compatible touch input
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 position = Touchscreen.current.primaryTouch.position.ReadValue();
                StartTouch(position);
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed && isTouching)
            {
                Vector2 position = Touchscreen.current.primaryTouch.position.ReadValue();
                UpdateTouch(position);
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame && isTouching)
            {
                Vector2 position = Touchscreen.current.primaryTouch.position.ReadValue();
                EndTouch(position);
            }
        }
        
        private void StartTouch(Vector2 screenPosition)
        {
            startPosition = screenPosition;
            endPosition = screenPosition;
            touchStartTime = Time.time;
            isTouching = true;
            
            OnTouchStart?.Invoke(screenPosition);
            
            if (enableDebug)
            {
                Debug.Log($"Touch started at: {screenPosition}");
            }
        }
        
        private void UpdateTouch(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            endPosition = screenPosition;
        }
        
        private void EndTouch(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            endPosition = screenPosition;
            isTouching = false;
            
            OnTouchEnd?.Invoke(screenPosition);
            
            // Detect swipe
            SwipeDirection swipeDirection = DetectSwipe();
            if (swipeDirection != SwipeDirection.None)
            {
                OnSwipeDetected?.Invoke(swipeDirection);
                if (enableDebug)
                {
                    Debug.Log($"Swipe detected: {swipeDirection}");
                }
            }
        }
        
        #endregion
        
        #region Swipe Detection
        
        private SwipeDirection DetectSwipe()
        {
            // Check if swipe time is within limits
            float swipeTime = Time.time - touchStartTime;
            if (swipeTime > maxSwipeTime)
            {
                return SwipeDirection.None;
            }
            
            // Calculate swipe distance
            float swipeDistance = Vector2.Distance(startPosition, endPosition);
            if (swipeDistance < minSwipeDistance)
            {
                return SwipeDirection.None;
            }
            
            // Calculate swipe direction
            Vector2 swipeVector = endPosition - startPosition;
            float angle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
            
            // Normalize angle to 0-360 degrees
            if (angle < 0) angle += 360;
            
            // Determine direction based on angle
            if (angle >= 45 && angle < 135)
            {
                return SwipeDirection.Up;
            }
            else if (angle >= 135 && angle < 225)
            {
                return SwipeDirection.Left;
            }
            else if (angle >= 225 && angle < 315)
            {
                return SwipeDirection.Down;
            }
            else
            {
                return SwipeDirection.Right;
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        public Vector2Int ScreenToGridPosition(Vector2 screenPosition)
        {
            if (mainCamera == null) return Vector2Int.zero;
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -mainCamera.transform.position.z));
            return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
        }
        
        public Vector2 GridToScreenPosition(Vector2Int gridPosition)
        {
            if (mainCamera == null) return Vector2.zero;
            
            Vector3 worldPosition = new Vector3(gridPosition.x, gridPosition.y, -mainCamera.transform.position.z);
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            return new Vector2(screenPosition.x, screenPosition.y);
        }
        
        public void SetSwipeSettings(float minDistance, float maxTime)
        {
            minSwipeDistance = minDistance;
            maxSwipeTime = maxTime;
        }
        
        public bool IsTouching()
        {
            return isTouching;
        }
        
        public Vector2 GetCurrentTouchPosition()
        {
            return endPosition;
        }
        
        #endregion
        
        #region Debug Methods
        
        [ContextMenu("Test Swipe Detection")]
        public void DebugTestSwipeDetection()
        {
            Vector2 testStart = new Vector2(100, 100);
            Vector2 testEnd = new Vector2(200, 100);
            
            startPosition = testStart;
            endPosition = testEnd;
            touchStartTime = Time.time - 0.5f;
            
            SwipeDirection direction = DetectSwipe();
            Debug.Log($"Test swipe: {direction} (Distance: {Vector2.Distance(testStart, testEnd)})");
        }
        
        [ContextMenu("Toggle Debug")]
        public void DebugToggleDebug()
        {
            enableDebug = !enableDebug;
            Debug.Log($"Input debug {(enableDebug ? "enabled" : "disabled")}");
        }
        
        #endregion
    }
} 