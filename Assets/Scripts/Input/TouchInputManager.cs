using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using FlowCrush.Core;

namespace FlowCrush.Input
{
    /// <summary>
    /// Handles touch input and swipe detection for mobile devices
    /// Provides responsive and accurate swipe direction detection
    /// </summary>
    public class TouchInputManager : MonoBehaviour
    {
        [Header("Swipe Detection Settings")]
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float maxSwipeTime = 1f;
        [SerializeField] private float swipeSensitivity = 1f;
        
        [Header("Touch Settings")]
        [SerializeField] public bool enableTouchDebug = true;
        [SerializeField] private LayerMask touchableLayers = -1;
        
        [Header("Visual Feedback")]
        [SerializeField] private GameObject touchIndicatorPrefab;
        [SerializeField] private GameObject swipeTrailPrefab;
        
        // Events
        public UnityEvent<SwipeDirection> OnSwipeDetected;
        public UnityEvent<Vector2> OnTouchStart;
        public UnityEvent<Vector2> OnTouchEnd;
        public UnityEvent<Vector2> OnTouchMove;
        
        // Touch state
        private Vector2 startTouchPosition;
        private Vector2 endTouchPosition;
        private float touchStartTime;
        private bool isTouching = false;
        
        // Visual feedback
        private GameObject currentTouchIndicator;
        private GameObject currentSwipeTrail;
        
        // Camera reference
        private Camera mainCamera;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("TouchInputManager: No main camera found!");
            }
        }
        
        private void Update()
        {
            HandleTouchInput();
        }
        
        #endregion
        
        #region Touch Input Handling
        
        private void HandleTouchInput()
        {
            // Handle mouse input for testing in editor
            if (Application.isEditor)
            {
                HandleMouseInput();
            }
            
            // Handle touch input for mobile using new Input System
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                HandleTouchPosition(touchPosition);
            }
            else
            {
                // Fallback to old input system if new system is not available
                try
                {
                    if (UnityEngine.Input.touchCount > 0)
                    {
                        Touch touch = UnityEngine.Input.GetTouch(0);
                        HandleTouch(touch);
                    }
                }
                catch (System.InvalidOperationException)
                {
                    // New Input System is active, ignore old input calls
                }
            }
        }
        
        private void HandleMouseInput()
        {
            // Try new Input System first
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    StartTouch(mousePos);
                }
                else if (Mouse.current.leftButton.isPressed && isTouching)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    UpdateTouch(mousePos);
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame && isTouching)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    EndTouch(mousePos);
                }
            }
            else
            {
                // Fallback to old input system
                try
                {
                    if (UnityEngine.Input.GetMouseButtonDown(0))
                    {
                        Vector2 mousePos = UnityEngine.Input.mousePosition;
                        StartTouch(mousePos);
                    }
                    else if (UnityEngine.Input.GetMouseButton(0) && isTouching)
                    {
                        Vector2 mousePos = UnityEngine.Input.mousePosition;
                        UpdateTouch(mousePos);
                    }
                    else if (UnityEngine.Input.GetMouseButtonUp(0) && isTouching)
                    {
                        Vector2 mousePos = UnityEngine.Input.mousePosition;
                        EndTouch(mousePos);
                    }
                }
                catch (System.InvalidOperationException)
                {
                    // New Input System is active, ignore old input calls
                }
            }
        }
        
        private void HandleTouchPosition(Vector2 touchPosition)
        {
            // For the new Input System, we'll handle touch as a simple press/release
            if (!isTouching)
            {
                StartTouch(touchPosition);
            }
            else
            {
                UpdateTouch(touchPosition);
            }
        }
        
        private void HandleTouch(Touch touch)
        {
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
        
        private void StartTouch(Vector2 screenPosition)
        {
            startTouchPosition = screenPosition;
            endTouchPosition = screenPosition;
            touchStartTime = Time.time;
            isTouching = true;
            
            OnTouchStart?.Invoke(screenPosition);
            
            if (enableTouchDebug)
            {
                CreateTouchIndicator(screenPosition);
            }
            
            Debug.Log($"Touch started at: {screenPosition}");
        }
        
        private void UpdateTouch(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            endTouchPosition = screenPosition;
            OnTouchMove?.Invoke(screenPosition);
            
            if (enableTouchDebug && currentSwipeTrail != null)
            {
                UpdateSwipeTrail(screenPosition);
            }
        }
        
        private void EndTouch(Vector2 screenPosition)
        {
            if (!isTouching) return;
            
            endTouchPosition = screenPosition;
            isTouching = false;
            
            OnTouchEnd?.Invoke(screenPosition);
            
            // Detect swipe
            SwipeDirection swipeDirection = DetectSwipe();
            if (swipeDirection != SwipeDirection.None)
            {
                OnSwipeDetected?.Invoke(swipeDirection);
                Debug.Log($"Swipe detected: {swipeDirection}");
            }
            
            // Clean up visual feedback
            CleanupTouchVisuals();
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
            float swipeDistance = Vector2.Distance(startTouchPosition, endTouchPosition);
            if (swipeDistance < minSwipeDistance)
            {
                return SwipeDirection.None;
            }
            
            // Calculate swipe direction
            Vector2 swipeVector = endTouchPosition - startTouchPosition;
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
        
        public SwipeDirection GetSwipeDirection(Vector2 startPos, Vector2 endPos)
        {
            Vector2 swipeVector = endPos - startPos;
            float angle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
            
            if (angle < 0) angle += 360;
            
            if (angle >= 45 && angle < 135) return SwipeDirection.Up;
            if (angle >= 135 && angle < 225) return SwipeDirection.Left;
            if (angle >= 225 && angle < 315) return SwipeDirection.Down;
            return SwipeDirection.Right;
        }
        
        #endregion
        
        #region Visual Feedback
        
        private void CreateTouchIndicator(Vector2 screenPosition)
        {
            if (touchIndicatorPrefab == null) return;
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10));
            currentTouchIndicator = Instantiate(touchIndicatorPrefab, worldPosition, Quaternion.identity);
        }
        
        private void CreateSwipeTrail(Vector2 screenPosition)
        {
            if (swipeTrailPrefab == null) return;
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10));
            currentSwipeTrail = Instantiate(swipeTrailPrefab, worldPosition, Quaternion.identity);
        }
        
        private void UpdateSwipeTrail(Vector2 screenPosition)
        {
            if (currentSwipeTrail == null) return;
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10));
            currentSwipeTrail.transform.position = worldPosition;
        }
        
        private void CleanupTouchVisuals()
        {
            if (currentTouchIndicator != null)
            {
                Destroy(currentTouchIndicator);
                currentTouchIndicator = null;
            }
            
            if (currentSwipeTrail != null)
            {
                Destroy(currentSwipeTrail);
                currentSwipeTrail = null;
            }
        }
        
        #endregion
        
        #region Grid Coordinate Conversion
        
        public Vector2Int ScreenToGridPosition(Vector2 screenPosition)
        {
            if (mainCamera == null) return new Vector2Int(-1, -1);
            
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
        
        #endregion
        
        #region Public Methods
        
        public void SetSwipeSettings(float minDistance, float maxTime, float sensitivity)
        {
            minSwipeDistance = minDistance;
            maxSwipeTime = maxTime;
            swipeSensitivity = sensitivity;
        }
        
        public bool IsTouching()
        {
            return isTouching;
        }
        
        public Vector2 GetCurrentTouchPosition()
        {
            return endTouchPosition;
        }
        
        public float GetTouchDuration()
        {
            return isTouching ? Time.time - touchStartTime : 0f;
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Test Swipe Detection")]
        public void DebugTestSwipeDetection()
        {
            Vector2 testStart = new Vector2(100, 100);
            Vector2 testEnd = new Vector2(200, 100);
            
            SwipeDirection direction = GetSwipeDirection(testStart, testEnd);
            Debug.Log($"Test swipe from {testStart} to {testEnd} = {direction}");
        }
        
        [ContextMenu("Toggle Touch Debug")]
        private void DebugToggleTouchDebug()
        {
            enableTouchDebug = !enableTouchDebug;
            Debug.Log($"Touch debug {(enableTouchDebug ? "enabled" : "disabled")}");
        }
        
        private void OnDrawGizmos()
        {
            if (!enableTouchDebug || !isTouching) return;
            
            // Draw touch path
            Gizmos.color = Color.yellow;
            Vector3 startWorld = mainCamera.ScreenToWorldPoint(new Vector3(startTouchPosition.x, startTouchPosition.y, 10));
            Vector3 endWorld = mainCamera.ScreenToWorldPoint(new Vector3(endTouchPosition.x, endTouchPosition.y, 10));
            
            Gizmos.DrawLine(startWorld, endWorld);
            Gizmos.DrawWireSphere(startWorld, 0.1f);
            Gizmos.DrawWireSphere(endWorld, 0.1f);
        }
        
        #endregion
    }
} 