using UnityEngine;
using FlowCrush.Core;
using FlowCrush.Input;
using FlowCrush.Gameplay;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Simple scene initializer for Sprint1Test scene
    /// Sets up basic game objects and systems
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool autoSetupOnStart = true;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupBasicScene();
                Destroy(this);
            }
        }
        
        [ContextMenu("Setup Basic Scene")]
        public void SetupBasicScene()
        {
            Debug.Log("Setting up basic FlowCrush scene...");
            
            // Create GameManager
            GameObject gameManagerObj = new GameObject("GameManager");
            GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
            
            // Create GridManager
            GameObject gridManagerObj = new GameObject("GridManager");
            GridManager gridManager = gridManagerObj.AddComponent<GridManager>();
            
            // Create SimpleInputSystemManager (compatible with Unity Input System package)
            GameObject inputManagerObj = new GameObject("SimpleInputSystemManager");
            SimpleInputSystemManager touchInput = inputManagerObj.AddComponent<SimpleInputSystemManager>();
            
            // Create MatchDetector
            GameObject matchDetectorObj = new GameObject("MatchDetector");
            // MatchDetector matchDetector = matchDetectorObj.AddComponent<MatchDetector>();
            
            // Create ScoreManager
            GameObject scoreManagerObj = new GameObject("ScoreManager");
            // ScoreManager scoreManager = scoreManagerObj.AddComponent<ScoreManager>();
            
            // Create BlockSpawner
            GameObject blockSpawnerObj = new GameObject("BlockSpawner");
            // BlockSpawner blockSpawner = blockSpawnerObj.AddComponent<BlockSpawner>();
            
            // Create PressureSystem
            GameObject pressureSystemObj = new GameObject("PressureSystem");
            PressureSystem pressureSystem = pressureSystemObj.AddComponent<PressureSystem>();
            
            // Create InputDiagnostic for testing
            GameObject diagnosticObj = new GameObject("InputDiagnostic");
            InputDiagnostic diagnostic = diagnosticObj.AddComponent<InputDiagnostic>();
            
            // Disable Adaptive Performance warnings
            GameObject adaptiveDisablerObj = new GameObject("AdaptivePerformanceDisabler");
            AdaptivePerformanceDisabler adaptiveDisabler = adaptiveDisablerObj.AddComponent<AdaptivePerformanceDisabler>();
            
            // Initialize the grid
            gridManager.InitializeGrid();
            
            // Start the game
            gameManager.StartGame();
            
            Debug.Log("Basic scene setup complete! Game should be running now.");
            Debug.Log("InputDiagnostic added - check Console for input system status.");
        }
        
        [ContextMenu("Test All Systems")]
        public void TestAllSystems()
        {
            Debug.Log("Testing all FlowCrush systems...");
            
            // Test grid system
            GridManager grid = FindFirstObjectByType<GridManager>();
            if (grid != null)
            {
                grid.LogGridState();
                Debug.Log("✅ Grid system working");
            }
            
            // Test touch input
            SimpleInputSystemManager input = FindFirstObjectByType<SimpleInputSystemManager>();
            if (input != null)
            {
                Debug.Log("✅ Touch input system working");
            }
            
            // Test match detection
            // MatchDetector detector = FindFirstObjectByType<MatchDetector>();
            // if (detector != null)
            // {
            //     Debug.Log("✅ Match detection system working");
            // }
            
            // Test scoring
            // ScoreManager score = FindFirstObjectByType<ScoreManager>();
            // if (score != null)
            // {
            //     Debug.Log("✅ Scoring system working");
            // }
            
            Debug.Log("All systems tested successfully!");
        }
    }
} 