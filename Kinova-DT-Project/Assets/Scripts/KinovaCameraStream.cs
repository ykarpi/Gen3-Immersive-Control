using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using RosMessageTypes.Sensor;

public class KinovaCameraStream : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // Assign a material's renderer in Unity
    private Texture2D texture;

    void Start()
    {
        // Ensure a Renderer is assigned
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned in the Inspector!");
            return;
        }

        // Initialize ROS connection and subscribe to the camera topic
        ROSConnection.GetOrCreateInstance().Subscribe<ImageMsg>("/camera/color/image_rect_color", DisplayImage);
    }

    void DisplayImage(ImageMsg imageMsg)
    {
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
