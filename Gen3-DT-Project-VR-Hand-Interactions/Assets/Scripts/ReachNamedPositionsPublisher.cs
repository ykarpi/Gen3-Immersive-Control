using UnityEngine;
using UnityEngine.UI; // For UI components like Button
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class ReachNamedPositionsPublisher : MonoBehaviour
{
    private ROSConnection rosConnection; // Reference to your ROS connection

    [SerializeField]
    private Button verticalButton, retractButton, homeButton; // Buttons for predefined positions

    private string robotPositionTopic = "/unity/my_gen3/named_robot_position"; // Topic to send the robot positions to

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        // Register the publisher with the topic
        rosConnection.RegisterPublisher<StringMsg>(robotPositionTopic);

        // Assign button listeners
        verticalButton.onClick.AddListener(() => PublishRobotPosition("vertical"));
        retractButton.onClick.AddListener(() => PublishRobotPosition("retract"));
        homeButton.onClick.AddListener(() => PublishRobotPosition("home"));
    }

    // Publishes the robot's predefined position
    private void PublishRobotPosition(string position)
    {
        // Create a ROS StringMsg with the specified position
        StringMsg msg = new StringMsg(position);

        // Log and publish the position
        Debug.Log($"Publishing robot position: {position}");
        rosConnection.Publish(robotPositionTopic, msg);
    }
}