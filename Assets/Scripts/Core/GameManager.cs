using UnityEngine;
using UnityEngine.Events;
using FlowCrush.Input;
using FlowCrush.Gameplay;

namespace FlowCrush.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private GameState currentGameState = GameState.Menu;
        
        [Header("Game Systems")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private SimpleInputSystemManager touchInputManager;
        [SerializeField] private BlockSpawner blockSpawner;
        [SerializeField] private MatchDetector matchDetector;
        [SerializeField] private PressureSystem pressureSystem;
        [SerializeField] private ScoreManager scoreManager;
        
        [Header("Game Settings")]
        [SerializeField] private float gameStartDelay = 1f;
        [SerializeField] private bool enablePressureSystem = true;
        [SerializeField] private bool enableScoring = true;
        
        [Header("Events")]
        public UnityEvent OnGameStart;
        public UnityEvent OnGamePause;
        public UnityEvent OnGameResume;
        public UnityEvent OnGameOver;
        
        // Singleton pattern for easy access
        public static GameManager Instance { get; private set; }
        
        // Game state enum
        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            GameOver
        }
        
        private void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGameSystems();
        }
        
        private void InitializeGameSystems()
        {
            Debug.Log("GameManager: Initializing game systems...");
            
            // Ensure all systems are properly initialized
            if (gridManager == null) 
            {
                gridManager = FindFirstObjectByType<GridManager>();
                Debug.Log($"GameManager: GridManager found: {gridManager != null}");
            }
            
            if (touchInputManager == null) 
            {
                touchInputManager = FindFirstObjectByType<SimpleInputSystemManager>();
                Debug.Log($"GameManager: SimpleInputSystemManager found: {touchInputManager != null}");
            }
            
            if (blockSpawner == null) 
            {
                blockSpawner = FindFirstObjectByType<BlockSpawner>();
                Debug.Log($"GameManager: BlockSpawner found: {blockSpawner != null}");
            }
            
            if (matchDetector == null) 
            {
                matchDetector = FindFirstObjectByType<MatchDetector>();
                Debug.Log($"GameManager: MatchDetector found: {matchDetector != null}");
            }
            
            if (pressureSystem == null) 
            {
                pressureSystem = FindFirstObjectByType<PressureSystem>();
                Debug.Log($"GameManager: PressureSystem found: {pressureSystem != null}");
            }
            
            if (scoreManager == null) 
            {
                scoreManager = FindFirstObjectByType<ScoreManager>();
                Debug.Log($"GameManager: ScoreManager found: {scoreManager != null}");
            }
            
            // Subscribe to events
            if (touchInputManager != null)
            {
                touchInputManager.OnSwipeDetected.AddListener(HandleSwipeInput);
                Debug.Log("GameManager: Subscribed to swipe events");
            }
            else
            {
                Debug.LogWarning("GameManager: No SimpleInputSystemManager found - input won't work!");
            }
            
            if (matchDetector != null)
            {
                matchDetector.OnMatchFound.AddListener(HandleMatchFound);
                Debug.Log("GameManager: Subscribed to match events");
            }
            else
            {
                Debug.LogWarning("GameManager: No MatchDetector found - matches won't be processed!");
            }
            
            Debug.Log("GameManager: System initialization complete");
        }
        
        public void StartGame()
        {
            if (currentGameState != GameState.Menu) return;
            
            currentGameState = GameState.Playing;
            OnGameStart?.Invoke();
            
            // Initialize game systems
            if (gridManager != null) gridManager.InitializeGrid();
            if (scoreManager != null && enableScoring) scoreManager.ResetScore();
            
            Debug.Log($"Game Started - FlowCrush Sprint 1 (Delay: {gameStartDelay}s)");
        }
        
        public void PauseGame()
        {
            if (currentGameState != GameState.Playing) return;
            
            currentGameState = GameState.Paused;
            OnGamePause?.Invoke();
            Time.timeScale = 0f;
        }
        
        public void ResumeGame()
        {
            if (currentGameState != GameState.Paused) return;
            
            currentGameState = GameState.Playing;
            OnGameResume?.Invoke();
            Time.timeScale = 1f;
        }
        
        public void EndGame()
        {
            currentGameState = GameState.GameOver;
            OnGameOver?.Invoke();
            
            // Save final score
            if (scoreManager != null)
            {
                scoreManager.SaveFinalScore();
            }
        }
        
        public void HandleSwipeInput(SwipeDirection swipeDirection)
        {
            if (currentGameState != GameState.Playing) return;
            
            // Process swipe through game systems
            if (gridManager != null)
            {
                // Handle tile movement based on swipe
                gridManager.ProcessSwipe(swipeDirection);
            }
            
            if (blockSpawner != null)
            {
                // Spawn new blocks from opposite direction
                blockSpawner.SpawnBlocksFromDirection(swipeDirection, 3);
            }
        }
        
        public void HandleMatchFound(Match match)
        {
            if (currentGameState != GameState.Playing) return;
            
            // Process match through scoring system
            if (scoreManager != null)
            {
                scoreManager.AddScore(match);
            }
            
            // Apply pressure system effects
            if (pressureSystem != null && enablePressureSystem)
            {
                pressureSystem.ApplyPressureToMatch(match);
            }
        }
        
        public GameState GetCurrentGameState()
        {
            return currentGameState;
        }
        
        public bool IsGamePlaying()
        {
            return currentGameState == GameState.Playing;
        }
        
        // Debug methods for development
        [ContextMenu("Start Game")]
        public void DebugStartGame()
        {
            StartGame();
        }
        
        [ContextMenu("Pause Game")]
        public void DebugPauseGame()
        {
            PauseGame();
        }
        
        [ContextMenu("Resume Game")]
        public void DebugResumeGame()
        {
            ResumeGame();
        }
    }
} 