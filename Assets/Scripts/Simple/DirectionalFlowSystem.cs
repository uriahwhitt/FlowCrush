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