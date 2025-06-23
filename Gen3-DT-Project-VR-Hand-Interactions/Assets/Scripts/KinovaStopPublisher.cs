using UnityEngine;
using UnityEngine.UI; // For UI components like Button
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class KinovaStopPublisher : MonoBehaviour
{
    private ROSConnection rosConnection;

    [SerializeField]
    private Button clearFaultsButton; // Button for clear faults
    //[SerializeField]
    //private Button stopButton; // Button for stop
    [SerializeField]
    private Button emergencyStopButton; // Button for emergency stop

    private string robotName = "my_gen3"; // Replace with your robot's name

    private string clearFaultsTopic; // Topic to clear faults
   // private string stopTopic; // Topic to stop motion
    private string emergencyStopTopic; // Topic for emergency stop

    bool emergencyIsActive = false;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        // Define the topics with the robot name
        clearFaultsTopic = $"/{robotName}/in/clear_faults";
       // stopTopic = $"/{robotName}/in/stop";
        emergencyStopTopic = $"/{robotName}/in/emergency_stop";

        // Register publishers for each topic
        rosConnection.RegisterPublisher<EmptyMsg>(clearFaultsTopic);
       // rosConnection.RegisterPublisher<EmptyMsg>(stopTopic);
        rosConnection.RegisterPublisher<EmptyMsg>(emergencyStopTopic);

        // Add listeners to buttons
        clearFaultsButton.onClick.AddListener(PublishClearFaults);
     //   stopButton.onClick.AddListener(PublishStop);
        emergencyStopButton.onClick.AddListener(PublishEmergencyStop);
    }

    private void PublishClearFaults()
    {
        if (emergencyIsActive == true)
        {
            // Publish an empty message to the clear faults topic
            EmptyMsg msg = new EmptyMsg();
            rosConnection.Publish(clearFaultsTopic, msg);
            Debug.Log("Published to /clear_faults");
        }

    }

    //private void PublishStop()
    //{
    //    // Publish an empty message to the stop topic
    //    EmptyMsg msg = new EmptyMsg();
    //    rosConnection.Publish(stopTopic, msg);
    //    Debug.Log("Published to /stop");
    //}

    private void PublishEmergencyStop()
    {
        // Publish an empty message to the emergency stop topic
        EmptyMsg msg = new EmptyMsg();
        rosConnection.Publish(emergencyStopTopic, msg);
        Debug.Log("Published to /emergency_stop");
        emergencyIsActive = true;
    }
}
