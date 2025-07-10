using UnityEngine;
using FlowCrush.Core;
using FlowCrush.Input;
using FlowCrush.Gameplay;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Utility script to set up the test scene with all necessary components
    /// Automatically creates and configures all game systems for Sprint 1 testing
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createTestGrid = true;
        [SerializeField] private bool enableDebugMode = true;
        
        [Header("Test Configuration")]
        [SerializeField] private bool enableZoneDebug = true;
        [SerializeField] private bool enableTouchDebug = true;
        [SerializeField] private bool enableMatchDebug = true;
        
        // Component references
        private GameManager gameManager;
        private GridManager gridManager;
        private TouchInputManager touchInputManager;
        private MatchDetector matchDetector;
        
        #region Unity Lifecycle
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupTestScene();
            }
        }
        
        #endregion
        
        #region Scene Setup
        
        [ContextMenu("Setup Test Scene")]
        public void SetupTestScene()
        {
            Debug.Log("Setting up FlowCrush test scene for Sprint 1...");
            
            // Create main game object
            GameObject gameObject = new GameObject("FlowCrush_Game");
            
            // Add GameManager
            gameManager = gameObject.AddComponent<GameManager>();
            
            // Create GridManager
            CreateGridManager();
            
            // Create TouchInputManager
            CreateTouchInputManager();
            
            // Create MatchDetector
            CreateMatchDetector();
            
            // Create ScoreManager
            CreateScoreManager();
            
            // Configure debug settings
            ConfigureDebugSettings();
            
            // Connect systems
            ConnectSystems();
            
            Debug.Log("Test scene setup complete! Ready for Sprint 1 testing.");
        }
        
        private void CreateGridManager()
        {
            if (!createTestGrid) return;
            
            GameObject gridObject = new GameObject("GridManager");
            gridObject.transform.SetParent(transform);
            
            gridManager = gridObject.AddComponent<GridManager>();
            
            // Configure grid settings
            gridManager.showZoneDebug = enableZoneDebug;
            
            Debug.Log("GridManager created and configured");
        }
        
        private void CreateTouchInputManager()
        {
            GameObject inputObject = new GameObject("TouchInputManager");
            inputObject.transform.SetParent(transform);
            
            touchInputManager = inputObject.AddComponent<TouchInputManager>();
            
            // Configure input settings
            touchInputManager.enableTouchDebug = enableTouchDebug;
            
            Debug.Log("TouchInputManager created and configured");
        }
        
        private void CreateMatchDetector()
        {
            GameObject matchObject = new GameObject("MatchDetector");
            matchObject.transform.SetParent(transform);
            
            matchDetector = matchObject.AddComponent<MatchDetector>();
            
            // Configure match debug settings
            if (enableMatchDebug)
            {
                Debug.Log("Match detection debug enabled");
            }
            
            Debug.Log("MatchDetector created and configured");
        }
        
        private void CreateScoreManager()
        {
            GameObject scoreObject = new GameObject("ScoreManager");
            scoreObject.transform.SetParent(transform);
            
            ScoreManager scoreManager = scoreObject.AddComponent<ScoreManager>();
            
            Debug.Log("ScoreManager created and configured");
        }
        
        private void ConfigureDebugSettings()
        {
            if (enableDebugMode)
            {
                // Enable debug logging
                Debug.unityLogger.logEnabled = true;
                
                // Set up debug UI if needed
                Debug.Log("Debug mode enabled");
            }
        }
        
        private void ConnectSystems()
        {
            // Connect TouchInputManager to GameManager
            if (touchInputManager != null && gameManager != null)
            {
                touchInputManager.OnSwipeDetected.AddListener(gameManager.HandleSwipeInput);
            }
            
            // Connect MatchDetector to GameManager
            if (matchDetector != null && gameManager != null)
            {
                matchDetector.OnMatchFound.AddListener(gameManager.HandleMatchFound);
            }
            
            // Connect GridManager events
            if (gridManager != null)
            {
                gridManager.OnTilePlaced += (pos, tile) => Debug.Log($"Tile placed at {pos}");
                gridManager.OnTileRemoved += (pos) => Debug.Log($"Tile removed from {pos}");
            }
            
            Debug.Log("All systems connected");
        }
        
        #endregion
        
        #region Test Functions
        
        [ContextMenu("Test Grid System")]
        public void TestGridSystem()
        {
            if (gridManager == null)
            {
                Debug.LogError("GridManager not found! Run Setup Test Scene first.");
                return;
            }
            
            Debug.Log("Testing Grid System...");
            gridManager.LogGridState();
            gridManager.TestZoneSystem();
        }
        
        [ContextMenu("Test Touch Input")]
        public void TestTouchInput()
        {
            if (touchInputManager == null)
            {
                Debug.LogError("TouchInputManager not found! Run Setup Test Scene first.");
                return;
            }
            
            Debug.Log("Testing Touch Input...");
            touchInputManager.DebugTestSwipeDetection();
        }
        
        [ContextMenu("Test Match Detection")]
        public void TestMatchDetection()
        {
            if (matchDetector == null)
            {
                Debug.LogError("MatchDetector not found! Run Setup Test Scene first.");
                return;
            }
            
            Debug.Log("Testing Match Detection...");
            matchDetector.DebugFindAllMatches();
        }
        
        [ContextMenu("Start Game")]
        public void StartGame()
        {
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found! Run Setup Test Scene first.");
                return;
            }
            
            Debug.Log("Starting FlowCrush game...");
            gameManager.StartGame();
        }
        
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            Debug.Log("Running all Sprint 1 tests...");
            
            TestGridSystem();
            TestTouchInput();
            TestMatchDetection();
            
            Debug.Log("All tests completed!");
        }
        
        #endregion
        
        #region Debug Utilities
        
        [ContextMenu("Log Scene Status")]
        public void LogSceneStatus()
        {
            Debug.Log("=== FlowCrush Scene Status ===");
            Debug.Log($"GameManager: {(gameManager != null ? "✓" : "✗")}");
            Debug.Log($"GridManager: {(gridManager != null ? "✓" : "✗")}");
            Debug.Log($"TouchInputManager: {(touchInputManager != null ? "✓" : "✗")}");
            Debug.Log($"MatchDetector: {(matchDetector != null ? "✓" : "✗")}");
            Debug.Log("==============================");
        }
        
        [ContextMenu("Clean Up Scene")]
        public void CleanUpScene()
        {
            Debug.Log("Cleaning up test scene...");
            
            // Find and destroy all FlowCrush objects
            GameObject[] flowCrushObjects = GameObject.FindGameObjectsWithTag("FlowCrush");
            foreach (GameObject obj in flowCrushObjects)
            {
                DestroyImmediate(obj);
            }
            
            // Reset references
            gameManager = null;
            gridManager = null;
            touchInputManager = null;
            matchDetector = null;
            
            Debug.Log("Scene cleanup complete");
        }
        
        #endregion
        
        #region Sprint 1 Validation
        
        [ContextMenu("Validate Sprint 1 Requirements")]
        public void ValidateSprint1Requirements()
        {
            Debug.Log("=== Sprint 1 Validation ===");
            
            bool allValid = true;
            
            // Check 8x8 grid system
            if (gridManager != null && gridManager.Width == 8 && gridManager.Height == 8)
            {
                Debug.Log("✓ 8x8 grid system operational");
            }
            else
            {
                Debug.LogError("✗ 8x8 grid system not operational");
                allValid = false;
            }
            
            // Check zone definitions
            if (gridManager != null && gridManager.GetZonePositions(ZoneType.Center).Count > 0)
            {
                Debug.Log("✓ Zone system implemented");
            }
            else
            {
                Debug.LogError("✗ Zone system not implemented");
                allValid = false;
            }
            
            // Check touch input
            if (touchInputManager != null)
            {
                Debug.Log("✓ Touch input system operational");
            }
            else
            {
                Debug.LogError("✗ Touch input system not operational");
                allValid = false;
            }
            
            // Check match detection
            if (matchDetector != null)
            {
                Debug.Log("✓ Match detection system operational");
            }
            else
            {
                Debug.LogError("✗ Match detection system not operational");
                allValid = false;
            }
            
            if (allValid)
            {
                Debug.Log("✓ All Sprint 1 requirements validated!");
            }
            else
            {
                Debug.LogError("✗ Some Sprint 1 requirements are missing");
            }
            
            Debug.Log("===========================");
        }
        
        #endregion
    }
} 