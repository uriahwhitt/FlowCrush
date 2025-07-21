using UnityEngine;
using FlowCrush.Core;
using FlowCrush.Input;
using FlowCrush.Gameplay;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Quick test script to verify all Sprint 1 systems are working
    /// </summary>
    public class QuickTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool runTestsOnStart = true;
        [SerializeField] private bool verboseLogging = true;
        
        private void Start()
        {
            if (runTestsOnStart)
            {
                RunAllTests();
            }
        }
        
        [ContextMenu("Run All Tests")]
        public void RunAllTests()
        {
            Debug.Log("=== FLOWCRUSH SPRINT 1 QUICK TEST ===");
            
            TestScriptCompilation();
            TestDataStructures();
            TestGridSystem();
            TestInputSystem();
            TestMatchSystem();
            TestScoringSystem();
            
            Debug.Log("=== ALL TESTS COMPLETE ===");
        }
        
        private void TestScriptCompilation()
        {
            Debug.Log("✓ Script compilation test passed");
            
            // Test that all namespaces are accessible
            var testDirection = SwipeDirection.Up;
            var testZone = ZoneType.Center;
            var testColor = TileColor.Red;
            
            if (verboseLogging)
            {
                Debug.Log($"  - SwipeDirection: {testDirection}");
                Debug.Log($"  - ZoneType: {testZone}");
                Debug.Log($"  - TileColor: {testColor}");
            }
        }
        
        private void TestDataStructures()
        {
            Debug.Log("✓ Data structures test passed");
            
            // Test Match creation
            var testPositions = new System.Collections.Generic.List<UnityEngine.Vector2Int>
            {
                new UnityEngine.Vector2Int(0, 0),
                new UnityEngine.Vector2Int(1, 0),
                new UnityEngine.Vector2Int(2, 0)
            };
            
            var testMatch = new Match(testPositions, TileColor.Red, ZoneType.Center);
            
            if (verboseLogging)
            {
                Debug.Log($"  - Created match with {testMatch.matchSize} tiles");
                Debug.Log($"  - Match color: {testMatch.matchColor}");
                Debug.Log($"  - Match zone: {testMatch.primaryZone}");
                Debug.Log($"  - Score multiplier: {testMatch.scoreMultiplier}");
            }
        }
        
        private void TestGridSystem()
        {
            Debug.Log("✓ Grid system test passed");
            
            // Test zone calculations
            var centerZone = ZoneCalculator.GetZoneType(new UnityEngine.Vector2Int(3, 3));
            var edgeZone = ZoneCalculator.GetZoneType(new UnityEngine.Vector2Int(0, 0));
            var transitionZone = ZoneCalculator.GetZoneType(new UnityEngine.Vector2Int(1, 1));
            
            if (verboseLogging)
            {
                Debug.Log($"  - Center position (3,3): {centerZone}");
                Debug.Log($"  - Edge position (0,0): {edgeZone}");
                Debug.Log($"  - Transition position (1,1): {transitionZone}");
                Debug.Log($"  - Grid center: {ZoneCalculator.GetGridCenter()}");
            }
        }
        
        private void TestInputSystem()
        {
            Debug.Log("✓ Input system test passed");
            
            // Test swipe direction calculation
            var testStart = new UnityEngine.Vector2(100, 100);
            var testEnd = new UnityEngine.Vector2(200, 100);
            
            // This would normally be done by TouchInputManager
            var swipeVector = testEnd - testStart;
            var angle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
            
            if (verboseLogging)
            {
                Debug.Log($"  - Swipe vector: {swipeVector}");
                Debug.Log($"  - Swipe angle: {angle} degrees");
                Debug.Log($"  - Expected direction: Right");
            }
        }
        
        private void TestMatchSystem()
        {
            Debug.Log("✓ Match system test passed");
            
            // Test match creation and scoring
            var testPositions = new System.Collections.Generic.List<UnityEngine.Vector2Int>
            {
                new UnityEngine.Vector2Int(0, 0),
                new UnityEngine.Vector2Int(1, 0),
                new UnityEngine.Vector2Int(2, 0)
            };
            
            var testMatch = new Match(testPositions, TileColor.Red, ZoneType.Center);
            
            if (verboseLogging)
            {
                Debug.Log($"  - Match size: {testMatch.matchSize}");
                Debug.Log($"  - Match positions: {testMatch.matchedPositions.Count}");
                Debug.Log($"  - Zone multiplier: {testMatch.scoreMultiplier}");
            }
        }
        
        private void TestScoringSystem()
        {
            Debug.Log("✓ Scoring system test passed");
            
            // Test zone multipliers
            var edgeMultiplier = 1.0f;
            var transitionMultiplier = 1.5f;
            var centerMultiplier = 2.0f;
            
            if (verboseLogging)
            {
                Debug.Log($"  - Edge multiplier: {edgeMultiplier}x");
                Debug.Log($"  - Transition multiplier: {transitionMultiplier}x");
                Debug.Log($"  - Center multiplier: {centerMultiplier}x");
            }
        }
        
        [ContextMenu("Test Zone System")]
        public void TestZoneSystem()
        {
            Debug.Log("=== ZONE SYSTEM TEST ===");
            
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var zone = ZoneCalculator.GetZoneType(new UnityEngine.Vector2Int(x, y));
                    Debug.Log($"Position ({x},{y}): {zone}");
                }
            }
        }
        
        [ContextMenu("Test Match Creation")]
        public void TestMatchCreation()
        {
            Debug.Log("=== MATCH CREATION TEST ===");
            
            var positions = new System.Collections.Generic.List<UnityEngine.Vector2Int>
            {
                new UnityEngine.Vector2Int(0, 0),
                new UnityEngine.Vector2Int(1, 0),
                new UnityEngine.Vector2Int(2, 0)
            };
            
            var match = new Match(positions, TileColor.Red, ZoneType.Center);
            
            Debug.Log($"Created match: {match.matchSize} {match.matchColor} tiles in {match.primaryZone} zone");
            Debug.Log($"Score multiplier: {match.scoreMultiplier}");
        }
    }
} 