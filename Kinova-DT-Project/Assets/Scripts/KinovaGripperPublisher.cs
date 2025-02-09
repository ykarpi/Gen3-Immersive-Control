using RosMessageTypes.Std;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // For UI components like Button
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class KinovaGripperPublisher : MonoBehaviour
{
    private ROSConnection rosConnection; // Reference to your ROS connection

    [SerializeField] private Button openGripperButton;
    [SerializeField] private Button closeGripperButton;

    private string gripperTopic = "/unity/my_gen3/gripper"; // Topic to send the gripper commands to

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        // Register the publisher with the topic
        rosConnection.RegisterPublisher<Float64Msg>(gripperTopic);

        // Assign button listeners
        openGripperButton.onClick.AddListener(() => PublishGripperCommand(0)); // 1 to open the gripper
        closeGripperButton.onClick.AddListener(() => PublishGripperCommand(1)); // 0 to close the gripper
    }

    // Publishes the gripper command (1 for open, 0 for close)
    private void PublishGripperCommand(int gripperState)
    {
        // Create a ROS Int32Msg with the gripper command
        Float64Msg msg = new Float64Msg(gripperState);

        // Log and publish the gripper state
        if (gripperState == 1)
        {
            Debug.Log("Publishing gripper close command.");
        }
        else
        {
            Debug.Log("Publishing gripper open command.");
        }

        rosConnection.Publish(gripperTopic, msg);
    }
}
