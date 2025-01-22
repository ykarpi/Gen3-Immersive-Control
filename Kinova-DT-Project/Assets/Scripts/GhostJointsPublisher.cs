using UnityEngine;
using UnityEngine.UI; // For UI components like Slider and Button
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class GhostJointsPublisher : MonoBehaviour
{
    private ROSConnection rosConnection; // Reference to your ROS connection

    [SerializeField]
    private Button publishButton; // Reference to the Unity button

    [SerializeField]
    private Slider[] jointSliders; // Array of 6 sliders for joint angles

    [SerializeField]
    private float[] actuatorLimitsMin = new float[] { -180f, -128.9f, -147.8f, -180f, -120.3f, -180f }; // Min angle for each joint (in degrees)

    [SerializeField]
    private float[] actuatorLimitsMax = new float[] { 180f, 128.9f, 147.8f, 180f, 120.3f, 180f }; // Max angle for each joint (in degrees)

    private string jointPositionsTopic = "/unity/my_gen3/joint_positions"; // Topic to send the joint positions to

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        // Register the publisher with the topic
        rosConnection.RegisterPublisher<Float64MultiArrayMsg>(jointPositionsTopic);
        // Add listener to the button's onClick event
        publishButton.onClick.AddListener(OnButtonClicked);
    }

    // This method will be called every time the button is clicked
    private void OnButtonClicked()
    {
        // Create an array to store the joint positions
        double[] jointPositions = new double[6];

        // Get the values from the sliders, map them to degrees, and assign to the joint positions array
        for (int i = 0; i < jointSliders.Length; i++)
        {
            jointPositions[i] = MapSliderToDegrees(jointSliders[i].value, actuatorLimitsMin[i], actuatorLimitsMax[i]);
        }

        // Create a ROS Float64MultiArrayMsg
        Float64MultiArrayMsg msg = new Float64MultiArrayMsg
        {
            data = jointPositions
        };

        // Log and publish the joint positions
       // Debug.Log("Publishing joint positions (degrees): " + string.Join(", ", jointPositions));
        rosConnection.Publish(jointPositionsTopic, msg);
    }

    // Helper function to map a slider value (0 to 1) to a joint angle range (min to max), using the middle as zero
    private float MapSliderToDegrees(float sliderValue, float minAngle, float maxAngle)
    {
        // Normalize the slider value between 0 and 1 using InverseLerp
        float normalizedValue = Mathf.InverseLerp(minAngle, maxAngle, sliderValue);

        // Map this normalized value back to the desired angle range
        return Mathf.Lerp(minAngle, maxAngle, normalizedValue);
    }

}
