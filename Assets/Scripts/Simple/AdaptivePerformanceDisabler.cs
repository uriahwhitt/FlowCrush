using UnityEngine;
using System.Collections.Generic;

namespace FlowCrush.Simple
{
    /// <summary>
    /// Disables Adaptive Performance warnings by removing AP components
    /// Add this to any GameObject in your scene
    /// </summary>
    public class AdaptivePerformanceDisabler : MonoBehaviour
    {
        [Header("Auto Disable")]
        [SerializeField] private bool autoDisableOnStart = true;
        
        void Start()
        {
            if (autoDisableOnStart)
            {
                DisableAdaptivePerformance();
                Destroy(this); // Remove this component after setup
            }
        }
        
        [ContextMenu("Disable Adaptive Performance")]
        public void DisableAdaptivePerformance()
        {
            Debug.Log("Disabling Adaptive Performance components...");
            
            // Find all Adaptive Performance components and remove them
            var apComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            int removedCount = 0;
            
            foreach (var component in apComponents)
            {
                if (component.GetType().Name.Contains("AdaptivePerformance"))
                {
                    Debug.Log($"Removing Adaptive Performance component: {component.GetType().Name}");
                    DestroyImmediate(component);
                    removedCount++;
                }
            }
            
            Debug.Log($"✅ Removed {removedCount} Adaptive Performance components");
        }
    }
} 