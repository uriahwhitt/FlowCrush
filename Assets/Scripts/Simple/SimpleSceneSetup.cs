using UnityEngine;

namespace FlowCrush.Simple
{
    /// <summary>
    /// Simple scene setup that creates the SimpleGameManager automatically
    /// Just add this to any GameObject in your scene and it will set everything up
    /// </summary>
    public class SimpleSceneSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool autoSetupOnStart = true;
        
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupSimpleScene();
                Destroy(this); // Remove this component after setup
            }
        }
        
        [ContextMenu("Setup Simple Scene")]
        public void SetupSimpleScene()
        {
            Debug.Log("Setting up Simple FlowCrush scene...");
            
            // Disable Adaptive Performance warnings
            GameObject apDisablerObj = new GameObject("AdaptivePerformanceDisabler");
            AdaptivePerformanceDisabler apDisabler = apDisablerObj.AddComponent<AdaptivePerformanceDisabler>();
            apDisabler.DisableAdaptivePerformance();
            DestroyImmediate(apDisablerObj);
            
            Debug.Log("✅ Adaptive Performance disabled");
            
            // Create SimpleGameManager if it doesn't exist
            if (SimpleGameManager.Instance == null)
            {
                GameObject gameManagerObj = new GameObject("SimpleGameManager");
                SimpleGameManager gameManager = gameManagerObj.AddComponent<SimpleGameManager>();
                
                Debug.Log("✅ SimpleGameManager created");
            }
            else
            {
                Debug.Log("✅ SimpleGameManager already exists");
            }
            
            // Create MouseClickDebugger for troubleshooting
            GameObject debuggerObj = new GameObject("MouseClickDebugger");
            MouseClickDebugger debugger = debuggerObj.AddComponent<MouseClickDebugger>();
            
            Debug.Log("✅ MouseClickDebugger created");
            
            Debug.Log("Simple scene setup complete! Press Play to test.");
        }
    }
} 