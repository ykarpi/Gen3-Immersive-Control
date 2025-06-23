using RosMessageTypes.Std;
using UnityEngine;
using UnityEngine.UI; // For UI components like Button
using Unity.Robotics.ROSTCPConnector;
using MixedReality.Toolkit.UX;
using TMPro;

public class KinovaGripperPublisher : MonoBehaviour
{
    private ROSConnection rosConnection;

    [SerializeField]
    private PressableButton gripperToggleButton; // Single button for open/close

    private string gripperTopic = "/unity/my_gen3/gripper";

    private bool isGripperClosed = false; // Start assuming it's open

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        rosConnection.RegisterPublisher<Float64Msg>(gripperTopic);

        gripperToggleButton.OnClicked.AddListener(ToggleGripper);

        // Optional: set initial button label
        gripperToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close Gripper";

    }

    private void ToggleGripper()
    {
        Float64Msg msg;

        if (!isGripperClosed)
        {
            msg = new Float64Msg(1); // Close
            Debug.Log("Publishing gripper CLOSE command.");
            gripperToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Open Gripper";
        }
        else
        {
            msg = new Float64Msg(0); // Open
            Debug.Log("Publishing gripper OPEN command.");
            gripperToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close Gripper";
        }

        rosConnection.Publish(gripperTopic, msg);
        isGripperClosed = !isGripperClosed;
    }
}
