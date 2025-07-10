using UnityEngine;
using UnityEngine.Events;
using FlowCrush.Core;

namespace FlowCrush.Input
{
    /// <summary>
    /// Fallback input manager that uses basic Unity input methods
    /// This should work regardless of Input System settings
    /// </summary>
    public class FallbackInputManager : MonoBehaviour
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
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("FallbackInputManager: No main camera found!");
            }
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            // Simple input handling that should work in any Unity setup
            if (Application.isEditor)
            {
                HandleMouseInput();
            }
            else
            {
                HandleTouchInput();
            }
        }
        
        private void HandleMouseInput()
        {
            // Use basic input methods that should work regardless of Input System
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                StartTouch(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButton(0) && isTouching)
            {
                UpdateTouch(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0) && isTouching)
            {
                EndTouch(UnityEngine.Input.mousePosition);
            }
        }
        
        private void HandleTouchInput()
        {
            // Simple touch handling for mobile
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);
                
                switch (touch.phase)
                {
                    case UnityEngine.TouchPhase.Began:
                        StartTouch(touch.position);
                        break;
                    case UnityEngine.TouchPhase.Moved:
                        UpdateTouch(touch.position);
                        break;
                    case UnityEngine.TouchPhase.Ended:
                    case UnityEngine.TouchPhase.Canceled:
                        EndTouch(touch.position);
                        break;
                }
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
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
            
            // Convert to grid coordinates (this will be handled by GridManager)
            // For now, return a simple conversion
            int gridX = Mathf.FloorToInt(worldPosition.x);
            int gridY = Mathf.FloorToInt(worldPosition.y);
            
            return new Vector2Int(gridX, gridY);
        }
        
        public Vector2 GridToScreenPosition(Vector2Int gridPosition)
        {
            if (mainCamera == null) return Vector2.zero;
            
            Vector3 worldPosition = new Vector3(gridPosition.x, gridPosition.y, 0);
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
            // Simulate a swipe for testing
            startPosition = new Vector2(100, 100);
            endPosition = new Vector2(200, 100);
            touchStartTime = Time.time - 0.5f;
            
            SwipeDirection direction = DetectSwipe();
            Debug.Log($"Test swipe direction: {direction}");
        }
        
        [ContextMenu("Toggle Debug")]
        public void DebugToggleDebug()
        {
            enableDebug = !enableDebug;
            Debug.Log($"Debug mode: {(enableDebug ? "ON" : "OFF")}");
        }
        
        #endregion
    }
} 