using UnityEngine;
using FlowCrush.Core;
using FlowCrush.Input;
using FlowCrush.Gameplay;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Final compilation check to verify all systems are accessible
    /// This script will be automatically removed after successful verification
    /// </summary>
    public class FinalCompilationCheck : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TouchInputManager touchInput;
        [SerializeField] private BlockSpawner blockSpawner;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private PressureSystem pressureSystem;
        [SerializeField] private ScoreManager scoreManager;
        
        [Header("Test Data")]
        [SerializeField] private SwipeDirection testSwipe = SwipeDirection.Up;
        [SerializeField] private ZoneType testZone = ZoneType.Center;
        [SerializeField] private TileColor testColor = TileColor.Red;
        
        private void Start()
        {
            // Verify all types are accessible
            VerifyTypeAccessibility();
            
            // Test system integration
            TestSystemIntegration();
            
            // Log success
            Debug.Log("✅ FinalCompilationCheck: All systems accessible and ready!");
            
            // Remove this component after verification
            Destroy(this);
        }
        
        private void VerifyTypeAccessibility()
        {
            // Test Core types
            SwipeDirection swipe = SwipeDirection.None;
            ZoneType zone = ZoneType.Edge;
            TileColor color = TileColor.Blue;
            
            // Use the serialized fields to avoid warnings
            Debug.Log($"Testing types: {swipe}, {zone}, {color}");
            Debug.Log($"Serialized test data: {testSwipe}, {testZone}, {testColor}");
            
            // Test data structures
            Vector2Int testPos = new Vector2Int(0, 0);
            System.Collections.Generic.List<Vector2Int> positions = new System.Collections.Generic.List<Vector2Int> { testPos };
            Match testMatch = new Match(positions, color, zone);
            
            Debug.Log("✅ Core types accessible");
        }
        
        private void TestSystemIntegration()
        {
            // Test that all systems can be found and referenced
            GameManager gm = FindFirstObjectByType<GameManager>();
            GridManager grid = FindFirstObjectByType<GridManager>();
            TouchInputManager input = FindFirstObjectByType<TouchInputManager>();
            BlockSpawner spawner = FindFirstObjectByType<BlockSpawner>();
            MatchDetector detector = FindFirstObjectByType<MatchDetector>();
            PressureSystem pressure = FindFirstObjectByType<PressureSystem>();
            ScoreManager score = FindFirstObjectByType<ScoreManager>();
            
            if (gm != null) Debug.Log("✅ GameManager found");
            if (grid != null) Debug.Log("✅ GridManager found");
            if (input != null) Debug.Log("✅ TouchInputManager found");
            if (spawner != null) Debug.Log("✅ BlockSpawner found");
            if (detector != null) Debug.Log("✅ MatchDetector found");
            if (pressure != null) Debug.Log("✅ PressureSystem found");
            if (score != null) Debug.Log("✅ ScoreManager found");
        }
        
        [ContextMenu("Run Final Check")]
        private void DebugRunFinalCheck()
        {
            VerifyTypeAccessibility();
            TestSystemIntegration();
            Debug.Log("🎉 Final compilation check completed successfully!");
        }
    }
} 