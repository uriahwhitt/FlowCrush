using UnityEngine;
using FlowCrush.Input;
using FlowCrush.Core;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Diagnostic script to test input system functionality
    /// Helps identify input issues and verify system connections
    /// </summary>
    public class InputDiagnostic : MonoBehaviour
    {
        [Header("Diagnostic Settings")]
        [SerializeField] private bool enableDiagnostic = true;
        [SerializeField] private bool logAllInput = false;
        
        private SimpleInputSystemManager inputManager;
        private GameManager gameManager;
        
        private void Start()
        {
            if (!enableDiagnostic) return;
            
            Debug.Log("=== INPUT DIAGNOSTIC STARTING ===");
            
            // Wait a frame to ensure all systems are initialized
            StartCoroutine(DelayedDiagnostic());
        }
        
        private System.Collections.IEnumerator DelayedDiagnostic()
        {
            // Wait for next frame to ensure all systems are initialized
            yield return null;
            
            // Find input manager
            inputManager = FindFirstObjectByType<SimpleInputSystemManager>();
            if (inputManager == null)
            {
                Debug.LogError("❌ No SimpleInputSystemManager found in scene!");
                yield break;
            }
            
            Debug.Log("✅ SimpleInputSystemManager found");
            
            // Find game manager
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("❌ No GameManager found in scene!");
                yield break;
            }
            
            Debug.Log("✅ GameManager found");
            
            // Subscribe to input events
            inputManager.OnSwipeDetected.AddListener(OnSwipeDetected);
            inputManager.OnTouchStart.AddListener(OnTouchStart);
            inputManager.OnTouchEnd.AddListener(OnTouchEnd);
            
            Debug.Log("✅ Input events subscribed");
            
            // Test input system status
            TestInputSystemStatus();
            
            Debug.Log("=== INPUT DIAGNOSTIC COMPLETE ===");
        }
        
        private void Update()
        {
            if (!enableDiagnostic || !logAllInput) return;
            
            // Log basic input status
            if (inputManager != null)
            {
                if (inputManager.IsTouching())
                {
                    Debug.Log($"Touch position: {inputManager.GetCurrentTouchPosition()}");
                }
            }
        }
        
        private void OnSwipeDetected(SwipeDirection direction)
        {
            Debug.Log($"🎯 SWIPE DETECTED: {direction}");
            
            // Check if GameManager received the swipe
            if (gameManager != null)
            {
                Debug.Log($"GameManager state: {gameManager.GetCurrentGameState()}");
                Debug.Log($"Game is playing: {gameManager.IsGamePlaying()}");
            }
        }
        
        private void OnTouchStart(Vector2 position)
        {
            Debug.Log($"👆 TOUCH START: {position}");
        }
        
        private void OnTouchEnd(Vector2 position)
        {
            Debug.Log($"👆 TOUCH END: {position}");
        }
        
        private void TestInputSystemStatus()
        {
            Debug.Log("=== INPUT SYSTEM STATUS ===");
            
            // Test Input System availability
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                Debug.Log("✅ Mouse.current is available");
                Debug.Log($"Mouse position: {UnityEngine.InputSystem.Mouse.current.position.ReadValue()}");
            }
            else
            {
                Debug.LogError("❌ Mouse.current is null");
            }
            
            if (UnityEngine.InputSystem.Touchscreen.current != null)
            {
                Debug.Log("✅ Touchscreen.current is available");
                Debug.Log($"Touch position: {UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue()}");
            }
            else
            {
                Debug.LogWarning("⚠️ Touchscreen.current is null (normal in editor)");
            }
            
            // Test input manager settings
            if (inputManager != null)
            {
                Debug.Log($"Input Manager Debug Mode: {inputManager.enableDebug}");
                Debug.Log($"Input Manager Is Touching: {inputManager.IsTouching()}");
            }
        }
        
        [ContextMenu("Test Input System")]
        public void TestInputSystem()
        {
            TestInputSystemStatus();
        }
        
        [ContextMenu("Test Swipe Detection")]
        public void TestSwipeDetection()
        {
            if (inputManager != null)
            {
                inputManager.DebugTestSwipeDetection();
            }
        }
        
        [ContextMenu("Toggle Input Logging")]
        public void ToggleInputLogging()
        {
            logAllInput = !logAllInput;
            Debug.Log($"Input logging: {(logAllInput ? "ON" : "OFF")}");
        }
    }
} 