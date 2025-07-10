using UnityEngine;

namespace FlowCrush.Core
{
    /// <summary>
    /// Represents an individual tile in the game grid
    /// Handles tile state, visual representation, and animations
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [Header("Tile Configuration")]
        [SerializeField] private TileColor tileColor = TileColor.Red;
        [SerializeField] private bool isMatched = false;
        [SerializeField] private bool isMoving = false;
        
        [Header("Visual Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem matchEffect;
        [SerializeField] private float animationDuration = 0.3f;
        
        // Grid position
        private Vector2Int gridPosition;
        private Vector3 targetPosition;
        
        // Color data - using the colors from DataStructures for consistency
        private static readonly Color[] tileColors = {
            new Color(1f, 0.2f, 0.2f),    // Red
            new Color(0.2f, 0.5f, 1f),    // Blue  
            new Color(0.2f, 1f, 0.2f),    // Green
            new Color(1f, 1f, 0.2f),      // Yellow
            new Color(1f, 0.2f, 1f),      // Purple (Magenta)
            new Color(1f, 0.5f, 0.2f)     // Orange (Cyan)
        };
        
        // Events
        public System.Action<Tile> OnTileMatched;
        public System.Action<Tile> OnTileDestroyed;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Check if this GameObject was created as a primitive (has MeshFilter)
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                // This is a primitive GameObject, use the existing MeshRenderer instead of SpriteRenderer
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Use the mesh renderer's material for coloring
                    meshRenderer.material.color = GetColorForTileType(tileColor);
                    Debug.Log($"Tile {gameObject.name}: Using MeshRenderer (primitive tile)");
                }
                return; // Skip SpriteRenderer setup for primitive tiles
            }
            
            // Get or create sprite renderer for non-primitive tiles
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
            
            // Create basic sprite if none exists
            if (spriteRenderer != null && spriteRenderer.sprite == null)
            {
                CreateBasicSprite();
            }
        }
        
        private void Start()
        {
            UpdateVisual();
        }
        
        private void Update()
        {
            // Handle smooth movement to target position
            if (isMoving && Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / animationDuration);
            }
            else if (isMoving)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
        
        #endregion
        
        #region Initialization
        
        public void Initialize(int x, int y, TileColor color)
        {
            gridPosition = new Vector2Int(x, y);
            tileColor = color;
            isMatched = false;
            isMoving = false;
            
            UpdateVisual();
            
            // Set name for debugging
            gameObject.name = $"Tile_{x}_{y}_{color}";
        }
        
        private void CreateBasicSprite()
        {
            // Create a basic square sprite if none is provided
            Texture2D texture = new Texture2D(64, 64);
            Color32[] pixels = new Color32[64 * 64];
            
            // Fill with white color (will be tinted by sprite renderer)
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            
            texture.SetPixels32(pixels);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            spriteRenderer.sprite = sprite;
        }
        
        #endregion
        
        #region Visual Updates
        
        private void UpdateVisual()
        {
            // Handle primitive tiles (MeshRenderer)
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Color baseColor = GetColorForTileType(tileColor);
                if (isMatched)
                {
                    meshRenderer.material.color = Color.Lerp(baseColor, Color.white, 0.5f);
                }
                else
                {
                    meshRenderer.material.color = baseColor;
                }
                
                // Update scale based on state
                transform.localScale = isMatched ? Vector3.one * 1.1f : Vector3.one;
                return;
            }
            
            // Handle sprite tiles (SpriteRenderer)
            if (spriteRenderer == null) return;
            
            // Update color based on tile type
            spriteRenderer.color = GetColorForTileType(tileColor);
            
            // Update visual state based on matched status
            if (isMatched)
            {
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, 0.5f);
            }
            
            // Update scale based on state
            transform.localScale = isMatched ? Vector3.one * 1.1f : Vector3.one;
        }
        
        private Color GetColorForTileType(TileColor color)
        {
            int colorIndex = (int)color;
            if (colorIndex >= 0 && colorIndex < tileColors.Length)
            {
                return tileColors[colorIndex];
            }
            return Color.white;
        }
        
        public void SetHighlight(bool highlighted)
        {
            // Handle primitive tiles (MeshRenderer)
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Color meshColor = GetColorForTileType(tileColor);
                meshRenderer.material.color = highlighted ? Color.Lerp(meshColor, Color.white, 0.3f) : meshColor;
                return;
            }
            
            // Handle sprite tiles (SpriteRenderer)
            if (spriteRenderer == null) return;
            
            Color spriteColor = GetColorForTileType(tileColor);
            spriteRenderer.color = highlighted ? Color.Lerp(spriteColor, Color.white, 0.3f) : spriteColor;
        }
        
        #endregion
        
        #region Grid Position Management
        
        public void SetGridPosition(int x, int y)
        {
            gridPosition = new Vector2Int(x, y);
            gameObject.name = $"Tile_{x}_{y}_{tileColor}";
        }
        
        public void SetGridPosition(Vector2Int position)
        {
            SetGridPosition(position.x, position.y);
        }
        
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        
        #endregion
        
        #region Movement and Animation
        
        public void MoveTo(Vector3 worldPosition, bool animate = true)
        {
            if (animate)
            {
                targetPosition = worldPosition;
                isMoving = true;
            }
            else
            {
                transform.position = worldPosition;
                targetPosition = worldPosition;
                isMoving = false;
            }
        }
        
        public void MoveTo(Vector3 worldPosition, float duration)
        {
            animationDuration = duration;
            MoveTo(worldPosition, true);
        }
        
        public bool IsMoving()
        {
            return isMoving;
        }
        
        public void StopMovement()
        {
            isMoving = false;
            transform.position = targetPosition;
        }
        
        #endregion
        
        #region Matching System
        
        public void SetMatched(bool matched)
        {
            isMatched = matched;
            UpdateVisual();
            
            if (matched)
            {
                OnTileMatched?.Invoke(this);
                if (matchEffect != null)
                {
                    matchEffect.Play();
                }
            }
        }
        
        public bool IsMatched()
        {
            return isMatched;
        }
        
        public bool CanMatch(Tile other)
        {
            if (other == null) return false;
            return tileColor == other.tileColor && !isMatched && !other.isMatched;
        }
        
        #endregion
        
        #region Tile Properties
        
        public TileColor TileColor
        {
            get => tileColor;
            set
            {
                tileColor = value;
                UpdateVisual();
            }
        }
        
        public bool IsEmpty => false; // For future empty tile support
        
        #endregion
        
        #region Animation Effects
        
        public void PlayMatchEffect()
        {
            if (matchEffect != null)
            {
                matchEffect.Play();
            }
            
            // Simple scale animation for match feedback
            StartCoroutine(MatchScaleAnimation());
        }
        
        private System.Collections.IEnumerator MatchScaleAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
            
            float elapsedTime = 0f;
            float duration = 0.2f;
            
            // Scale up
            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            elapsedTime = 0f;
            
            // Scale back down
            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        public void PlayDestroyEffect()
        {
            // Add particle effect and scale down animation
            StartCoroutine(DestroyAnimation());
        }
        
        private System.Collections.IEnumerator DestroyAnimation()
        {
            float duration = 0.3f;
            float elapsedTime = 0f;
            Vector3 originalScale = transform.localScale;
            
            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.clear, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Notify destruction
            OnTileDestroyed?.Invoke(this);
            
            // Tile is now ready to be removed/recycled
            gameObject.SetActive(false);
        }
        
        #endregion
        
        #region Pressure Effects
        
        public void ApplyPressureEffect(float pressureIntensity)
        {
            if (spriteRenderer == null) return;
            
            // Apply pressure visual effect
            Color baseColor = GetColorForTileType(tileColor);
            Color pressureColor = Color.Lerp(baseColor, Color.red, pressureIntensity * 0.5f);
            
            // Add glow effect
            spriteRenderer.color = pressureColor;
            
            // Scale effect
            float scaleMultiplier = 1f + (pressureIntensity * 0.2f);
            transform.localScale = Vector3.one * scaleMultiplier;
            
            // Start pressure animation
            StartCoroutine(PressureAnimation(pressureIntensity));
        }
        
        private System.Collections.IEnumerator PressureAnimation(float intensity)
        {
            Vector3 originalScale = transform.localScale;
            float duration = 0.5f;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float pulse = Mathf.Sin(t * Mathf.PI * 4) * 0.1f * intensity;
                
                transform.localScale = originalScale * (1f + pulse);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Change Color")]
        private void DebugChangeColor()
        {
            int currentColor = (int)tileColor;
            currentColor = (currentColor + 1) % System.Enum.GetValues(typeof(TileColor)).Length;
            TileColor = (TileColor)currentColor;
        }
        
        [ContextMenu("Test Match Effect")]
        private void DebugTestMatchEffect()
        {
            PlayMatchEffect();
        }
        
        [ContextMenu("Test Destroy Effect")]
        private void DebugTestDestroyEffect()
        {
            PlayDestroyEffect();
        }
        
        #endregion
    }
} 