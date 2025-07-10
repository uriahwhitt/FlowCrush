using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FlowCrush.Core;

namespace FlowCrush.Gameplay
{
    /// <summary>
    /// Detects and processes tile matches in the game grid
    /// Handles horizontal and vertical match detection with zone-based scoring
    /// </summary>
    public class MatchDetector : MonoBehaviour
    {
        [Header("Match Detection Settings")]
        [SerializeField] private int minMatchSize = 3;
        [SerializeField] private bool enableDiagonalMatches = false;
        [SerializeField] private bool enableCascadingMatches = true;
        
        [Header("Events")]
        public UnityEvent<Match> OnMatchFound;
        public UnityEvent<List<Match>> OnMatchesProcessed;
        
        // References
        private GridManager gridManager;
        private List<Match> currentMatches;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("MatchDetector: No GridManager found in scene!");
            }
            
            currentMatches = new List<Match>();
        }
        
        #endregion
        
        #region Match Detection
        
        public List<Match> FindAllMatches()
        {
            currentMatches.Clear();
            
            if (gridManager == null) return currentMatches;
            
            // Find horizontal matches
            FindHorizontalMatches();
            
            // Find vertical matches
            FindVerticalMatches();
            
            // Find diagonal matches if enabled
            if (enableDiagonalMatches)
            {
                FindDiagonalMatches();
            }
            
            return currentMatches;
        }
        
        private void FindHorizontalMatches()
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                for (int x = 0; x < gridManager.Width; x++)
                {
                    List<Vector2Int> matchPositions = new List<Vector2Int>();
                    TileColor currentColor = TileColor.Red; // Default
                    bool colorSet = false;
                    
                    // Check for consecutive tiles of the same color
                    for (int checkX = x; checkX < gridManager.Width; checkX++)
                    {
                        Tile tile = gridManager.GetTile(checkX, y);
                        
                        if (tile == null) break;
                        
                        if (!colorSet)
                        {
                            currentColor = tile.TileColor;
                            colorSet = true;
                        }
                        
                        if (tile.TileColor == currentColor && !tile.IsMatched())
                        {
                            matchPositions.Add(new Vector2Int(checkX, y));
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    // Create match if we have enough tiles
                    if (matchPositions.Count >= minMatchSize)
                    {
                        CreateMatch(matchPositions, currentColor);
                        x += matchPositions.Count - 1; // Skip already matched tiles
                    }
                }
            }
        }
        
        private void FindVerticalMatches()
        {
            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    List<Vector2Int> matchPositions = new List<Vector2Int>();
                    TileColor currentColor = TileColor.Red; // Default
                    bool colorSet = false;
                    
                    // Check for consecutive tiles of the same color
                    for (int checkY = y; checkY < gridManager.Height; checkY++)
                    {
                        Tile tile = gridManager.GetTile(x, checkY);
                        
                        if (tile == null) break;
                        
                        if (!colorSet)
                        {
                            currentColor = tile.TileColor;
                            colorSet = true;
                        }
                        
                        if (tile.TileColor == currentColor && !tile.IsMatched())
                        {
                            matchPositions.Add(new Vector2Int(x, checkY));
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    // Create match if we have enough tiles
                    if (matchPositions.Count >= minMatchSize)
                    {
                        CreateMatch(matchPositions, currentColor);
                        y += matchPositions.Count - 1; // Skip already matched tiles
                    }
                }
            }
        }
        
        private void FindDiagonalMatches()
        {
            // Check diagonal matches (top-left to bottom-right)
            for (int x = 0; x < gridManager.Width - minMatchSize + 1; x++)
            {
                for (int y = 0; y < gridManager.Height - minMatchSize + 1; y++)
                {
                    CheckDiagonalMatch(x, y, 1, 1); // Down-right diagonal
                }
            }
            
            // Check diagonal matches (top-right to bottom-left)
            for (int x = minMatchSize - 1; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height - minMatchSize + 1; y++)
                {
                    CheckDiagonalMatch(x, y, -1, 1); // Down-left diagonal
                }
            }
        }
        
        private void CheckDiagonalMatch(int startX, int startY, int deltaX, int deltaY)
        {
            List<Vector2Int> matchPositions = new List<Vector2Int>();
            TileColor currentColor = TileColor.Red; // Default
            bool colorSet = false;
            
            for (int i = 0; i < minMatchSize; i++)
            {
                int x = startX + (i * deltaX);
                int y = startY + (i * deltaY);
                
                Tile tile = gridManager.GetTile(x, y);
                
                if (tile == null) return;
                
                if (!colorSet)
                {
                    currentColor = tile.TileColor;
                    colorSet = true;
                }
                
                if (tile.TileColor == currentColor && !tile.IsMatched())
                {
                    matchPositions.Add(new Vector2Int(x, y));
                }
                else
                {
                    return;
                }
            }
            
            // Create match if we have enough tiles
            if (matchPositions.Count >= minMatchSize)
            {
                CreateMatch(matchPositions, currentColor);
            }
        }
        
        private void CreateMatch(List<Vector2Int> positions, TileColor color)
        {
            if (positions.Count < minMatchSize) return;
            
            // Determine primary zone for the match
            ZoneType primaryZone = DeterminePrimaryZone(positions);
            
            // Create match object
            Match match = new Match(positions, color, primaryZone);
            currentMatches.Add(match);
            
            // Mark tiles as matched
            foreach (Vector2Int pos in positions)
            {
                Tile tile = gridManager.GetTile(pos);
                if (tile != null)
                {
                    tile.SetMatched(true);
                }
            }
            
            // Trigger event
            OnMatchFound?.Invoke(match);
            
            Debug.Log($"Match found: {positions.Count} {color} tiles in {primaryZone} zone");
        }
        
        private ZoneType DeterminePrimaryZone(List<Vector2Int> positions)
        {
            // Count tiles in each zone
            int edgeCount = 0, transitionCount = 0, centerCount = 0;
            
            foreach (Vector2Int pos in positions)
            {
                ZoneType zone = gridManager.GetZoneType(pos);
                switch (zone)
                {
                    case ZoneType.Edge:
                        edgeCount++;
                        break;
                    case ZoneType.Transition:
                        transitionCount++;
                        break;
                    case ZoneType.Center:
                        centerCount++;
                        break;
                }
            }
            
            // Return the zone with the most tiles
            if (centerCount >= transitionCount && centerCount >= edgeCount)
                return ZoneType.Center;
            else if (transitionCount >= edgeCount)
                return ZoneType.Transition;
            else
                return ZoneType.Edge;
        }
        
        #endregion
        
        #region Match Processing
        
        public void ProcessMatches()
        {
            List<Match> matches = FindAllMatches();
            
            if (matches.Count > 0)
            {
                OnMatchesProcessed?.Invoke(matches);
                
                if (enableCascadingMatches)
                {
                    // Process cascading matches after a delay
                    StartCoroutine(ProcessCascadingMatches());
                }
            }
        }
        
        private System.Collections.IEnumerator ProcessCascadingMatches()
        {
            // Wait for match animations to complete
            yield return new WaitForSeconds(0.5f);
            
            // Remove matched tiles
            RemoveMatchedTiles();
            
            // Wait for tiles to fall
            yield return new WaitForSeconds(0.3f);
            
            // Check for new matches
            ProcessMatches();
        }
        
        private void RemoveMatchedTiles()
        {
            if (gridManager == null) return;
            
            for (int x = 0; x < gridManager.Width; x++)
            {
                for (int y = 0; y < gridManager.Height; y++)
                {
                    Tile tile = gridManager.GetTile(x, y);
                    if (tile != null && tile.IsMatched())
                    {
                        // Play destroy effect
                        tile.PlayDestroyEffect();
                        
                        // Clear the tile position
                        gridManager.ClearTile(x, y);
                    }
                }
            }
        }
        
        #endregion
        
        #region Public Methods
        
        public void SetMatchSettings(int minSize, bool enableDiagonal, bool enableCascading)
        {
            minMatchSize = minSize;
            enableDiagonalMatches = enableDiagonal;
            enableCascadingMatches = enableCascading;
        }
        
        public bool HasMatches()
        {
            return FindAllMatches().Count > 0;
        }
        
        public int GetMatchCount()
        {
            return currentMatches.Count;
        }
        
        public List<Match> GetCurrentMatches()
        {
            return new List<Match>(currentMatches);
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Find All Matches")]
        public void DebugFindAllMatches()
        {
            List<Match> matches = FindAllMatches();
            Debug.Log($"Found {matches.Count} matches");
            
            foreach (Match match in matches)
            {
                Debug.Log($"Match: {match.matchSize} {match.matchColor} tiles in {match.primaryZone} zone");
            }
        }
        
        [ContextMenu("Process Matches")]
        private void DebugProcessMatches()
        {
            ProcessMatches();
        }
        
        [ContextMenu("Test Match Detection")]
        private void DebugTestMatchDetection()
        {
            // Create a test scenario
            Debug.Log("Testing match detection...");
            
            // This would create a test grid with known matches
            // For now, just log the current state
            Debug.Log($"Grid size: {gridManager?.Width}x{gridManager?.Height}");
            Debug.Log($"Current matches: {currentMatches.Count}");
        }
        
        #endregion
    }
} 