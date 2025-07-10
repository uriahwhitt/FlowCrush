using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FlowCrush.Core;

namespace FlowCrush.Gameplay
{
    /// <summary>
    /// Manages the center-flow pressure system that affects gameplay mechanics
    /// Creates pressure zones and applies effects based on match locations
    /// </summary>
    public class PressureSystem : MonoBehaviour
    {
        [Header("Pressure Settings")]
        [SerializeField] private float pressureStrength = 2f;
        [SerializeField] private float pressureUpdateRate = 0.1f;
        [SerializeField] private float maxPressure = 10f;
        [SerializeField] private AnimationCurve pressureCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        
        [Header("Pressure Zones")]
        [SerializeField] private bool enablePressureZones = true;
        [SerializeField] private float zoneInfluenceRadius = 2f;
        [SerializeField] private Color pressureZoneColor = Color.red;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject pressureEffectPrefab;
        [SerializeField] private bool enablePressureParticles = true;
        
        [Header("Events")]
        public UnityEvent<float> OnPressureChanged;
        public UnityEvent<Vector2Int> OnPressureApplied;
        public UnityEvent<ZoneType> OnZonePressureChanged;
        
        // Pressure tracking
        private float currentPressure = 0f;
        private Dictionary<Vector2Int, float> pressureMap;
        private List<Vector2Int> activePressureZones;
        
        // References
        private GridManager gridManager;
        private Camera mainCamera;
        
        // Visual effects
        private List<GameObject> pressureEffects;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            gridManager = FindFirstObjectByType<GridManager>();
            mainCamera = Camera.main;
            
            pressureMap = new Dictionary<Vector2Int, float>();
            activePressureZones = new List<Vector2Int>();
            pressureEffects = new List<GameObject>();
            
            if (gridManager == null)
            {
                Debug.LogError("PressureSystem: No GridManager found in scene!");
            }
        }
        
        private void Start()
        {
            // Subscribe to match events
            MatchDetector matchDetector = FindFirstObjectByType<MatchDetector>();
            if (matchDetector != null)
            {
                matchDetector.OnMatchFound.AddListener(HandleMatchFound);
            }
            
            // Start pressure update coroutine
            StartCoroutine(UpdatePressureSystem());
        }
        
        #endregion
        
        #region Pressure Management
        
        public void ApplyPressureToMatch(Match match)
        {
            if (!enablePressureZones) return;
            
            // Calculate pressure based on match properties
            float matchPressure = CalculateMatchPressure(match);
            
            // Apply pressure to match positions
            foreach (Vector2Int position in match.matchedPositions)
            {
                ApplyPressureToPosition(position, matchPressure);
            }
            
            // Update current pressure
            currentPressure = Mathf.Min(currentPressure + matchPressure, maxPressure);
            OnPressureChanged?.Invoke(currentPressure);
            
            Debug.Log($"Applied pressure {matchPressure} to match at {match.matchedPositions.Count} positions");
        }
        
        private float CalculateMatchPressure(Match match)
        {
            // Base pressure from match size
            float basePressure = match.matchSize * pressureStrength;
            
            // Zone multiplier
            float zoneMultiplier = GetZonePressureMultiplier(match.primaryZone);
            
            // Pressure curve adjustment
            float curveMultiplier = pressureCurve.Evaluate(currentPressure / maxPressure);
            
            return basePressure * zoneMultiplier * curveMultiplier;
        }
        
        private float GetZonePressureMultiplier(ZoneType zone)
        {
            switch (zone)
            {
                case ZoneType.Edge: return 0.5f;
                case ZoneType.Transition: return 1.0f;
                case ZoneType.Center: return 2.0f;
                default: return 1.0f;
            }
        }
        
        private void ApplyPressureToPosition(Vector2Int position, float pressure)
        {
            // Add pressure to the position
            if (pressureMap.ContainsKey(position))
            {
                pressureMap[position] += pressure;
            }
            else
            {
                pressureMap[position] = pressure;
            }
            
            // Clamp pressure
            pressureMap[position] = Mathf.Min(pressureMap[position], maxPressure);
            
            // Add to active zones if not already present
            if (!activePressureZones.Contains(position))
            {
                activePressureZones.Add(position);
            }
            
            // Apply visual effects
            CreatePressureEffect(position, pressure);
            
            OnPressureApplied?.Invoke(position);
        }
        
        private void CreatePressureEffect(Vector2Int position, float pressure)
        {
            if (!enablePressureParticles || pressureEffectPrefab == null) return;
            
            Vector3 worldPosition = gridManager.GridToWorldPosition(position);
            GameObject effect = Instantiate(pressureEffectPrefab, worldPosition, Quaternion.identity);
            
            // Scale effect based on pressure
            float scale = Mathf.Lerp(0.5f, 2f, pressure / maxPressure);
            effect.transform.localScale = Vector3.one * scale;
            
            // Set color based on pressure intensity
            Renderer renderer = effect.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color effectColor = Color.Lerp(Color.yellow, pressureZoneColor, pressure / maxPressure);
                renderer.material.color = effectColor;
            }
            
            pressureEffects.Add(effect);
            
            // Destroy effect after some time
            StartCoroutine(DestroyEffectAfterDelay(effect, 2f));
        }
        
        private System.Collections.IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (effect != null)
            {
                pressureEffects.Remove(effect);
                Destroy(effect);
            }
        }
        
        #endregion
        
        #region Pressure Updates
        
        private System.Collections.IEnumerator UpdatePressureSystem()
        {
            while (true)
            {
                UpdatePressureZones();
                DecayPressure();
                UpdateVisualEffects();
                
                yield return new WaitForSeconds(pressureUpdateRate);
            }
        }
        
        private void UpdatePressureZones()
        {
            // Update pressure influence on surrounding tiles
            List<Vector2Int> zonesToRemove = new List<Vector2Int>();
            
            foreach (Vector2Int zonePosition in activePressureZones)
            {
                // Apply pressure influence to surrounding tiles
                ApplyPressureInfluence(zonePosition);
                
                // Check if pressure has decayed below threshold
                if (pressureMap.ContainsKey(zonePosition) && pressureMap[zonePosition] < 0.1f)
                {
                    zonesToRemove.Add(zonePosition);
                }
            }
            
            // Remove decayed zones
            foreach (Vector2Int position in zonesToRemove)
            {
                activePressureZones.Remove(position);
                pressureMap.Remove(position);
            }
        }
        
        private void ApplyPressureInfluence(Vector2Int centerPosition)
        {
            if (!pressureMap.ContainsKey(centerPosition)) return;
            
            float centerPressure = pressureMap[centerPosition];
            
            // Apply influence to surrounding tiles
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Skip center
                    
                    Vector2Int neighborPos = centerPosition + new Vector2Int(x, y);
                    
                    // Check if position is valid
                    if (!ZoneCalculator.IsValidGridPosition(neighborPos)) continue;
                    
                    // Calculate influence based on distance
                    float distance = Vector2Int.Distance(centerPosition, neighborPos);
                    float influence = centerPressure * (1f - distance / zoneInfluenceRadius);
                    
                    if (influence > 0.1f)
                    {
                        ApplyPressureToPosition(neighborPos, influence * 0.1f); // Reduced influence
                    }
                }
            }
        }
        
        private void DecayPressure()
        {
            // Decay pressure over time
            float decayRate = pressureUpdateRate * 0.5f;
            
            List<Vector2Int> positionsToUpdate = new List<Vector2Int>(pressureMap.Keys);
            
            foreach (Vector2Int position in positionsToUpdate)
            {
                pressureMap[position] -= decayRate;
                
                if (pressureMap[position] <= 0f)
                {
                    pressureMap.Remove(position);
                    activePressureZones.Remove(position);
                }
            }
            
            // Decay overall pressure
            currentPressure = Mathf.Max(0f, currentPressure - decayRate * 0.1f);
        }
        
        private void UpdateVisualEffects()
        {
            // Update visual representation of pressure zones
            foreach (Vector2Int position in activePressureZones)
            {
                if (pressureMap.ContainsKey(position))
                {
                    UpdatePressureVisual(position, pressureMap[position]);
                }
            }
        }
        
        private void UpdatePressureVisual(Vector2Int position, float pressure)
        {
            // Update tile appearance based on pressure
            Tile tile = gridManager.GetTile(position.x, position.y);
            if (tile != null)
            {
                // Apply pressure visual effect to tile
                tile.ApplyPressureEffect(pressure / maxPressure);
            }
        }
        
        #endregion
        
        #region Public Interface
        
        public float GetCurrentPressure()
        {
            return currentPressure;
        }
        
        public float GetPressureAtPosition(Vector2Int position)
        {
            return pressureMap.ContainsKey(position) ? pressureMap[position] : 0f;
        }
        
        public List<Vector2Int> GetActivePressureZones()
        {
            return new List<Vector2Int>(activePressureZones);
        }
        
        public void SetPressureSettings(float strength, float updateRate, float maxPressure)
        {
            this.pressureStrength = strength;
            this.pressureUpdateRate = updateRate;
            this.maxPressure = maxPressure;
        }
        
        public void EnablePressureZones(bool enable)
        {
            enablePressureZones = enable;
        }
        
        public void ResetPressure()
        {
            currentPressure = 0f;
            pressureMap.Clear();
            activePressureZones.Clear();
            
            // Clean up visual effects
            foreach (GameObject effect in pressureEffects)
            {
                if (effect != null)
                {
                    Destroy(effect);
                }
            }
            pressureEffects.Clear();
            
            OnPressureChanged?.Invoke(currentPressure);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void HandleMatchFound(Match match)
        {
            // This will be called by the MatchDetector
            // The actual pressure application is handled by the GameManager
        }
        
        #endregion
        
        #region Debug Methods
        
        [ContextMenu("Apply Test Pressure")]
        private void DebugApplyTestPressure()
        {
            Vector2Int testPosition = new Vector2Int(4, 4);
            ApplyPressureToPosition(testPosition, 5f);
        }
        
        [ContextMenu("Reset Pressure")]
        private void DebugResetPressure()
        {
            ResetPressure();
        }
        
        [ContextMenu("Log Pressure Map")]
        private void DebugLogPressureMap()
        {
            Debug.Log($"Current Pressure: {currentPressure}");
            Debug.Log($"Active Zones: {activePressureZones.Count}");
            foreach (var kvp in pressureMap)
            {
                Debug.Log($"Position {kvp.Key}: Pressure {kvp.Value}");
            }
        }
        
        #endregion
        
        #region Gizmos
        
        private void OnDrawGizmos()
        {
            if (!enablePressureZones) return;
            
            // Draw pressure zones
            foreach (Vector2Int position in activePressureZones)
            {
                if (pressureMap.ContainsKey(position))
                {
                    float pressure = pressureMap[position];
                    float intensity = pressure / maxPressure;
                    
                    Gizmos.color = Color.Lerp(Color.yellow, pressureZoneColor, intensity);
                    Vector3 worldPos = gridManager != null ? gridManager.GridToWorldPosition(position) : Vector3.zero;
                    Gizmos.DrawWireCube(worldPos, Vector3.one * 0.8f);
                }
            }
        }
        
        #endregion
    }
} 