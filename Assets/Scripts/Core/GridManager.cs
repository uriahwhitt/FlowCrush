using UnityEngine;
using System.Collections.Generic;

namespace FlowCrush.Core
{
    /// <summary>
    /// Manages the 8x8 game grid with zone-based scoring system
    /// Handles grid initialization, coordinate mapping, and zone definitions
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private int gridWidth = 8;
        [SerializeField] private int gridHeight = 8;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float tileSpacing = 0.1f;
        
        [Header("Zone Visual Settings")]
        [SerializeField] public bool showZoneDebug = true;
        [SerializeField] private Color edgeZoneColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color transitionZoneColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private Color centerZoneColor = new Color(1f, 0f, 0f, 0.3f);
        
        [Header("Prefabs")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject zoneIndicatorPrefab;
        
        // Grid data
        private Tile[,] grid;
        private Transform gridParent;
        private Camera mainCamera;
        private Vector2 gridOffset;
        
        // Zone definitions
        private Dictionary<ZoneType, List<Vector2Int>> zoneTiles;
        
        // Events
        public System.Action<Vector2Int, Tile> OnTilePlaced;
        public System.Action<Vector2Int> OnTileRemoved;
        public System.Action<ZoneType> OnZoneEntered;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            InitializeGrid();
            SetupCamera();
            CreateZoneDefinitions();
        }
        
        private void Start()
        {
            CreateGridVisuals();
            if (showZoneDebug)
            {
                CreateZoneVisuals();
            }
        }
        
        #endregion
        
        #region Grid Initialization
        
        public void InitializeGrid()
        {
            // Create grid data structure
            grid = new Tile[gridWidth, gridHeight];
            zoneTiles = new Dictionary<ZoneType, List<Vector2Int>>();
            
            // Create grid parent object
            GameObject gridParentObj = new GameObject("Grid");
            gridParentObj.transform.SetParent(transform);
            gridParent = gridParentObj.transform;
            
            // Calculate grid offset for centering
            float totalWidth = (gridWidth * tileSize) + ((gridWidth - 1) * tileSpacing);
            float totalHeight = (gridHeight * tileSize) + ((gridHeight - 1) * tileSpacing);
            gridOffset = new Vector2(-totalWidth / 2f, -totalHeight / 2f);
            
            Debug.Log($"Grid initialized: {gridWidth}x{gridHeight}, Size: {totalWidth}x{totalHeight}");
        }
        
        private void SetupCamera()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found! Please ensure scene has a camera tagged as MainCamera.");
                return;
            }
            
            // Calculate camera position to fit grid
            float gridWorldWidth = (gridWidth * tileSize) + ((gridWidth - 1) * tileSpacing);
            float gridWorldHeight = (gridHeight * tileSize) + ((gridHeight - 1) * tileSpacing);
            
            // Add some padding around the grid
            float padding = 2f;
            float requiredOrthographicSize = Mathf.Max(gridWorldWidth, gridWorldHeight) / 2f + padding;
            
            mainCamera.orthographicSize = requiredOrthographicSize;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            
            Debug.Log($"Camera configured: Orthographic size = {requiredOrthographicSize}");
        }
        
        #endregion
        
        #region Zone System
        
        private void CreateZoneDefinitions()
        {
            zoneTiles[ZoneType.Edge] = new List<Vector2Int>();
            zoneTiles[ZoneType.Transition] = new List<Vector2Int>();
            zoneTiles[ZoneType.Center] = new List<Vector2Int>();
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    ZoneType zone = CalculateZoneType(x, y);
                    zoneTiles[zone].Add(gridPos);
                }
            }
            
            Debug.Log($"Zone distribution - Edge: {zoneTiles[ZoneType.Edge].Count}, " +
                     $"Transition: {zoneTiles[ZoneType.Transition].Count}, " +
                     $"Center: {zoneTiles[ZoneType.Center].Count}");
        }
        
        private ZoneType CalculateZoneType(int x, int y)
        {
            // Calculate distance from edge
            int distanceFromEdge = Mathf.Min(
                Mathf.Min(x, gridWidth - 1 - x),
                Mathf.Min(y, gridHeight - 1 - y)
            );
            
            // Zone assignment based on distance from edge
            if (distanceFromEdge == 0) return ZoneType.Edge;           // Outer ring
            if (distanceFromEdge == 1) return ZoneType.Transition;     // Middle ring
            return ZoneType.Center;                                    // Inner area (2+ from edge)
        }
        
        public ZoneType GetZoneType(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
            {
                Debug.LogWarning($"Invalid grid position: ({x}, {y})");
                return ZoneType.Edge; // Default to edge for invalid positions
            }
            
            return CalculateZoneType(x, y);
        }
        
        public ZoneType GetZoneType(Vector2Int position)
        {
            return GetZoneType(position.x, position.y);
        }
        
        public float GetZoneMultiplier(ZoneType zone)
        {
            switch (zone)
            {
                case ZoneType.Edge: return 1f;
                case ZoneType.Transition: return 1.5f;
                case ZoneType.Center: return 2f;
                default: return 1f;
            }
        }
        
        public List<Vector2Int> GetZonePositions(ZoneType zone)
        {
            return zoneTiles.ContainsKey(zone) ? zoneTiles[zone] : new List<Vector2Int>();
        }
        
        #endregion
        
        #region Grid Coordinate System
        
        public Vector3 GridToWorldPosition(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
            {
                Debug.LogWarning($"Invalid grid position: ({x}, {y})");
                return Vector3.zero;
            }
            
            float worldX = gridOffset.x + (x * (tileSize + tileSpacing)) + (tileSize / 2f);
            float worldY = gridOffset.y + (y * (tileSize + tileSpacing)) + (tileSize / 2f);
            
            return new Vector3(worldX, worldY, 0);
        }
        
        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            return GridToWorldPosition(gridPos.x, gridPos.y);
        }
        
        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            // Convert world position back to grid coordinates
            Vector2 localPos = worldPos - transform.position;
            localPos -= gridOffset;
            
            int x = Mathf.FloorToInt((localPos.x + (tileSpacing / 2f)) / (tileSize + tileSpacing));
            int y = Mathf.FloorToInt((localPos.y + (tileSpacing / 2f)) / (tileSize + tileSpacing));
            
            return new Vector2Int(x, y);
        }
        
        public Vector2Int ScreenToGridPosition(Vector2 screenPos)
        {
            if (mainCamera == null) return new Vector2Int(-1, -1);
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
            return WorldToGridPosition(worldPos);
        }
        
        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }
        
        public bool IsValidGridPosition(Vector2Int pos)
        {
            return IsValidGridPosition(pos.x, pos.y);
        }
        
        #endregion
        
        #region Grid Access
        
        public Tile GetTile(int x, int y)
        {
            if (!IsValidGridPosition(x, y)) return null;
            return grid[x, y];
        }
        
        public Tile GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }
        
        public void SetTile(int x, int y, Tile tile)
        {
            if (!IsValidGridPosition(x, y))
            {
                Debug.LogWarning($"Cannot set tile at invalid position: ({x}, {y})");
                return;
            }
            
            // Remove old tile if exists
            if (grid[x, y] != null)
            {
                OnTileRemoved?.Invoke(new Vector2Int(x, y));
            }
            
            grid[x, y] = tile;
            
            if (tile != null)
            {
                tile.transform.position = GridToWorldPosition(x, y);
                tile.SetGridPosition(x, y);
                OnTilePlaced?.Invoke(new Vector2Int(x, y), tile);
            }
        }
        
        public void SetTile(Vector2Int pos, Tile tile)
        {
            SetTile(pos.x, pos.y, tile);
        }
        
        public void ClearTile(int x, int y)
        {
            if (!IsValidGridPosition(x, y)) return;
            
            Tile oldTile = grid[x, y];
            if (oldTile != null)
            {
                OnTileRemoved?.Invoke(new Vector2Int(x, y));
                Destroy(oldTile.gameObject);
            }
            
            grid[x, y] = null;
        }
        
        public void ClearTile(Vector2Int pos)
        {
            ClearTile(pos.x, pos.y);
        }
        
        public bool IsPositionEmpty(int x, int y)
        {
            return IsValidGridPosition(x, y) && grid[x, y] == null;
        }
        
        public bool IsPositionEmpty(Vector2Int pos)
        {
            return IsPositionEmpty(pos.x, pos.y);
        }
        
        public void RegisterTile(Tile tile)
        {
            if (tile == null) return;
            
            Vector2Int gridPos = tile.GetGridPosition();
            
            // Validate position
            if (!IsValidGridPosition(gridPos))
            {
                Debug.LogWarning($"Attempted to register tile at invalid position: {gridPos}");
                return;
            }
            
            // Set the tile in the grid
            SetTile(gridPos, tile);
            
            // Trigger event
            OnTilePlaced?.Invoke(gridPos, tile);
            
            Debug.Log($"Registered tile at position {gridPos}");
        }
        
        #endregion
        
        #region Visual Creation
        
        private void CreateGridVisuals()
        {
            if (tilePrefab == null)
            {
                Debug.LogWarning("No tile prefab assigned! Creating placeholder tiles.");
                CreatePlaceholderTiles();
                return;
            }
            
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    CreateTileAtPosition(x, y);
                }
            }
        }
        
        private void CreateTileAtPosition(int x, int y)
        {
            Vector3 worldPos = GridToWorldPosition(x, y);
            GameObject tileObj = Instantiate(tilePrefab, worldPos, Quaternion.identity, gridParent);
            tileObj.name = $"Tile_{x}_{y}";
            
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile == null)
            {
                tile = tileObj.AddComponent<Tile>();
            }
            
            // Initialize tile with random color for testing
            tile.Initialize(x, y, (TileColor)(Random.Range(0, 6)));
            SetTile(x, y, tile);
        }
        
        private void CreatePlaceholderTiles()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GameObject tileObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tileObj.transform.SetParent(gridParent);
                    tileObj.transform.position = GridToWorldPosition(x, y);
                    tileObj.transform.localScale = Vector3.one * tileSize;
                    tileObj.name = $"Placeholder_Tile_{x}_{y}";
                    
                    // Add basic tile component
                    Tile tile = tileObj.AddComponent<Tile>();
                    tile.Initialize(x, y, (TileColor)(Random.Range(0, 6)));
                    SetTile(x, y, tile);
                    
                    // Random color for visual distinction
                    Renderer renderer = tileObj.GetComponent<Renderer>();
                    renderer.material.color = GetRandomTileColor();
                }
            }
        }
        
        private void CreateZoneVisuals()
        {
            CreateZoneIndicators(ZoneType.Edge, edgeZoneColor);
            CreateZoneIndicators(ZoneType.Transition, transitionZoneColor);
            CreateZoneIndicators(ZoneType.Center, centerZoneColor);
        }
        
        private void CreateZoneIndicators(ZoneType zone, Color color)
        {
            GameObject zoneParent = new GameObject($"Zone_{zone}");
            zoneParent.transform.SetParent(transform);
            
            foreach (Vector2Int pos in zoneTiles[zone])
            {
                GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
                indicator.transform.SetParent(zoneParent.transform);
                indicator.transform.position = GridToWorldPosition(pos.x, pos.y) + Vector3.back * 0.1f;
                indicator.transform.localScale = Vector3.one * (tileSize + tileSpacing);
                indicator.name = $"ZoneIndicator_{zone}_{pos.x}_{pos.y}";
                
                Renderer renderer = indicator.GetComponent<Renderer>();
                renderer.material.color = color;
                
                // Remove collider to avoid interfering with input
                Destroy(indicator.GetComponent<Collider>());
            }
        }
        
        private Color GetRandomTileColor()
        {
            Color[] colors = {
                Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan
            };
            return colors[Random.Range(0, colors.Length)];
        }
        
        #endregion
        
        #region Game Logic Integration
        
        public void ProcessSwipe(SwipeDirection swipeDirection)
        {
            // This will be called by GameManager when a swipe is detected
            // For now, just log the swipe direction
            Debug.Log($"GridManager received swipe: {swipeDirection}");
            
            // TODO: Implement tile movement logic based on swipe direction
            // This will be implemented in Sprint 1 as we build the directional flow system
        }
        
        #endregion
        
        #region Debug and Utilities
        
        [ContextMenu("Toggle Zone Debug")]
        public void ToggleZoneDebug()
        {
            showZoneDebug = !showZoneDebug;
            
            // Find and toggle zone indicator objects
            Transform[] zoneParents = {
                transform.Find("Zone_Edge"),
                transform.Find("Zone_Transition"),
                transform.Find("Zone_Center")
            };
            
            foreach (Transform zoneParent in zoneParents)
            {
                if (zoneParent != null)
                {
                    zoneParent.gameObject.SetActive(showZoneDebug);
                }
            }
        }
        
        [ContextMenu("Log Grid State")]
        public void LogGridState()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Grid State:");
            
            for (int y = gridHeight - 1; y >= 0; y--) // Top to bottom for readability
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    Tile tile = GetTile(x, y);
                    string tileChar = tile != null ? tile.TileColor.ToString().Substring(0, 1) : "X";
                    sb.Append($"[{tileChar}]");
                }
                sb.AppendLine();
            }
            
            Debug.Log(sb.ToString());
        }
        
        [ContextMenu("Test Zone System")]
        public void TestZoneSystem()
        {
            Debug.Log("Testing Zone System:");
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    ZoneType zone = GetZoneType(x, y);
                    Debug.Log($"Position ({x}, {y}) is in {zone} zone");
                }
            }
        }
        
        #endregion
        
        #region Public Properties
        
        public int Width => gridWidth;
        public int Height => gridHeight;
        public float TileSize => tileSize;
        public Vector2 GridOffset => gridOffset;
        public bool ShowZoneDebug => showZoneDebug;
        
        #endregion
    }
} 