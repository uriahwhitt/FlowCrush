using UnityEngine;
using UnityEditor;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Utility script to help fix Unity scene file issues
    /// This script will be automatically removed after use
    /// </summary>
    public class SceneFixer : MonoBehaviour
    {
        [Header("Scene Fix Options")]
        [SerializeField] private bool autoFixOnStart = true;
        
        private void Start()
        {
            if (autoFixOnStart)
            {
                FixSceneIssues();
                Destroy(this);
            }
        }
        
        [ContextMenu("Fix Scene Issues")]
        public void FixSceneIssues()
        {
            Debug.Log("Attempting to fix Unity scene issues...");
            
            // Force Unity to refresh assets
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Scene fix attempt completed. If issues persist, try:");
            Debug.Log("1. Close Unity and reopen the project");
            Debug.Log("2. Delete Library folder and let Unity regenerate");
            Debug.Log("3. Reimport all assets");
        }
        
        [ContextMenu("Log Scene Status")]
        public void LogSceneStatus()
        {
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"Scene count: {UnityEngine.SceneManagement.SceneManager.sceneCount}");
            
            // Check if Sprint1Test scene exists
            bool sprint1TestExists = false;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                if (scenePath.Contains("Sprint1Test"))
                {
                    sprint1TestExists = true;
                    break;
                }
            }
            
            Debug.Log($"Sprint1Test scene in build settings: {sprint1TestExists}");
        }
    }
} 