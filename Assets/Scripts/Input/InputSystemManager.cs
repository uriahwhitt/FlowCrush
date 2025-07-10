using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using FlowCrush.Core;

namespace FlowCrush.Input
{
    /// <summary>
    /// Input System compatible input manager
    /// Uses the new Unity Input System package
    /// </summary>
    public class InputSystemManager : MonoBehaviour
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
        
        // Input System components
        private PlayerInput playerInput;
        private InputActionMap inputActionMap;
        private InputAction touchAction;
        private InputAction mouseAction;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("InputSystemManager: No main camera found!");
            }
            
            InitializeInputSystem();
        }
        
        private void Update()
        {
            // Input System handles input through callbacks, but we can add additional processing here
        }
        
        private void OnDestroy()
        {
            CleanupInputSystem();
        }
        
        #endregion
        
        #region Input System Setup
        
        private void InitializeInputSystem()
        {
            try
            {
                // Create input actions
                inputActionMap = new InputActionMap("GameInput");
                
                // Touch input action
                touchAction = inputActionMap.AddAction("Touch", InputActionType.PassThrough);
                touchAction.AddBinding("<Touchscreen>/touch*");
                touchAction.AddBinding("<Touchscreen>/primaryTouch");
                
                // Mouse input action for editor
                mouseAction = inputActionMap.AddAction("Mouse", InputActionType.PassThrough);
                mouseAction.AddBinding("<Mouse>/leftButton");
                mouseAction.AddBinding("<Mouse>/position");
                
                // Set up callbacks
                touchAction.performed += OnTouchPerformed;
                touchAction.started += OnTouchStarted;
                touchAction.canceled += OnTouchCanceled;
                
                mouseAction.performed += OnMousePerformed;
                mouseAction.started += OnMouseStarted;
                mouseAction.canceled += OnMouseCanceled;
                
                // Enable the action map
                inputActionMap.Enable();
                
                if (enableDebug)
                {
                    Debug.Log("InputSystemManager: Input System initialized successfully");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"InputSystemManager: Failed to initialize Input System - {e.Message}");
                // Fallback to basic input if Input System fails
                InitializeFallbackInput();
            }
        }
        
        private void InitializeFallbackInput()
        {
            Debug.LogWarning("InputSystemManager: Using fallback input method");
            // This will be called if Input System initialization fails
        }
        
        private void CleanupInputSystem()
        {
            if (inputActionMap != null)
            {
                inputActionMap.Disable();
                inputActionMap.Dispose();
            }
        }
        
        #endregion
        
        #region Input Callbacks
        
        private void OnTouchStarted(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            StartTouch(position);
        }
        
        private void OnTouchPerformed(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            UpdateTouch(position);
        }
        
        private void OnTouchCanceled(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            EndTouch(position);
        }
        
        private void OnMouseStarted(InputAction.CallbackContext context)
        {
            Vector2 position = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            StartTouch(position);
        }
        
        private void OnMousePerformed(InputAction.CallbackContext context)
        {
            Vector2 position = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            UpdateTouch(position);
        }
        
        private void OnMouseCanceled(InputAction.CallbackContext context)
        {
            Vector2 position = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            EndTouch(position);
        }
        
        #endregion
        
        #region Touch Handling
        
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
        
        [ContextMenu("Test Input System")]
        public void DebugTestInputSystem()
        {
            Debug.Log($"Input System Status:");
            Debug.Log($"- Touch Action: {(touchAction?.enabled == true ? "Enabled" : "Disabled")}");
            Debug.Log($"- Mouse Action: {(mouseAction?.enabled == true ? "Enabled" : "Disabled")}");
            Debug.Log($"- Action Map: {(inputActionMap?.enabled == true ? "Enabled" : "Disabled")}");
        }
        
        #endregion
    }
} 