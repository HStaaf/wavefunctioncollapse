using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public Texture2D crosshairTexture; // Assign this in the Inspector
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Debug.Log("Setting cursor");
        if (crosshairTexture != null)
        {
            Cursor.SetCursor(crosshairTexture, hotSpot, cursorMode);
            Debug.Log("Cursor set successfully");
        }
        else
        {
            Debug.LogWarning("Crosshair texture is not assigned.");
        }
    }

    void OnDisable()
    {
        Debug.Log("Resetting cursor to default");
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
