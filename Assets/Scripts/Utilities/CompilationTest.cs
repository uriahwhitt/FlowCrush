using UnityEngine;
using FlowCrush.Input;
using FlowCrush.Core;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Test script to verify input system compilation
    /// </summary>
    public class CompilationTest : MonoBehaviour
    {
        [Header("Input System Test")]
        [SerializeField] private SimpleInputManager simpleInputManager;
        [SerializeField] private TouchInputManager touchInputManager;
        
        private void Start()
        {
            // Test that we can access input managers
            if (simpleInputManager != null)
            {
                Debug.Log("SimpleInputManager reference is valid");
            }
            
            if (touchInputManager != null)
            {
                Debug.Log("TouchInputManager reference is valid");
            }
            
            // Test that we can access SwipeDirection enum
            SwipeDirection testDirection = SwipeDirection.Up;
            Debug.Log($"SwipeDirection test: {testDirection}");
            
            // Test that we can access Vector2 (should work)
            Vector2 testPosition = Vector2.zero;
            Debug.Log($"Vector2 test: {testPosition}");
            
            Debug.Log("Input system compilation test passed!");
        }
        
        [ContextMenu("Test Input System")]
        public void TestInputSystem()
        {
            Debug.Log("Input system test triggered from context menu");
        }
    }
} 