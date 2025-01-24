using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.UI; // For UI components like Button
using RosMessageTypes.Sensor;

public class KinovaCameraStream : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // Assign the quad's renderer in Unity
    [SerializeField] private Button toggleStreamButton; // Assign a UI Button in Unity
    [SerializeField] private Color activeColor = Color.green; // Color when the button is active
    [SerializeField] private Color inactiveColor = Color.red; // Color when the button is inactive

    private Texture2D texture;
    private bool isStreaming = false; // Tracks whether streaming is enabled
    private Image buttonImage; // Reference to the button's Image component

    void Start()
    {
        // Ensure a Renderer is assigned
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned in the Inspector!");
            return;
        }

        // Ensure the button is assigned
        if (toggleStreamButton == null)
        {
            Debug.LogError("Toggle Stream Button is not assigned in the Inspector!");
            return;
        }

        // Get the button's image component for visual feedback
        buttonImage = toggleStreamButton.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("The Button does not have an Image component!");
            return;
        }

        // Set the button's initial color
        buttonImage.color = inactiveColor;

        // Initialize the button's listener
        toggleStreamButton.onClick.AddListener(ToggleStream);

        // Initialize ROS connection (but do not subscribe yet)
        ROSConnection.GetOrCreateInstance();
    }

    void ToggleStream()
    {
        // Toggle the streaming state
        isStreaming = !isStreaming;

        if (isStreaming)
        {
            // Subscribe to the camera topic
            ROSConnection.GetOrCreateInstance().Subscribe<ImageMsg>("/camera/color/image_rect_color", DisplayImage);
            targetRenderer.material.mainTexture = null; // Clear any old textures
            buttonImage.color = activeColor; // Change button color to active
            Debug.Log("Streaming started.");
        }
        else
        {
            // Stop streaming
            targetRenderer.material.mainTexture = null; // Clear the quad's texture
            buttonImage.color = inactiveColor; // Change button color to inactive
            Debug.Log("Streaming stopped.");
        }
    }

    void DisplayImage(ImageMsg imageMsg)
    {
        // Only process if streaming is enabled
        if (!isStreaming)
            return;

        // Check if texture is already created
        if (texture == null)
        {
            texture = new Texture2D((int)imageMsg.width, (int)imageMsg.height, TextureFormat.RGB24, false);
            targetRenderer.material.mainTexture = texture; // Assign texture to material
        }

        // Validate image data size
        int expectedDataSize = (int)(imageMsg.width * imageMsg.height * 3); // RGB = 3 bytes per pixel
        if (imageMsg.data.Length != expectedDataSize)
        {
            Debug.LogError($"Image data size mismatch! Expected {expectedDataSize} bytes, but got {imageMsg.data.Length} bytes.");
            return;
        }

        // Load image data into the texture
        texture.LoadRawTextureData(imageMsg.data);
        texture.Apply();
    }
}
