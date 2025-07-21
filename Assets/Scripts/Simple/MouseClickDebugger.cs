using UnityEngine;

namespace FlowCrush.Simple
{
    /// <summary>
    /// Simple mouse click debugger to help diagnose click detection issues
    /// </summary>
    public class MouseClickDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebug = true;
        [SerializeField] private bool logMousePosition = true;
        [SerializeField] private bool logRaycastHits = true;
        
        private Camera mainCamera;
        
        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("MouseClickDebugger: No main camera found!");
            }
            else
            {
                Debug.Log($"MouseClickDebugger: Found main camera at {mainCamera.transform.position}");
            }
        }
        
        // Note: Update() method removed because it conflicts with Unity's new Input System
        // The debugger now relies on context menu methods for testing
        // OnMouseDown() in SimpleTile handles the actual click detection
        
        [ContextMenu("Test Camera Setup")]
        public void TestCameraSetup()
        {
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found!");
                return;
            }
            
            Debug.Log($"Camera position: {mainCamera.transform.position}");
            Debug.Log($"Camera size: {mainCamera.orthographicSize}");
            Debug.Log($"Camera projection: {mainCamera.orthographic}");
            Debug.Log($"Camera culling mask: {mainCamera.cullingMask}");
        }
        
        [ContextMenu("Test Tile Colliders")]
        public void TestTileColliders()
        {
            SimpleTile[] tiles = FindObjectsByType<SimpleTile>(FindObjectsSortMode.None);
            Debug.Log($"Found {tiles.Length} SimpleTile objects");
            
            foreach (SimpleTile tile in tiles)
            {
                Collider2D collider = tile.GetComponent<Collider2D>();
                if (collider != null)
                {
                    Debug.Log($"Tile {tile.name} has collider: {collider.GetType().Name}");
                }
                else
                {
                    Debug.LogError($"Tile {tile.name} has NO collider!");
                }
            }
        }
        
        [ContextMenu("Test Mouse Position")]
        public void TestMousePosition()
        {
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            Debug.Log($"Current mouse position: {mousePos}");
            
            if (mainCamera != null)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
                Debug.Log($"Mouse world position: {worldPos}");
            }
        }
    }
} 