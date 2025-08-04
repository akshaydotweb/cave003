using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SpotlightController : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public Light spotlight; // Assign your Spotlight in the Inspector
    
    [Header("Controller Input")]
    public XRNode controllerNode = XRNode.RightHand; // Which controller to check
    
    [Header("XR Interaction Toolkit Input")]
    public InputActionReference buttonAction; // Assign XRI Default Input Action for primary button
    
    [Header("Fallback Input (for testing)")]
    public Key fallbackKey = Key.F; // Keyboard key for testing
    
    private bool wasAPressed = false;
    private float lastToggleTime = 0f;
    private float toggleCooldown = 0.2f; // Prevent rapid toggling

    void Update()
    {
        // Only check input if cooldown has passed
        if (Time.time - lastToggleTime < toggleCooldown)
            return;
            
        // Method 1: Try XR Interaction Toolkit Input (Preferred)
        if (CheckXRIInput()) return;
        
        // Method 2: Try XR Controller Input (Legacy)
        if (CheckXRInput()) return;
        
        // Method 3: Fallback keyboard input for testing
        if (CheckKeyboardInput()) return;
        
        // Method 4: Try gamepad input
        CheckGamepadInput();
    }
    
    bool CheckXRIInput()
    {
        // Check XR Interaction Toolkit Input Action
        if (buttonAction != null && buttonAction.action != null)
        {
            if (buttonAction.action.WasPressedThisFrame())
            {
                Debug.Log("XRI Input Action button pressed!");
                ToggleSpotlight();
                return true;
            }
        }
        return false;
    }
    
    bool CheckXRInput()
    {
        // Get the input device for the specified controller - specify XR namespace to avoid ambiguity
        UnityEngine.XR.InputDevice controller = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(controllerNode);
        
        if (controller.isValid)
        {
            // Check if the primary button (A button on right controller, X on left) is pressed
            bool isAPressed;
            if (controller.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isAPressed))
            {
                // Only toggle when button is pressed (not held)
                if (isAPressed && !wasAPressed)
                {
                    Debug.Log("XR Primary button pressed!");
                    ToggleSpotlight();
                    wasAPressed = isAPressed;
                    return true;
                }
                wasAPressed = isAPressed;
            }
        }
        // Only log warning occasionally to avoid spam
        else if (Time.frameCount % 120 == 0) // Every 2 seconds at 60fps
        {
            Debug.LogWarning("No valid XR controller found");
        }
        return false;
    }
    
    bool CheckKeyboardInput()
    {
        // New Input System - fallback keyboard input for testing
        if (Keyboard.current != null && Keyboard.current[fallbackKey].wasPressedThisFrame)
        {
            Debug.Log($"Fallback key {fallbackKey} pressed!");
            ToggleSpotlight();
            return true;
        }
        return false;
    }
    
    bool CheckGamepadInput()
    {
        // Try XR Input using new Input System
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame) // A button on Xbox controller
            {
                Debug.Log("New Input System gamepad button pressed!");
                ToggleSpotlight();
                return true;
            }
        }
        return false;
    }

    void ToggleSpotlight()
    {
        if (spotlight != null)
        {
            // Add safety check for light transform
            if (spotlight.transform.position.x == float.PositiveInfinity || 
                spotlight.transform.position.y == float.PositiveInfinity || 
                spotlight.transform.position.z == float.PositiveInfinity ||
                float.IsNaN(spotlight.transform.position.x) ||
                float.IsNaN(spotlight.transform.position.y) ||
                float.IsNaN(spotlight.transform.position.z))
            {
                Debug.LogError("Spotlight has invalid position! Resetting to origin.");
                spotlight.transform.position = Vector3.zero;
                return;
            }
            
            // Set the cooldown timer to prevent rapid toggling
            lastToggleTime = Time.time;
            
            spotlight.enabled = !spotlight.enabled;
            Debug.Log($"Spotlight {(spotlight.enabled ? "ON" : "OFF")} at time: {Time.time}");
        }
        else
        {
            Debug.LogWarning("Spotlight not assigned!");
        }
    }
}
