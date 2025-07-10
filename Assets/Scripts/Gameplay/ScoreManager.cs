using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using FlowCrush.Core;

namespace FlowCrush.Gameplay
{
    /// <summary>
    /// Manages scoring system with zone-based multipliers and score persistence
    /// Handles match scoring, combos, and high score tracking
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("Scoring Settings")]
        [SerializeField] private int baseScorePerMatch = 100;
        [SerializeField] private float comboMultiplier = 1.5f;
        [SerializeField] private int maxComboBonus = 5;
        [SerializeField] private bool enableZoneMultipliers = true;
        
        [Header("Score Display")]
        [SerializeField] private bool enableScoreAnimation = true;
        [SerializeField] private float scoreAnimationDuration = 0.5f;
        
        [Header("Events")]
        public UnityEvent<int> OnScoreChanged;
        public UnityEvent<int> OnComboChanged;
        public UnityEvent<int> OnHighScoreBeaten;
        
        // Score tracking
        private int currentScore = 0;
        private int highScore = 0;
        private int currentCombo = 0;
        private int maxCombo = 0;
        
        // Match tracking
        private int totalMatches = 0;
        private Dictionary<ZoneType, int> zoneMatchCounts;
        
        // Animation
        private int displayScore = 0;
        private bool isAnimatingScore = false;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            LoadHighScore();
            InitializeZoneTracking();
        }
        
        private void Start()
        {
            // Subscribe to match events
            MatchDetector matchDetector = FindFirstObjectByType<MatchDetector>();
            if (matchDetector != null)
            {
                matchDetector.OnMatchFound.AddListener(HandleMatchFound);
            }
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeZoneTracking()
        {
            zoneMatchCounts = new Dictionary<ZoneType, int>();
            zoneMatchCounts[ZoneType.Edge] = 0;
            zoneMatchCounts[ZoneType.Transition] = 0;
            zoneMatchCounts[ZoneType.Center] = 0;
        }
        
        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("FlowCrush_HighScore", 0);
        }
        
        #endregion
        
        #region Score Management
        
        public void AddScore(Match match)
        {
            if (match == null) return;
            
            // Calculate base score
            int matchScore = CalculateMatchScore(match);
            
            // Apply combo multiplier
            int comboBonus = CalculateComboBonus();
            int totalScore = matchScore + comboBonus;
            
            // Update score
            currentScore += totalScore;
            
            // Update combo
            currentCombo++;
            if (currentCombo > maxCombo)
            {
                maxCombo = currentCombo;
            }
            
            // Update zone tracking
            zoneMatchCounts[match.primaryZone]++;
            totalMatches++;
            
            // Trigger events
            OnScoreChanged?.Invoke(currentScore);
            OnComboChanged?.Invoke(currentCombo);
            
            // Check for high score
            if (currentScore > highScore)
            {
                highScore = currentScore;
                OnHighScoreBeaten?.Invoke(highScore);
                SaveHighScore();
            }
            
            // Animate score display
            if (enableScoreAnimation)
            {
                StartCoroutine(AnimateScoreChange(displayScore, currentScore));
            }
            else
            {
                displayScore = currentScore;
            }
            
            Debug.Log($"Match scored: {matchScore} + {comboBonus} combo = {totalScore} (Total: {currentScore})");
        }
        
        private int CalculateMatchScore(Match match)
        {
            // Base score for match size
            int baseScore = baseScorePerMatch * match.matchSize;
            
            // Apply zone multiplier if enabled
            if (enableZoneMultipliers)
            {
                float zoneMultiplier = GetZoneMultiplier(match.primaryZone);
                baseScore = Mathf.RoundToInt(baseScore * zoneMultiplier);
            }
            
            return baseScore;
        }
        
        private float GetZoneMultiplier(ZoneType zone)
        {
            switch (zone)
            {
                case ZoneType.Edge: return 1.0f;
                case ZoneType.Transition: return 1.5f;
                case ZoneType.Center: return 2.0f;
                default: return 1.0f;
            }
        }
        
        private int CalculateComboBonus()
        {
            if (currentCombo <= 1) return 0;
            
            // Combo bonus increases with each consecutive match
            int comboBonus = Mathf.Min(currentCombo - 1, maxComboBonus);
            return comboBonus * baseScorePerMatch;
        }
        
        public void ResetScore()
        {
            currentScore = 0;
            displayScore = 0;
            currentCombo = 0;
            totalMatches = 0;
            
            // Reset zone tracking
            foreach (ZoneType zone in System.Enum.GetValues(typeof(ZoneType)))
            {
                zoneMatchCounts[zone] = 0;
            }
            
            OnScoreChanged?.Invoke(currentScore);
            OnComboChanged?.Invoke(currentCombo);
            
            Debug.Log("Score reset");
        }
        
        public void ResetCombo()
        {
            currentCombo = 0;
            OnComboChanged?.Invoke(currentCombo);
        }
        
        #endregion
        
        #region Score Animation
        
        private System.Collections.IEnumerator AnimateScoreChange(int fromScore, int toScore)
        {
            if (isAnimatingScore) yield break;
            
            isAnimatingScore = true;
            float elapsedTime = 0f;
            
            while (elapsedTime < scoreAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / scoreAnimationDuration;
                
                displayScore = Mathf.RoundToInt(Mathf.Lerp(fromScore, toScore, progress));
                OnScoreChanged?.Invoke(displayScore);
                
                yield return null;
            }
            
            displayScore = toScore;
            OnScoreChanged?.Invoke(displayScore);
            isAnimatingScore = false;
        }
        
        #endregion
        
        #region High Score Management
        
        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("FlowCrush_HighScore", highScore);
            PlayerPrefs.Save();
        }
        
        public void SaveFinalScore()
        {
            // Save current session stats
            PlayerPrefs.SetInt("FlowCrush_LastScore", currentScore);
            PlayerPrefs.SetInt("FlowCrush_TotalMatches", totalMatches);
            PlayerPrefs.SetInt("FlowCrush_MaxCombo", maxCombo);
            
            // Save zone statistics
            PlayerPrefs.SetInt("FlowCrush_EdgeMatches", zoneMatchCounts[ZoneType.Edge]);
            PlayerPrefs.SetInt("FlowCrush_TransitionMatches", zoneMatchCounts[ZoneType.Transition]);
            PlayerPrefs.SetInt("FlowCrush_CenterMatches", zoneMatchCounts[ZoneType.Center]);
            
            PlayerPrefs.Save();
            
            Debug.Log($"Session saved - Score: {currentScore}, Matches: {totalMatches}, Max Combo: {maxCombo}");
        }
        
        #endregion
        
        #region Event Handlers
        
        private void HandleMatchFound(Match match)
        {
            // This method is called when a match is found
            // The actual scoring is handled by the GameManager
            Debug.Log($"ScoreManager received match: {match.matchSize} tiles of {match.matchColor} in {match.primaryZone} zone");
        }
        
        #endregion
        
        #region Statistics
        
        public int GetCurrentScore()
        {
            return currentScore;
        }
        
        public int GetHighScore()
        {
            return highScore;
        }
        
        public int GetCurrentCombo()
        {
            return currentCombo;
        }
        
        public int GetMaxCombo()
        {
            return maxCombo;
        }
        
        public int GetTotalMatches()
        {
            return totalMatches;
        }
        
        public Dictionary<ZoneType, int> GetZoneMatchCounts()
        {
            return new Dictionary<ZoneType, int>(zoneMatchCounts);
        }
        
        public float GetZoneMatchPercentage(ZoneType zone)
        {
            if (totalMatches == 0) return 0f;
            return (float)zoneMatchCounts[zone] / totalMatches * 100f;
        }
        
        #endregion
        
        #region Public Methods
        
        public void SetScoringSettings(int baseScore, float comboMult, int maxComboBonus)
        {
            this.baseScorePerMatch = baseScore;
            this.comboMultiplier = comboMult;
            this.maxComboBonus = maxComboBonus;
        }
        
        public void EnableZoneMultipliers(bool enable)
        {
            enableZoneMultipliers = enable;
        }
        
        public void EnableScoreAnimation(bool enable)
        {
            enableScoreAnimation = enable;
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Add Test Score")]
        private void DebugAddTestScore()
        {
            // Create a test match
            List<Vector2Int> testPositions = new List<Vector2Int>
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0)
            };
            
            Match testMatch = new Match(testPositions, TileColor.Red, ZoneType.Center);
            AddScore(testMatch);
        }
        
        [ContextMenu("Reset Score")]
        private void DebugResetScore()
        {
            ResetScore();
        }
        
        [ContextMenu("Log Statistics")]
        private void DebugLogStatistics()
        {
            Debug.Log("=== Score Statistics ===");
            Debug.Log($"Current Score: {currentScore}");
            Debug.Log($"High Score: {highScore}");
            Debug.Log($"Current Combo: {currentCombo}");
            Debug.Log($"Max Combo: {maxCombo}");
            Debug.Log($"Total Matches: {totalMatches}");
            Debug.Log($"Edge Matches: {zoneMatchCounts[ZoneType.Edge]} ({GetZoneMatchPercentage(ZoneType.Edge):F1}%)");
            Debug.Log($"Transition Matches: {zoneMatchCounts[ZoneType.Transition]} ({GetZoneMatchPercentage(ZoneType.Transition):F1}%)");
            Debug.Log($"Center Matches: {zoneMatchCounts[ZoneType.Center]} ({GetZoneMatchPercentage(ZoneType.Center):F1}%)");
            Debug.Log("=======================");
        }
        
        #endregion
    }
} 