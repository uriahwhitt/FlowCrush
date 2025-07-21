using UnityEngine;

namespace FlowCrush.Simple
{
    public class SimpleTile : MonoBehaviour
    {
        [Header("Tile Properties")]
        public int gridX;
        public int gridY;
        public TileColor tileColor;
        
        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        
        [Header("Debug")]
        public bool isSelected = false;
        
        // Simple color enum
        public enum TileColor
        {
            Red = 0,
            Blue = 1,
            Green = 2,
            Yellow = 3,
            Purple = 4,
            Orange = 5
        }
        
        void Awake()
        {
            // Get sprite renderer if not assigned
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Start()
        {
            // Set random color if not set
            if (tileColor == TileColor.Red && Random.Range(0, 10) > 1) // 90% chance to randomize
            {
                tileColor = (TileColor)Random.Range(0, 6);
            }
            
            UpdateVisuals();
        }
        
        // Handle mouse clicks
        void OnMouseDown()
        {
            Debug.Log($"OnMouseDown called on tile {name} at ({gridX}, {gridY})");
            
            if (SimpleGameManager.Instance != null)
            {
                SimpleGameManager.Instance.OnTileClicked(this);
            }
            else
            {
                Debug.LogWarning("No SimpleGameManager found!");
            }
        }
        
        // Note: OnMouseDown() is the preferred method for click detection
        // The Update() method was removed because it conflicts with Unity's new Input System
        // OnMouseDown() works with both old and new input systems
        
        public void SetColor(TileColor newColor)
        {
            tileColor = newColor;
            UpdateVisuals();
        }
        
        public void SetPosition(int x, int y)
        {
            gridX = x;
            gridY = y;
            name = $"Tile_{x}_{y}";
        }
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateSelectionVisual();
        }
        
        void UpdateVisuals()
        {
            if (spriteRenderer == null) return;
            
            // Simple color mapping
            switch (tileColor)
            {
                case TileColor.Red:    spriteRenderer.color = Color.red; break;
                case TileColor.Blue:   spriteRenderer.color = Color.blue; break;
                case TileColor.Green:  spriteRenderer.color = Color.green; break;
                case TileColor.Yellow: spriteRenderer.color = Color.yellow; break;
                case TileColor.Purple: spriteRenderer.color = Color.magenta; break;
                case TileColor.Orange: spriteRenderer.color = new Color(1f, 0.5f, 0f); break;
            }
        }
        
        void UpdateSelectionVisual()
        {
            if (spriteRenderer == null) return;
            
            // Simple selection feedback - make brighter when selected
            if (isSelected)
            {
                transform.localScale = Vector3.one * 1.1f;
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, 0.3f);
            }
            else
            {
                transform.localScale = Vector3.one;
                UpdateVisuals(); // Reset to normal color
            }
        }
        
        // Helper method to check if tiles are adjacent
        public bool IsAdjacentTo(SimpleTile other)
        {
            if (other == null) return false;
            
            int deltaX = Mathf.Abs(gridX - other.gridX);
            int deltaY = Mathf.Abs(gridY - other.gridY);
            
            // Adjacent means exactly 1 unit away in one direction
            return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
        }
    }
} 