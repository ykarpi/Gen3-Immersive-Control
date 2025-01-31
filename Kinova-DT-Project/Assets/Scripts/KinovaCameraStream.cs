using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.UI;

public class KinovaCameraStream : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // Assign the quad's renderer in Unity
    [SerializeField] private Button StreamOnButton;
    [SerializeField] private Button StreamOffButton;

    private Texture2D texture;
    private bool isStreaming = false;

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned in the Inspector!");
            return;
        }

        if (StreamOnButton == null || StreamOffButton == null)
        {
            Debug.LogError("Stream On/Off buttons are not assigned in the Inspector!");
            return;
        }

        // Assign button listeners
        StreamOnButton.onClick.AddListener(StartStream);
        StreamOffButton.onClick.AddListener(StopStream);

        // Initialize ROS connection
        ROSConnection.GetOrCreateInstance();

        // Ensure the quad is hidden at the start
        targetRenderer.gameObject.SetActive(false);
    }

    void StartStream()
    {
        if (isStreaming)
            return;

        isStreaming = true;
        targetRenderer.gameObject.SetActive(true); // Show the quad

        ROSConnection.GetOrCreateInstance().Subscribe<ImageMsg>("/camera/color/image_rect_color", DisplayImage);
        targetRenderer.material.mainTexture = null; // Clear previous texture
        Debug.Log("Streaming started.");
    }

    void StopStream()
    {
        if (!isStreaming)
            return;

        isStreaming = false;
        ROSConnection.GetOrCreateInstance().Unsubscribe("/camera/color/image_rect_color");

        targetRenderer.material.mainTexture = null; // Clear the displayed texture
        targetRenderer.gameObject.SetActive(false); // Hide the quad
        Debug.Log("Streaming stopped.");
    }

    void DisplayImage(ImageMsg imageMsg)
    {
        if (!isStreaming)
            return;

        if (texture == null || texture.width != (int)imageMsg.width || texture.height != (int)imageMsg.height)
        {
            texture = new Texture2D((int)imageMsg.width, (int)imageMsg.height, TextureFormat.RGB24, false);
            targetRenderer.material.mainTexture = texture;
        }

        int expectedDataSize = (int)(imageMsg.width * imageMsg.height * 3);
        if (imageMsg.data.Length != expectedDataSize)
        {
            Debug.LogError($"Image data size mismatch! Expected {expectedDataSize} bytes, but got {imageMsg.data.Length} bytes.");
            return;
        }

        texture.LoadRawTextureData(imageMsg.data);
        texture.Apply();
    }
}