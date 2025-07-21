using UnityEngine;
using System.Collections.Generic;

public class SimpleFlowCrushManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float tileSpacing = 1.1f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    // Singleton
    public static SimpleFlowCrushManager Instance;
    
    // Grid storage
    private SimpleTileBasic[,] grid;
    private SimpleTileBasic selectedTile = null;
    
    // Stats
    public int score = 0;
    public int matches = 0;
    
    // Directional flow system
    private DirectionalFlowSystem flowSystem;
    private DirectionalFlowSystem.FlowDirection lastFlowDirection = DirectionalFlowSystem.FlowDirection.None;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Get directional flow system
        flowSystem = GetComponent<DirectionalFlowSystem>();
        if (flowSystem == null)
        {
            flowSystem = gameObject.AddComponent<DirectionalFlowSystem>();
        }
        
        CreateGrid();
    }
    
    void Update()
    {
        // Simple input detection - works with old input system
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            Debug.Log("🖱️ Click detected!");
            HandleClick();
        }
    }
    
    void HandleClick()
    {
        Vector3 mousePos = UnityEngine.Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        
        Debug.Log($"🎯 Click at world position: {worldPos}");
        
        // Try raycast first
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log($"🎯 Raycast hit: {hit.collider.name}");
            SimpleTileBasic tile = hit.collider.GetComponent<SimpleTileBasic>();
            if (tile != null)
            {
                OnTileClicked(tile);
                return;
            }
        }
        
        // Fallback: find closest tile
        SimpleTileBasic closestTile = FindClosestTile(worldPos);
        if (closestTile != null)
        {
            OnTileClicked(closestTile);
        }
    }
    
    SimpleTileBasic FindClosestTile(Vector3 worldPos)
    {
        float minDistance = float.MaxValue;
        SimpleTileBasic closest = null;
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    float distance = Vector3.Distance(worldPos, grid[x, y].transform.position);
                    if (distance < minDistance && distance < 0.6f)
                    {
                        minDistance = distance;
                        closest = grid[x, y];
                    }
                }
            }
        }
        
        return closest;
    }
    
    void CreateGrid()
    {
        grid = new SimpleTileBasic[gridWidth, gridHeight];
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                CreateTileAt(x, y);
            }
        }
        
        Debug.Log($"✅ Created {gridWidth}x{gridHeight} grid");
    }
    
    void CreateTileAt(int x, int y)
    {
        // Create tile GameObject
        GameObject tileObj = new GameObject($"Tile_{x}_{y}");
        tileObj.transform.parent = transform;
        
        // Position it
        Vector3 position = new Vector3(
            x * tileSpacing - (gridWidth * tileSpacing) / 2f,
            y * tileSpacing - (gridHeight * tileSpacing) / 2f,
            0
        );
        tileObj.transform.position = position;
        
        // Add sprite renderer
        SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSimpleSprite();
        sr.sortingOrder = 1;
        
        // Add collider
        BoxCollider2D collider = tileObj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.9f, 0.9f);
        
        // Add tile component
        SimpleTileBasic tile = tileObj.AddComponent<SimpleTileBasic>();
        tile.Initialize(x, y, sr);
        
        grid[x, y] = tile;
    }
    
    Sprite CreateSimpleSprite()
    {
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
    
    public void OnTileClicked(SimpleTileBasic clickedTile)
    {
        Debug.Log($"🎮 Tile clicked: ({clickedTile.gridX}, {clickedTile.gridY}) - Color: {clickedTile.tileColor}");
        
        if (selectedTile == null)
        {
            // First selection
            SelectTile(clickedTile);
        }
        else if (selectedTile == clickedTile)
        {
            // Same tile - deselect
            DeselectTile();
        }
        else if (IsAdjacent(selectedTile, clickedTile))
        {
            // Adjacent tiles - attempt swap
            AttemptSwap(selectedTile, clickedTile);
            DeselectTile();
        }
        else
        {
            // Different tile - select new one
            DeselectTile();
            SelectTile(clickedTile);
        }
    }
    
    void SelectTile(SimpleTileBasic tile)
    {
        selectedTile = tile;
        tile.SetSelected(true);
        Debug.Log($"✅ Selected tile at ({tile.gridX}, {tile.gridY})");
    }
    
    void DeselectTile()
    {
        if (selectedTile != null)
        {
            selectedTile.SetSelected(false);
            selectedTile = null;
        }
    }
    
    bool IsAdjacent(SimpleTileBasic tile1, SimpleTileBasic tile2)
    {
        int deltaX = Mathf.Abs(tile1.gridX - tile2.gridX);
        int deltaY = Mathf.Abs(tile1.gridY - tile2.gridY);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }
    
    void AttemptSwap(SimpleTileBasic tile1, SimpleTileBasic tile2)
    {
        Debug.Log($"🔄 ATTEMPTING SWAP: ({tile1.gridX}, {tile1.gridY}) ↔ ({tile2.gridX}, {tile2.gridY})");
        
        // Calculate flow direction from selection order
        lastFlowDirection = flowSystem.CalculateFlowDirection(tile1, tile2);
        
        // Swap colors
        TileColor temp = tile1.tileColor;
        tile1.SetColor(tile2.tileColor);
        tile2.SetColor(temp);
        
        // Check for matches
        bool foundMatch = CheckForMatchesEntireBoard();
        
        if (foundMatch)
        {
            score += 100;
            matches++;
            Debug.Log($"🎉 MATCH FOUND! Score: {score}, Total Matches: {matches}");
            
            // Use directional flow to refill board
            StartCoroutine(ProcessMatchesWithDirectionalFlow());
        }
        else
        {
            // No match - swap back
            temp = tile1.tileColor;
            tile1.SetColor(tile2.tileColor);
            tile2.SetColor(temp);
            Debug.Log("❌ No match - swapped back");
        }
    }
    
    // Enhanced cascade detection - checks entire board
    bool CheckForMatchesEntireBoard()
    {
        List<SimpleTileBasic> matchedTiles = new List<SimpleTileBasic>();
        
        // Check ALL horizontal matches
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth - 2; x++)
            {
                if (grid[x, y] != null && grid[x + 1, y] != null && grid[x + 2, y] != null)
                {
                    if (grid[x, y].tileColor == grid[x + 1, y].tileColor && 
                        grid[x + 1, y].tileColor == grid[x + 2, y].tileColor)
                    {
                        AddToList(matchedTiles, grid[x, y]);
                        AddToList(matchedTiles, grid[x + 1, y]);
                        AddToList(matchedTiles, grid[x + 2, y]);
                        
                        // Check for longer matches
                        for (int extend = x + 3; extend < gridWidth; extend++)
                        {
                            if (grid[extend, y] != null && grid[extend, y].tileColor == grid[x, y].tileColor)
                            {
                                AddToList(matchedTiles, grid[extend, y]);
                            }
                            else break;
                        }
                    }
                }
            }
        }
        
        // Check ALL vertical matches
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight - 2; y++)
            {
                if (grid[x, y] != null && grid[x, y + 1] != null && grid[x, y + 2] != null)
                {
                    if (grid[x, y].tileColor == grid[x, y + 1].tileColor && 
                        grid[x, y + 1].tileColor == grid[x, y + 2].tileColor)
                    {
                        AddToList(matchedTiles, grid[x, y]);
                        AddToList(matchedTiles, grid[x, y + 1]);
                        AddToList(matchedTiles, grid[x, y + 2]);
                        
                        // Check for longer matches
                        for (int extend = y + 3; extend < gridHeight; extend++)
                        {
                            if (grid[x, extend] != null && grid[x, extend].tileColor == grid[x, y].tileColor)
                            {
                                AddToList(matchedTiles, grid[x, extend]);
                            }
                            else break;
                        }
                    }
                }
            }
        }
        
        // Process matched tiles
        if (matchedTiles.Count > 0)
        {
            Debug.Log($"🔥 Found {matchedTiles.Count} matched tiles across entire board");
            
            // Highlight matched tiles briefly
            StartCoroutine(HighlightAndClearMatches(matchedTiles));
            return true;
        }
        
        return false;
    }

    // Highlight matches before clearing them
    System.Collections.IEnumerator HighlightAndClearMatches(List<SimpleTileBasic> matchedTiles)
    {
        // Highlight matched tiles
        foreach (SimpleTileBasic tile in matchedTiles)
        {
            if (tile != null && tile.spriteRenderer != null)
            {
                tile.spriteRenderer.color = Color.white; // Bright highlight
                tile.transform.localScale = Vector3.one * 1.2f; // Slightly bigger
            }
        }
        
        yield return new WaitForSeconds(0.5f); // Brief pause to show matches
        
        // Clear matched tiles and mark positions as empty
        List<Vector2Int> emptyPositions = new List<Vector2Int>();
        foreach (SimpleTileBasic tile in matchedTiles)
        {
            if (tile != null)
            {
                emptyPositions.Add(new Vector2Int(tile.gridX, tile.gridY));
                
                // Remove from grid
                grid[tile.gridX, tile.gridY] = null;
                
                // Destroy the tile object
                Destroy(tile.gameObject);
            }
        }
        
        Debug.Log($"🗑️ Cleared {emptyPositions.Count} tiles, preparing directional refill");
        
        // Refill with directional flow
        yield return flowSystem.FlowTilesWithDirection(emptyPositions, lastFlowDirection);
        
        // Check for cascade matches
        if (CheckForMatchesEntireBoard())
        {
            Debug.Log("🔗 Cascade match detected!");
        }
    }

    // Process matches with directional flow
    System.Collections.IEnumerator ProcessMatchesWithDirectionalFlow()
    {
        yield return new WaitForSeconds(0.1f); // Brief delay for player to see the swap
        
        // The CheckForMatchesEntireBoard already started the highlight and clear process
        // This coroutine is just a placeholder for any additional processing
        Debug.Log("🌊 Processing matches with directional flow system");
    }

    // Helper methods for directional flow system
    public SimpleTileBasic CreateTileAtPosition(int gridX, int gridY)
    {
        Vector3 position = GridToWorldPosition(gridX, gridY);
        
        // Create tile GameObject
        GameObject tileObj = new GameObject($"FlowTile_{gridX}_{gridY}");
        tileObj.transform.parent = transform;
        tileObj.transform.position = position;
        
        // Add sprite renderer
        SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSimpleSprite();
        sr.sortingOrder = 1;
        
        // Add collider if within grid bounds
        if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight)
        {
            BoxCollider2D collider = tileObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.9f, 0.9f);
        }
        
        // Add tile component
        SimpleTileBasic tile = tileObj.AddComponent<SimpleTileBasic>();
        tile.Initialize(gridX, gridY, sr);
        
        return tile;
    }

    public Vector3 GridToWorldPosition(int gridX, int gridY)
    {
        return new Vector3(
            gridX * tileSpacing - (gridWidth * tileSpacing) / 2f,
            gridY * tileSpacing - (gridHeight * tileSpacing) / 2f,
            0
        );
    }

    public void SetTileAtGridPosition(int gridX, int gridY, SimpleTileBasic tile)
    {
        if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight)
        {
            grid[gridX, gridY] = tile;
        }
    }
    
    void AddToList(List<SimpleTileBasic> list, SimpleTileBasic tile)
    {
        if (!list.Contains(tile))
            list.Add(tile);
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 250, 120));
        GUILayout.Label($"Score: {score}");
        GUILayout.Label($"Matches: {matches}");
        GUILayout.Label($"Selected: {(selectedTile != null ? $"({selectedTile.gridX}, {selectedTile.gridY})" : "None")}");
        GUILayout.Label($"Last Flow: {lastFlowDirection}");
        
        if (GUILayout.Button("Test Directional Flow"))
        {
            Debug.Log("🧪 Testing directional flow system");
        }
        
        GUILayout.EndArea();
    }
}

// Simple tile color enum
public enum TileColor
{
    Red = 0,
    Blue = 1,
    Green = 2,
    Yellow = 3,
    Purple = 4,
    Orange = 5
}

// Simple tile component
public class SimpleTileBasic : MonoBehaviour
{
    public int gridX, gridY;
    public TileColor tileColor;
    public SpriteRenderer spriteRenderer;
    public bool isSelected = false;
    
    public void Initialize(int x, int y, SpriteRenderer sr)
    {
        gridX = x;
        gridY = y;
        spriteRenderer = sr;
        tileColor = (TileColor)Random.Range(0, 6);
        UpdateVisuals();
    }
    
    public void SetColor(TileColor newColor)
    {
        tileColor = newColor;
        UpdateVisuals();
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals();
    }

    // Add this method for DirectionalFlowSystem
    public void SetPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
    
    void UpdateVisuals()
    {
        if (spriteRenderer == null) return;
        
        // Set color
        Color baseColor = GetColorFromEnum(tileColor);
        
        // Brighten if selected
        if (isSelected)
        {
            baseColor = Color.Lerp(baseColor, Color.white, 0.4f);
            transform.localScale = Vector3.one * 1.1f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
        
        spriteRenderer.color = baseColor;
    }
    
    Color GetColorFromEnum(TileColor color)
    {
        switch (color)
        {
            case TileColor.Red: return Color.red;
            case TileColor.Blue: return Color.blue;
            case TileColor.Green: return Color.green;
            case TileColor.Yellow: return Color.yellow;
            case TileColor.Purple: return Color.magenta;
            case TileColor.Orange: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
} 