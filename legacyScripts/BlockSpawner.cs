using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FlowCrush.Core;
using FlowCrush.Input;

namespace FlowCrush.Gameplay
{
    /// <summary>
    /// Handles spawning new blocks from different directions based on swipe input
    /// Manages block flow and ensures continuous gameplay
    /// </summary>
    public class BlockSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private int blocksPerSwipe = 3;
        [SerializeField] private float spawnDelay = 0.1f;
        [SerializeField] private float blockMoveSpeed = 5f;
        [SerializeField] private bool enableRandomColors = true;
        
        [Header("Spawn Positions")]
        [SerializeField] private Transform topSpawnPoint;
        [SerializeField] private Transform bottomSpawnPoint;
        [SerializeField] private Transform leftSpawnPoint;
        [SerializeField] private Transform rightSpawnPoint;
        
        [Header("Block Prefab")]
        [SerializeField] private GameObject blockPrefab;
        
        [Header("Events")]
        public UnityEvent<Vector2Int, TileColor> OnBlockSpawned;
        public UnityEvent<SwipeDirection> OnBlocksSpawned;
        
        // References
        private GridManager gridManager;
        private Queue<SpawnRequest> spawnQueue;
        
        // Spawn request data structure
        private class SpawnRequest
        {
            public SwipeDirection direction;
            public int blockCount;
            public float delay;
            
            public SpawnRequest(SwipeDirection dir, int count, float delayTime)
            {
                direction = dir;
                blockCount = count;
                delay = delayTime;
            }
        }
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("BlockSpawner: No GridManager found in scene!");
            }
            
            spawnQueue = new Queue<SpawnRequest>();
        }
        
        private void Start()
        {
            // Subscribe to swipe events
            TouchInputManager touchInput = FindFirstObjectByType<TouchInputManager>();
            if (touchInput != null)
            {
                touchInput.OnSwipeDetected.AddListener(HandleSwipeInput);
            }
        }
        
        #endregion
        
        #region Spawn Management
        
        public void SpawnBlocksFromDirection(SwipeDirection direction, int count = -1)
        {
            if (count == -1) count = blocksPerSwipe;
            
            // Queue the spawn request
            SpawnRequest request = new SpawnRequest(direction, count, spawnDelay);
            spawnQueue.Enqueue(request);
            
            // Start processing if not already running
            if (spawnQueue.Count == 1)
            {
                StartCoroutine(ProcessSpawnQueue());
            }
        }
        
        private System.Collections.IEnumerator ProcessSpawnQueue()
        {
            while (spawnQueue.Count > 0)
            {
                SpawnRequest request = spawnQueue.Dequeue();
                
                // Wait for the specified delay
                yield return new WaitForSeconds(request.delay);
                
                // Spawn blocks for this request
                for (int i = 0; i < request.blockCount; i++)
                {
                    SpawnSingleBlock(request.direction);
                    yield return new WaitForSeconds(spawnDelay);
                }
                
                OnBlocksSpawned?.Invoke(request.direction);
            }
        }
        
        private void SpawnSingleBlock(SwipeDirection direction)
        {
            if (gridManager == null) return;
            
            // Determine spawn position and target grid position
            Vector3 spawnPosition = GetSpawnPosition(direction);
            Vector2Int targetGridPos = GetTargetGridPosition(direction);
            
            // Check if target position is available
            if (!gridManager.IsPositionEmpty(targetGridPos))
            {
                // Find next available position in the flow direction
                targetGridPos = FindNextAvailablePosition(targetGridPos, direction);
            }
            
            // Create the block
            GameObject blockObject = CreateBlock(spawnPosition, targetGridPos);
            
            // Start movement coroutine
            StartCoroutine(MoveBlockToPosition(blockObject, targetGridPos));
            
            // Trigger event
            TileColor blockColor = GetRandomTileColor();
            OnBlockSpawned?.Invoke(targetGridPos, blockColor);
            
            Debug.Log($"Spawned block at {targetGridPos} from {direction} direction");
        }
        
        private Vector3 GetSpawnPosition(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Up:
                    return bottomSpawnPoint != null ? bottomSpawnPoint.position : Vector3.zero;
                case SwipeDirection.Down:
                    return topSpawnPoint != null ? topSpawnPoint.position : Vector3.zero;
                case SwipeDirection.Left:
                    return rightSpawnPoint != null ? rightSpawnPoint.position : Vector3.zero;
                case SwipeDirection.Right:
                    return leftSpawnPoint != null ? leftSpawnPoint.position : Vector3.zero;
                default:
                    return Vector3.zero;
            }
        }
        
        private Vector2Int GetTargetGridPosition(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Up:
                    return new Vector2Int(Random.Range(0, gridManager.Width), 0);
                case SwipeDirection.Down:
                    return new Vector2Int(Random.Range(0, gridManager.Width), gridManager.Height - 1);
                case SwipeDirection.Left:
                    return new Vector2Int(gridManager.Width - 1, Random.Range(0, gridManager.Height));
                case SwipeDirection.Right:
                    return new Vector2Int(0, Random.Range(0, gridManager.Height));
                default:
                    return Vector2Int.zero;
            }
        }
        
        private Vector2Int FindNextAvailablePosition(Vector2Int startPos, SwipeDirection direction)
        {
            Vector2Int currentPos = startPos;
            int maxAttempts = 10;
            int attempts = 0;
            
            while (attempts < maxAttempts)
            {
                // Move in the opposite direction of the swipe
                Vector2Int offset = GetFlowOffset(direction);
                currentPos += offset;
                
                // Check bounds
                if (currentPos.x < 0 || currentPos.x >= gridManager.Width ||
                    currentPos.y < 0 || currentPos.y >= gridManager.Height)
                {
                    break;
                }
                
                // Check if position is available
                if (gridManager.IsPositionEmpty(currentPos))
                {
                    return currentPos;
                }
                
                attempts++;
            }
            
            // If no position found, return original position
            return startPos;
        }
        
        private Vector2Int GetFlowOffset(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Up: return new Vector2Int(0, -1);
                case SwipeDirection.Down: return new Vector2Int(0, 1);
                case SwipeDirection.Left: return new Vector2Int(1, 0);
                case SwipeDirection.Right: return new Vector2Int(-1, 0);
                default: return Vector2Int.zero;
            }
        }
        
        private GameObject CreateBlock(Vector3 spawnPosition, Vector2Int gridPosition)
        {
            GameObject blockObject;
            
            if (blockPrefab != null)
            {
                blockObject = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                // Create a simple cube if no prefab is assigned
                blockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blockObject.transform.position = spawnPosition;
                blockObject.transform.localScale = Vector3.one * 0.9f;
            }
            
            // Add Tile component if it doesn't exist
            Tile tileComponent = blockObject.GetComponent<Tile>();
            if (tileComponent == null)
            {
                tileComponent = blockObject.AddComponent<Tile>();
            }
            
            // Set tile properties
            tileComponent.Initialize(gridPosition.x, gridPosition.y, GetRandomTileColor());
            
            return blockObject;
        }
        
        private System.Collections.IEnumerator MoveBlockToPosition(GameObject blockObject, Vector2Int targetGridPos)
        {
            Vector3 targetWorldPos = gridManager.GridToWorldPosition(targetGridPos);
            Vector3 startPos = blockObject.transform.position;
            float distance = Vector3.Distance(startPos, targetWorldPos);
            float duration = distance / blockMoveSpeed;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                
                // Use smooth step for better movement
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                blockObject.transform.position = Vector3.Lerp(startPos, targetWorldPos, smoothT);
                
                yield return null;
            }
            
            // Ensure final position is exact
            blockObject.transform.position = targetWorldPos;
            
            // Register with grid manager
            if (gridManager != null)
            {
                gridManager.RegisterTile(blockObject.GetComponent<Tile>());
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        private TileColor GetRandomTileColor()
        {
            if (!enableRandomColors) return TileColor.Red;
            
            TileColor[] colors = System.Enum.GetValues(typeof(TileColor)) as TileColor[];
            return colors[Random.Range(0, colors.Length)];
        }
        
        private void HandleSwipeInput(SwipeDirection direction)
        {
            // This will be called by the TouchInputManager
            // The actual spawning is handled by the GameManager
        }
        
        #endregion
        
        #region Configuration
        
        public void SetSpawnSettings(int blocksPerSwipe, float spawnDelay, float moveSpeed)
        {
            this.blocksPerSwipe = blocksPerSwipe;
            this.spawnDelay = spawnDelay;
            this.blockMoveSpeed = moveSpeed;
        }
        
        public void SetBlockPrefab(GameObject prefab)
        {
            blockPrefab = prefab;
        }
        
        public void SetSpawnPoints(Transform top, Transform bottom, Transform left, Transform right)
        {
            topSpawnPoint = top;
            bottomSpawnPoint = bottom;
            leftSpawnPoint = left;
            rightSpawnPoint = right;
        }
        
        #endregion
        
        #region Debug Methods
        
        [ContextMenu("Spawn Test Block")]
        private void DebugSpawnTestBlock()
        {
            SpawnBlocksFromDirection(SwipeDirection.Up, 1);
        }
        
        [ContextMenu("Spawn Blocks from All Directions")]
        private void DebugSpawnFromAllDirections()
        {
            SpawnBlocksFromDirection(SwipeDirection.Up, 2);
            SpawnBlocksFromDirection(SwipeDirection.Down, 2);
            SpawnBlocksFromDirection(SwipeDirection.Left, 2);
            SpawnBlocksFromDirection(SwipeDirection.Right, 2);
        }
        
        #endregion
    }
} 