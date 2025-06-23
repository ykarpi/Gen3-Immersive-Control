using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using MixedReality.Toolkit.UX;
using TMPro;

public class KinovaStopPublisher : MonoBehaviour
{
    private ROSConnection rosConnection;

    [SerializeField]
    private PressableButton emergencyStopButton; // Single button for both actions

    private string robotName = "my_gen3";

    private string clearFaultsTopic;
    private string emergencyStopTopic;

    private bool emergencyIsActive = false;

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();

        clearFaultsTopic = $"/{robotName}/in/clear_faults";
        emergencyStopTopic = $"/{robotName}/in/emergency_stop";

        rosConnection.RegisterPublisher<EmptyMsg>(clearFaultsTopic);
        rosConnection.RegisterPublisher<EmptyMsg>(emergencyStopTopic);

        emergencyStopButton.OnClicked.AddListener(ToggleEmergencyAction);
    }

    private void ToggleEmergencyAction()
    {
        EmptyMsg msg = new EmptyMsg();

        if (!emergencyIsActive)
        {
            rosConnection.Publish(emergencyStopTopic, msg);
            Debug.Log("Published Emergency Stop");
            emergencyIsActive = true;

            // Optional: change button label
            emergencyStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Clear Faults";
        }
        else
        {
            rosConnection.Publish(clearFaultsTopic, msg);
            Debug.Log("Published Clear Faults");
            emergencyIsActive = false;

            // Optional: reset button label
            emergencyStopButton.GetComponentInChildren<TextMeshProUGUI>().text = "Emergency Stop";
        }
    }
}
