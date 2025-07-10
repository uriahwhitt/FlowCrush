using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCrush.Core
{
    // Core enums for the game
    public enum SwipeDirection
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
    
    public enum ZoneType
    {
        Edge,
        Transition,
        Center
    }
    
    public enum TileColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Orange
    }
    
    // Core data structures
    [System.Serializable]
    public class Match
    {
        public List<Vector2Int> matchedPositions;
        public TileColor matchColor;
        public int matchSize;
        public ZoneType primaryZone;
        public float scoreMultiplier;
        
        public Match(List<Vector2Int> positions, TileColor color, ZoneType zone)
        {
            matchedPositions = new List<Vector2Int>(positions);
            matchColor = color;
            matchSize = positions.Count;
            primaryZone = zone;
            scoreMultiplier = CalculateScoreMultiplier();
        }
        
        private float CalculateScoreMultiplier()
        {
            // Base multiplier based on zone
            float zoneMultiplier = primaryZone switch
            {
                ZoneType.Edge => 1.0f,
                ZoneType.Transition => 1.5f,
                ZoneType.Center => 2.0f,
                _ => 1.0f
            };
            
            // Bonus for larger matches
            float sizeMultiplier = matchSize switch
            {
                3 => 1.0f,
                4 => 1.5f,
                5 => 2.0f,
                _ => 2.5f // 6+ tiles
            };
            
            return zoneMultiplier * sizeMultiplier;
        }
    }
    
    [System.Serializable]
    public class TileData
    {
        public Vector2Int gridPosition;
        public TileColor color;
        public ZoneType zone;
        public bool isMatched;
        public bool isMoving;
        
        public TileData(Vector2Int pos, TileColor tileColor)
        {
            gridPosition = pos;
            color = tileColor;
            zone = ZoneCalculator.GetZoneType(pos);
            isMatched = false;
            isMoving = false;
        }
    }
    
    // Utility class for zone calculations
    public static class ZoneCalculator
    {
        private const int GRID_SIZE = 8;
        
        public static ZoneType GetZoneType(Vector2Int position)
        {
            // Edge zone: outer 2 rings
            if (position.x < 2 || position.x >= GRID_SIZE - 2 || 
                position.y < 2 || position.y >= GRID_SIZE - 2)
            {
                return ZoneType.Edge;
            }
            
            // Center zone: inner 4x4 area
            if (position.x >= 2 && position.x < GRID_SIZE - 2 && 
                position.y >= 2 && position.y < GRID_SIZE - 2)
            {
                return ZoneType.Center;
            }
            
            // Transition zone: everything else
            return ZoneType.Transition;
        }
        
        public static Vector2Int GetGridCenter()
        {
            return new Vector2Int(GRID_SIZE / 2, GRID_SIZE / 2);
        }
        
        public static bool IsValidGridPosition(Vector2Int position)
        {
            return position.x >= 0 && position.x < GRID_SIZE && 
                   position.y >= 0 && position.y < GRID_SIZE;
        }
    }
    
    // Event system for decoupled communication
    public static class GameEvents
    {
        public static event Action<SwipeDirection> OnSwipeDetected;
        public static event Action<Match> OnMatchFound;
        public static event Action<int> OnScoreChanged;
        public static event Action<Vector2Int, TileColor> OnTileSpawned;
        public static event Action<Vector2Int> OnTileRemoved;
        public static event Action<ZoneType> OnZoneEntered;
        
        public static void TriggerSwipeDetected(SwipeDirection direction)
        {
            OnSwipeDetected?.Invoke(direction);
        }
        
        public static void TriggerMatchFound(Match match)
        {
            OnMatchFound?.Invoke(match);
        }
        
        public static void TriggerScoreChanged(int newScore)
        {
            OnScoreChanged?.Invoke(newScore);
        }
        
        public static void TriggerTileSpawned(Vector2Int position, TileColor color)
        {
            OnTileSpawned?.Invoke(position, color);
        }
        
        public static void TriggerTileRemoved(Vector2Int position)
        {
            OnTileRemoved?.Invoke(position);
        }
        
        public static void TriggerZoneEntered(ZoneType zone)
        {
            OnZoneEntered?.Invoke(zone);
        }
    }
    
    // Configuration data for easy tuning
    [CreateAssetMenu(fileName = "GameConfig", menuName = "FlowCrush/Game Configuration")]
    public class GameConfig : ScriptableObject
    {
        [Header("Grid Settings")]
        public int gridWidth = 8;
        public int gridHeight = 8;
        public float tileSize = 1f;
        public float tileSpacing = 0.1f;
        
        [Header("Swipe Settings")]
        public float minSwipeDistance = 50f;
        public float maxSwipeTime = 1f;
        public float swipeSensitivity = 1f;
        
        [Header("Block Flow Settings")]
        public float blockMoveSpeed = 5f;
        public float blockSpawnDelay = 0.1f;
        public int blocksPerSwipe = 3;
        
        [Header("Pressure System")]
        public float pressureStrength = 2f;
        public float pressureUpdateRate = 0.1f;
        public AnimationCurve pressureCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        
        [Header("Scoring")]
        public int baseScorePerMatch = 100;
        public float comboMultiplier = 1.5f;
        public int maxComboBonus = 5;
        
        [Header("Visual Effects")]
        public float matchAnimationDuration = 0.5f;
        public float tileMoveAnimationDuration = 0.3f;
        public Color[] tileColors = new Color[6];
        
        [Header("Audio")]
        public AudioClip matchSound;
        public AudioClip swipeSound;
        public AudioClip pressureSound;
        public AudioClip scoreSound;
    }
} 