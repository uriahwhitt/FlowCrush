using UnityEngine;

public class UltraSimpleClickTest : MonoBehaviour
{
    void Update()
    {
        // Most basic input test possible
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("CLICK DETECTED! Input.GetMouseButtonDown(0) is working!");
        }
        
        // Test if any input is happening
        if (Input.anyKeyDown)
        {
            Debug.Log("Some input detected: " + Input.inputString);
        }
    }
    
    void OnGUI()
    {
        // Visual confirmation in game window
        GUI.Label(new Rect(10, 10, 400, 20), "Ultra Simple Click Test - Click anywhere!");
        GUI.Label(new Rect(10, 30, 400, 20), "Mouse Position: " + Input.mousePosition);
        GUI.Label(new Rect(10, 50, 400, 20), "Time: " + Time.time.ToString("F1"));
        
        // Big clickable button for testing
        if (GUI.Button(new Rect(10, 80, 200, 50), "TEST BUTTON"))
        {
            Debug.Log("GUI Button clicked - Unity input is working!");
        }
    }
} 