using UnityEngine;
using FlowCrush.Input;
using FlowCrush.Core;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Test script to verify FallbackInputManager compilation
    /// </summary>
    public class InputCompilationTest : MonoBehaviour
    {
        [Header("Input System Test")]
        [SerializeField] private FallbackInputManager fallbackInputManager;
        
        private void Start()
        {
            // Test that we can access the input manager
            if (fallbackInputManager != null)
            {
                Debug.Log("FallbackInputManager reference is valid");
                
                // Test that we can access its methods
                bool isTouching = fallbackInputManager.IsTouching();
                Vector2 position = fallbackInputManager.GetCurrentTouchPosition();
                
                Debug.Log($"Input manager test - IsTouching: {isTouching}, Position: {position}");
            }
            
            // Test that we can access SwipeDirection enum
            SwipeDirection testDirection = SwipeDirection.Up;
            Debug.Log($"SwipeDirection test: {testDirection}");
            
            // Test that we can access Vector2 (should work)
            Vector2 testPosition = Vector2.zero;
            Debug.Log($"Vector2 test: {testPosition}");
            
            Debug.Log("Input compilation test passed!");
        }
        
        [ContextMenu("Test Input System")]
        public void TestInputSystem()
        {
            Debug.Log("Input system test triggered from context menu");
            
            // Test creating a new input manager
            GameObject testObj = new GameObject("TestInputManager");
            FallbackInputManager testInput = testObj.AddComponent<FallbackInputManager>();
            
            if (testInput != null)
            {
                Debug.Log("Successfully created FallbackInputManager");
                Destroy(testObj);
            }
        }
    }
} 