using UnityEngine;
using UnityEngine.UI; // For UI components like Button
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class StopGestureScript : MonoBehaviour
{

    private ROSConnection rosConnection;

    private string clearFaultsTopic; // Topic to clear faults
    private string emergencyStopTopic; // Topic for emergency stop

    private string robotName = "my_gen3";

    // Start is called before the first frame update
    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        // Define the topics with the robot name
        clearFaultsTopic = $"/{robotName}/in/clear_faults";
        // stopTopic = $"/{robotName}/in/stop";
        emergencyStopTopic = $"/{robotName}/in/emergency_stop";
    }

    public void PublishEmergencyStop()
    {
        // Publish an empty message to the emergency stop topic
        EmptyMsg msg = new EmptyMsg();
        rosConnection.Publish(emergencyStopTopic, msg);
        Debug.Log("Published to /emergency_stop");
    //    emergencyIsActive = true;
    }
}
