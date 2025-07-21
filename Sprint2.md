🎯 SPRINT 2 PLANNING SESSION
Sprint 2 Goal:

"Implement the revolutionary directional block flow system with smooth animations and complete cascade detection - making FlowCrush feel truly unique"

Sprint Duration: 2 weeks
Success Criteria: Game feels fundamentally different from traditional match-3 through directional control

📋 SPRINT 2 BACKLOG
🔥 HIGHEST PRIORITY - Core Innovation
Epic 3: Directional Block Flow (Revolutionary Feature)

User Story 3.1: Implement Selection-to-Direction Mapping

"As a player, when I select tiles in a direction, new blocks flow from the opposite edge"
Story Points: 8
Acceptance Criteria:

First tile → Second tile selection determines flow direction
New tiles spawn from opposite edge (Up selection = blocks from bottom)
Visual feedback shows flow direction
Works for all 4 cardinal directions




User Story 3.2: Smooth Block Flow Animation

"As a player, I want to see blocks smoothly flow into position so I understand the mechanic"
Story Points: 5
Acceptance Criteria:

Blocks animate from spawn edge to final position
60 FPS smooth movement
Clear visual trail showing flow direction
Animation completes in <1 second





🐛 CRITICAL FIXES
Fix 1: Complete Board Cascade Detection

Story Points: 5
Problem: Currently only checks matches at swap location
Solution: Check entire board for matches after any change
Acceptance Criteria:

All 3+ matches detected and cleared regardless of location
Cascade matches trigger automatically
Match detection performance <16ms (60 FPS maintained)



Fix 2: Enhanced Match Feedback

Story Points: 3
Problem: Matches happen too fast to see
Solution: Add brief highlight/pause before clearing
Acceptance Criteria:

Matched tiles highlight for 0.5 seconds
Clear visual indication of which tiles matched
Satisfying disappear animation



🎮 STRATEGIC DEPTH
Epic 4: Basic Center-Flow Pressure (Phase 1)

User Story 4.1: Implement Pressure Gradient

"As a player, I want tiles to naturally drift toward center zones for strategic planning"
Story Points: 8
Acceptance Criteria:

Subtle center-ward force on all tiles
Effect is noticeable but not overwhelming
Pressure increases toward center zones
Visual indicator shows pressure direction




User Story 4.2: Zone-Based Scoring Multipliers

"As a player, I want center matches to be more valuable to encourage strategic play"
Story Points: 3
Acceptance Criteria:

Edge zone matches: 1x points
Transition zone matches: 1.5x points
Center zone matches: 2x points
Clear UI feedback showing zone bonuses


🚀 SPRINT 2 SETUP INSTRUCTIONS
Step 1: Add the New System (5 minutes)

Add DirectionalFlowSystem.cs to your project
Update SimpleFlowCrushManager.cs with the changes above
The system will auto-attach when you start the game

Step 2: Test Directional Flow (10 minutes)

Press Play
Make a tile swap (click tile A, then adjacent tile B)
Watch console for direction detection:
🧭 Selection vector: (1, 0)
🎯 Player swiped RIGHT → Blocks flow FROM LEFT
🌊 Starting directional flow: FromLeft for 3 positions

Watch tiles flow in smoothly from the opposite edge

Step 3: Verify Complete Board Detection

Look for matches anywhere on board - they should all be detected
Watch for cascade matches after directional refill
Check that match highlighting shows before clearing

🎯 EXPECTED RESULTS:
When you make a match, you should see:

Direction calculation based on your selection order
Brief highlight of matched tiles (0.5 seconds)
Smooth animation of new tiles flowing from opposite edge
Yellow arrow indicator showing flow direction
All matches detected regardless of board position

📊 Sprint 2 Success Metrics:

✅ Directional flow working - tiles flow from opposite edge of selection
✅ Smooth animations - 60 FPS tile movement
✅ Complete cascade detection - all matches found
✅ Enhanced feedback - clear visual indication of matches
✅ Game feels unique - different from traditional match-3

Ready to test the revolutionary directional flow system? This is the core innovation that makes FlowCrush unique! 🌊✨

Below is my plan and rough code modifications for Sprint 2:



🛠️ SPRINT 2 IMPLEMENTATION PLAN
Week 1 Focus: Directional Flow Core
Day 1-2: Selection Direction Detection

*** DIRECTIONALFLOWSYSTEM.CS  ***
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionalFlowSystem : MonoBehaviour
{
    [Header("Flow Settings")]
    public float flowSpeed = 2.0f;
    public float flowDelay = 0.1f; // Time between each tile flowing
    public bool showDirectionIndicators = true;
    
    [Header("Direction Visual Feedback")]
    public GameObject directionArrowPrefab;
    private GameObject currentDirectionArrow;
    
    // Flow direction enum
    public enum FlowDirection
    {
        None,
        FromTop,    // Tiles flow from top edge (swipe was DOWN)
        FromBottom, // Tiles flow from bottom edge (swipe was UP)
        FromLeft,   // Tiles flow from left edge (swipe was RIGHT)
        FromRight   // Tiles flow from right edge (swipe was LEFT)
    }
    
    private SimpleFlowCrushManager gameManager;
    
    void Start()
    {
        gameManager = GetComponent<SimpleFlowCrushManager>();
    }
    
    // Calculate flow direction based on selection order
    public FlowDirection CalculateFlowDirection(SimpleTileBasic firstTile, SimpleTileBasic secondTile)
    {
        if (firstTile == null || secondTile == null) return FlowDirection.None;
        
        int deltaX = secondTile.gridX - firstTile.gridX;
        int deltaY = secondTile.gridY - firstTile.gridY;
        
        Debug.Log($"🧭 Selection vector: ({deltaX}, {deltaY})");
        
        // Determine primary direction based on larger delta
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            // Horizontal movement
            if (deltaX > 0)
            {
                Debug.Log("🎯 Player swiped RIGHT → Blocks flow FROM LEFT");
                return FlowDirection.FromLeft;
            }
            else
            {
                Debug.Log("🎯 Player swiped LEFT → Blocks flow FROM RIGHT");
                return FlowDirection.FromRight;
            }
        }
        else
        {
            // Vertical movement
            if (deltaY > 0)
            {
                Debug.Log("🎯 Player swiped UP → Blocks flow FROM BOTTOM");
                return FlowDirection.FromBottom;
            }
            else
            {
                Debug.Log("🎯 Player swiped DOWN → Blocks flow FROM TOP");
                return FlowDirection.FromTop;
            }
        }
    }
    
    // Get spawn positions for new tiles based on flow direction
    public List<Vector2Int> GetSpawnPositions(FlowDirection direction, int numTilesToSpawn)
    {
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        
        switch (direction)
        {
            case FlowDirection.FromTop:
                // Spawn from top edge (y = gridHeight - 1)
                for (int i = 0; i < numTilesToSpawn; i++)
                {
                    int x = Random.Range(0, gameManager.gridWidth);
                    spawnPositions.Add(new Vector2Int(x, gameManager.gridHeight));
                }
                break;
                
            case FlowDirection.FromBottom:
                // Spawn from bottom edge (y = 0)
                for (int i = 0; i < numTilesToSpawn; i++)
                {
                    int x = Random.Range(0, gameManager.gridWidth);
                    spawnPositions.Add(new Vector2Int(x, -1));
                }
                break;
                
            case FlowDirection.FromLeft:
                // Spawn from left edge (x = 0)
                for (int i = 0; i < numTilesToSpawn; i++)
                {
                    int y = Random.Range(0, gameManager.gridHeight);
                    spawnPositions.Add(new Vector2Int(-1, y));
                }
                break;
                
            case FlowDirection.FromRight:
                // Spawn from right edge (x = gridWidth - 1)
                for (int i = 0; i < numTilesToSpawn; i++)
                {
                    int y = Random.Range(0, gameManager.gridHeight);
                    spawnPositions.Add(new Vector2Int(gameManager.gridWidth, y));
                }
                break;
        }
        
        Debug.Log($"🎲 Generated {spawnPositions.Count} spawn positions for {direction}");
        return spawnPositions;
    }
    
    // Get destination positions for flowing tiles
    public List<Vector2Int> GetDestinationPositions(List<Vector2Int> emptyPositions, FlowDirection direction)
    {
        // Sort empty positions based on flow direction priority
        List<Vector2Int> destinations = new List<Vector2Int>(emptyPositions);
        
        switch (direction)
        {
            case FlowDirection.FromTop:
                // Fill from top to bottom
                destinations.Sort((a, b) => b.y.CompareTo(a.y));
                break;
                
            case FlowDirection.FromBottom:
                // Fill from bottom to top
                destinations.Sort((a, b) => a.y.CompareTo(b.y));
                break;
                
            case FlowDirection.FromLeft:
                // Fill from left to right
                destinations.Sort((a, b) => a.x.CompareTo(b.x));
                break;
                
            case FlowDirection.FromRight:
                // Fill from right to left
                destinations.Sort((a, b) => b.x.CompareTo(a.x));
                break;
        }
        
        return destinations;
    }
    
    // Animate tiles flowing from spawn to destination
    public IEnumerator FlowTilesWithDirection(List<Vector2Int> emptyPositions, FlowDirection direction)
    {
        Debug.Log($"🌊 Starting directional flow: {direction} for {emptyPositions.Count} positions");
        
        // Show direction indicator
        ShowDirectionIndicator(direction);
        
        // Get spawn and destination positions
        List<Vector2Int> spawnPositions = GetSpawnPositions(direction, emptyPositions.Count);
        List<Vector2Int> destinations = GetDestinationPositions(emptyPositions, direction);
        
        // Create tiles at spawn positions
        List<SimpleTileBasic> flowingTiles = new List<SimpleTileBasic>();
        for (int i = 0; i < spawnPositions.Count && i < destinations.Count; i++)
        {
            Vector2Int spawnPos = spawnPositions[i];
            Vector2Int destPos = destinations[i];
            
            // Create tile at spawn position
            SimpleTileBasic newTile = gameManager.CreateTileAtPosition(spawnPos.x, spawnPos.y);
            flowingTiles.Add(newTile);
            
            // Start flowing animation with delay
            StartCoroutine(FlowSingleTile(newTile, spawnPos, destPos, i * flowDelay));
        }
        
        // Wait for all tiles to finish flowing
        float totalFlowTime = flowSpeed + (flowingTiles.Count * flowDelay);
        yield return new WaitForSeconds(totalFlowTime);
        
        // Hide direction indicator
        HideDirectionIndicator();
        
        Debug.Log("🎉 Directional flow complete!");
    }
    
    // Animate single tile flowing from spawn to destination
    private IEnumerator FlowSingleTile(SimpleTileBasic tile, Vector2Int spawnPos, Vector2Int destPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Vector3 startWorldPos = gameManager.GridToWorldPosition(spawnPos.x, spawnPos.y);
        Vector3 endWorldPos = gameManager.GridToWorldPosition(destPos.x, destPos.y);
        
        tile.transform.position = startWorldPos;
        
        // Update grid reference
        gameManager.SetTileAtGridPosition(destPos.x, destPos.y, tile);
        tile.SetPosition(destPos.x, destPos.y);
        
        // Smooth movement animation
        float elapsedTime = 0;
        while (elapsedTime < flowSpeed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / flowSpeed;
            
            // Smooth curve for more natural movement
            progress = Mathf.SmoothStep(0, 1, progress);
            
            tile.transform.position = Vector3.Lerp(startWorldPos, endWorldPos, progress);
            yield return null;
        }
        
        // Ensure final position is exact
        tile.transform.position = endWorldPos;
        
        Debug.Log($"🎯 Tile flowed to ({destPos.x}, {destPos.y})");
    }
    
    // Show visual indicator of flow direction
    void ShowDirectionIndicator(FlowDirection direction)
    {
        if (!showDirectionIndicators) return;
        
        // Create simple direction indicator
        if (currentDirectionArrow == null)
        {
            currentDirectionArrow = new GameObject("DirectionIndicator");
            SpriteRenderer sr = currentDirectionArrow.AddComponent<SpriteRenderer>();
            sr.sprite = CreateArrowSprite();
            sr.color = Color.yellow;
            sr.sortingOrder = 10;
        }
        
        // Position and rotate arrow based on direction
        Vector3 position = Vector3.zero;
        float rotation = 0f;
        
        switch (direction)
        {
            case FlowDirection.FromTop:
                position = new Vector3(0, 3, 0);
                rotation = 180f; // Arrow pointing down
                break;
            case FlowDirection.FromBottom:
                position = new Vector3(0, -3, 0);
                rotation = 0f; // Arrow pointing up
                break;
            case FlowDirection.FromLeft:
                position = new Vector3(-3, 0, 0);
                rotation = 90f; // Arrow pointing right
                break;
            case FlowDirection.FromRight:
                position = new Vector3(3, 0, 0);
                rotation = -90f; // Arrow pointing left
                break;
        }
        
        currentDirectionArrow.transform.position = position;
        currentDirectionArrow.transform.rotation = Quaternion.Euler(0, 0, rotation);
        currentDirectionArrow.SetActive(true);
        
        Debug.Log($"🏹 Showing direction indicator: {direction}");
    }
    
    void HideDirectionIndicator()
    {
        if (currentDirectionArrow != null)
        {
            currentDirectionArrow.SetActive(false);
        }
    }
    
    // Create simple arrow sprite
    Sprite CreateArrowSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        
        // Simple arrow pattern (pointing up)
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                pixels[y * 32 + x] = Color.clear;
                
                // Arrow shape
                if (y > 16)
                {
                    // Arrow head
                    int distance = Mathf.Abs(x - 16);
                    if (distance <= (y - 16))
                    {
                        pixels[y * 32 + x] = Color.white;
                    }
                }
                else if (y < 16 && x >= 14 && x <= 18)
                {
                    // Arrow shaft
                    pixels[y * 32 + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
}



Day 3-4: Integration with Game Manager
Now let's update the main game manager to use directional flow:
And add helper methods:
Add stats back to game:

*** SimpleFlowCrushManager.cs ***

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
        mousePos.z = Camera.main.nearClipPlane; // Important for orthographic cameras
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        
        Debug.Log($"🎯 Click at screen: {UnityEngine.Input.mousePosition}, world: {worldPos}");
        
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
    
    bool CheckForMatches()
    {
        List<SimpleTileBasic> matchedTiles = new List<SimpleTileBasic>();
        
        // Check horizontal matches
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth - 2; x++)
            {
                if (grid[x, y].tileColor == grid[x + 1, y].tileColor && 
                    grid[x + 1, y].tileColor == grid[x + 2, y].tileColor)
                {
                    AddToList(matchedTiles, grid[x, y]);
                    AddToList(matchedTiles, grid[x + 1, y]);
                    AddToList(matchedTiles, grid[x + 2, y]);
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
                    AddToList(matchedTiles, grid[x, y]);
                    AddToList(matchedTiles, grid[x, y + 1]);
                    AddToList(matchedTiles, grid[x, y + 2]);
                }
            }
        }
        
        // Replace matched tiles
        if (matchedTiles.Count > 0)
        {
            foreach (SimpleTileBasic tile in matchedTiles)
            {
                tile.SetColor((TileColor)Random.Range(0, 6));
            }
            Debug.Log($"🔥 Replaced {matchedTiles.Count} matched tiles");
            return true;
        }
        
        return false;
    }
    
    void AddToList(List<SimpleTileBasic> list, SimpleTileBasic tile)
    {
        if (!list.Contains(tile))
            list.Add(tile);
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