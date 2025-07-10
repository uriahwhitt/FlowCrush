using UnityEngine;

namespace FlowCrush.Utilities
{
    /// <summary>
    /// Disables Adaptive Performance warnings by removing the component
    /// This prevents the "No Provider was configured" warnings
    /// </summary>
    public class AdaptivePerformanceDisabler : MonoBehaviour
    {
        private void Awake()
        {
            // Find and disable Adaptive Performance components
            var adaptivePerformanceComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            
            foreach (var component in adaptivePerformanceComponents)
            {
                if (component.GetType().Name.Contains("AdaptivePerformance"))
                {
                    Debug.Log($"Disabling Adaptive Performance component: {component.GetType().Name}");
                    Destroy(component);
                }
            }
        }
    }
} 