using UnityEngine;
using System.Collections.Generic;

namespace FlowCrush.Simple
{
    public class SimpleGameManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        public int gridWidth = 8;
        public int gridHeight = 8;
        public float tileSpacing = 1.1f;
        
        [Header("Prefabs")]
        public GameObject tilePrefab; // Assign in inspector
        
        [Header("Debug")]
        public bool showDebugInfo = true;
        
        // Singleton
        public static SimpleGameManager Instance;
        
        // Grid storage
        private SimpleTile[,] grid;
        private SimpleTile selectedTile = null;
        
        // Stats
        public int score = 0;
        public int matches = 0;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            CreateGrid();
        }
        
        void CreateGrid()
        {
            grid = new SimpleTile[gridWidth, gridHeight];
            
            // Create tile prefab if missing
            if (tilePrefab == null)
            {
                CreateDefaultTilePrefab();
            }
            
            // Create all tiles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    CreateTileAt(x, y);
                }
            }
            
            Debug.Log($"Created {gridWidth}x{gridHeight} grid with {gridWidth * gridHeight} tiles");
        }
        
        void CreateDefaultTilePrefab()
        {
            // Create a simple default tile prefab at runtime
            GameObject prefab = new GameObject("DefaultTile");
            
            // Add sprite renderer
            SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
            sr.sprite = CreateDefaultSprite();
            sr.sortingOrder = 1; // Ensure sprites are visible
            
            // Add collider for mouse detection - make it slightly larger
            BoxCollider2D collider = prefab.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.9f, 0.9f); // Slightly smaller than full size for visual gaps
            
            // Add our tile component
            SimpleTile tile = prefab.AddComponent<SimpleTile>();
            tile.spriteRenderer = sr;
            
            tilePrefab = prefab;
            
            Debug.Log("Created default tile prefab with collider");
        }
        
        Sprite CreateDefaultSprite()
        {
            // Create a simple square sprite
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }
        
        void CreateTileAt(int x, int y)
        {
            Vector3 position = new Vector3(
                x * tileSpacing - (gridWidth * tileSpacing) / 2f,
                y * tileSpacing - (gridHeight * tileSpacing) / 2f,
                0
            );
            
            GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            SimpleTile tile = tileObject.GetComponent<SimpleTile>();
            
            tile.SetPosition(x, y);
            grid[x, y] = tile;
        }
        
        // Called when player clicks a tile
        public void OnTileClicked(SimpleTile clickedTile)
        {
            if (showDebugInfo)
                Debug.Log($"Clicked tile at ({clickedTile.gridX}, {clickedTile.gridY}) - Color: {clickedTile.tileColor}");
            
            if (selectedTile == null)
            {
                // First selection
                SelectTile(clickedTile);
            }
            else if (selectedTile == clickedTile)
            {
                // Clicking same tile - deselect
                DeselectTile();
            }
            else if (selectedTile.IsAdjacentTo(clickedTile))
            {
                // Valid swap - try to make a match
                AttemptSwap(selectedTile, clickedTile);
                DeselectTile();
            }
            else
            {
                // Not adjacent - select new tile
                DeselectTile();
                SelectTile(clickedTile);
            }
        }
        
        void SelectTile(SimpleTile tile)
        {
            selectedTile = tile;
            tile.SetSelected(true);
            
            if (showDebugInfo)
                Debug.Log($"Selected tile at ({tile.gridX}, {tile.gridY})");
        }
        
        void DeselectTile()
        {
            if (selectedTile != null)
            {
                selectedTile.SetSelected(false);
                selectedTile = null;
            }
        }
        
        void AttemptSwap(SimpleTile tile1, SimpleTile tile2)
        {
            if (showDebugInfo)
                Debug.Log($"Attempting swap: ({tile1.gridX}, {tile1.gridY}) <-> ({tile2.gridX}, {tile2.gridY})");
            
            // Swap the colors
            SimpleTile.TileColor temp = tile1.tileColor;
            tile1.SetColor(tile2.tileColor);
            tile2.SetColor(temp);
            
            // Check for matches
            bool foundMatch = CheckForMatches();
            
            if (foundMatch)
            {
                score += 100;
                matches++;
                Debug.Log($"Match found! Score: {score}, Matches: {matches}");
            }
            else
            {
                // No match - swap back
                temp = tile1.tileColor;
                tile1.SetColor(tile2.tileColor);
                tile2.SetColor(temp);
                
                if (showDebugInfo)
                    Debug.Log("No match found - swapped back");
            }
        }
        
        bool CheckForMatches()
        {
            bool foundAnyMatch = false;
            List<SimpleTile> tilesToRemove = new List<SimpleTile>();
            
            // Check horizontal matches
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth - 2; x++)
                {
                    if (grid[x, y].tileColor == grid[x + 1, y].tileColor && 
                        grid[x + 1, y].tileColor == grid[x + 2, y].tileColor)
                    {
                        // Found horizontal match of 3
                        if (!tilesToRemove.Contains(grid[x, y])) tilesToRemove.Add(grid[x, y]);
                        if (!tilesToRemove.Contains(grid[x + 1, y])) tilesToRemove.Add(grid[x + 1, y]);
                        if (!tilesToRemove.Contains(grid[x + 2, y])) tilesToRemove.Add(grid[x + 2, y]);
                        foundAnyMatch = true;
                    }
                }
            }
            
            // Check vertical matches
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight - 2; y++)
                {
                    if (grid[x, y].tileColor == grid[x, y + 1].tileColor && 
                        grid[x, y + 1].tileColor == grid[x, y + 2].tileColor)
                    {
                        // Found vertical match of 3
                        if (!tilesToRemove.Contains(grid[x, y])) tilesToRemove.Add(grid[x, y]);
                        if (!tilesToRemove.Contains(grid[x, y + 1])) tilesToRemove.Add(grid[x, y + 1]);
                        if (!tilesToRemove.Contains(grid[x, y + 2])) tilesToRemove.Add(grid[x, y + 2]);
                        foundAnyMatch = true;
                    }
                }
            }
            
            // Remove matched tiles and replace them
            if (foundAnyMatch)
            {
                foreach (SimpleTile tile in tilesToRemove)
                {
                    tile.SetColor((SimpleTile.TileColor)Random.Range(0, 6));
                }
                
                Debug.Log($"Removed {tilesToRemove.Count} tiles in matches");
            }
            
            return foundAnyMatch;
        }
        
        // Debug GUI
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));
            GUILayout.Label($"Score: {score}");
            GUILayout.Label($"Matches: {matches}");
            GUILayout.Label($"Selected: {(selectedTile != null ? $"({selectedTile.gridX}, {selectedTile.gridY})" : "None")}");
            GUILayout.EndArea();
        }
    }
}