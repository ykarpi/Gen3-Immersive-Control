using System.Collections;
using System.Collections.Generic;
using System;
using Joint = RosMessageTypes.Sensor.JointStateMsg;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.UI;
using RosMessageTypes.Std;
using System.Linq;

public class GazeboSimManualJointControl : MonoBehaviour
{
    private ROSConnection rosConnection; // Reference to your ROS connection
    private string publishJointsToGen3 = "/unity/my_gen3/joint_positions";
    private Float64MultiArrayMsg jointStateMsg = new Float64MultiArrayMsg();

    [SerializeField] GameObject kinova6dof;
    private double[] prevPos6dof = new double[6];
    private double[] currPos6dof = new double[6];
    private ArticulationBody joint6dof;
    private int[] rev6dof = new int[6];
    private ArticulationBody[] articulationChain6dof;

    [SerializeField] Slider[] jointSliders; // Array of sliders for manual control (6 sliders for each joint)
    private bool isSynchronized = true; // Flag to control whether the physical and virtual models are synchronized

    [SerializeField] private Button syncOff;
    [SerializeField] private Button syncOn;

    [SerializeField] private Button publishAllToZeroButton;
    [SerializeField] private Button publishCurrentJoints;

    // Define the joint limits - according to the official documentation
    private float[] actuatorLimitsMin = new float[] { -180f, -128.9f, -147.8f, -180f, -120.3f, -180f };
    private float[] actuatorLimitsMax = new float[] { 180f, 128.9f, 147.8f, 180f, 120.3f, 180f };

    void Start()
    {
        rosConnection = ROSConnection.GetOrCreateInstance();
        // Subscribe to ROS messages
        ROSConnection.GetOrCreateInstance().Subscribe<Joint>("my_gen3/joint_states", UpdateJoints6dof);
        articulationChain6dof = kinova6dof.GetComponentsInChildren<ArticulationBody>();

        // Set up sliders to control the joints
        for (int i = 0; i < jointSliders.Length; i++)
        {
            int index = i;
            jointSliders[i].onValueChanged.AddListener(value => OnSliderValueChanged(index, value));
        }

        // Button listeners to trigger synchronization methods
        syncOff.onClick.AddListener(StopSynchronization);
        syncOn.onClick.AddListener(StartSynchronization);

        

        // Register the publisher with the topic
        rosConnection.RegisterPublisher<Float64MultiArrayMsg>(publishJointsToGen3);
        publishAllToZeroButton.onClick.AddListener(SendAllJointsToZero);
        publishCurrentJoints.onClick.AddListener(PublishJointStates);

    }

    void Update()
    {

        if (isSynchronized && Time.time > 1)
        {
            for (int i = 0; i < 6; i++)
            {
                joint6dof = articulationChain6dof[i + 1];
                ArticulationDrive currentDrive = joint6dof.xDrive;
                currentDrive.stiffness = 1e+10f;
                articulationChain6dof[i + 1].xDrive = currentDrive;
            }
        }
       
        // If synchronization is stopped, control the virtual joints with sliders
        else
        {
            UpdateVirtualJoints();
        }
    }

    void UpdateJoints6dof(Joint jointMessage)
    {
        if (isSynchronized)
        {
            // Update joint angles based on ROS message
            //currPos6dof = (jointMessage.position); // the line used only with the physical model
            currPos6dof = jointMessage.position.Skip(1).ToArray(); //adjusting the array for the data from Gazebo simulation
            for (int i = 0; i < 6; i++)
            {
                currPos6dof[i] = (currPos6dof[i] / Math.PI * 180); // Convert radians to degrees

                // Update slider values based on joint angles
                if (jointSliders != null && jointSliders.Length > i)
                {
                    // Map joint angle to slider value using the specific joint limits
                    float sliderValue = Mathf.InverseLerp(actuatorLimitsMin[i], actuatorLimitsMax[i], (float)currPos6dof[i]);
                    jointSliders[i].value = sliderValue;
                }

                // Handle joint wrap-around (if necessary)
                if (currPos6dof[i] - prevPos6dof[i] > 300)
                {
                    rev6dof[i]--;
                }
                else if (currPos6dof[i] - prevPos6dof[i] < -300)
                {
                    rev6dof[i]++;
                }

                joint6dof = articulationChain6dof[i + 1];
                ArticulationDrive currentDrive = joint6dof.xDrive;
                currentDrive.target = (float)(currPos6dof[i] + 360 * rev6dof[i]);
                joint6dof.xDrive = currentDrive;
                prevPos6dof[i] = currPos6dof[i];
            }
        }
    }

    // Function to stop synchronization, allowing manual control with sliders
    public void StopSynchronization()
    {
        isSynchronized = false;
    }

    // Function to start synchronization, allowing ROS to control the joint positions
    public void StartSynchronization()
    {
        isSynchronized = true;
    }

    // Update virtual joints based on slider values
    void UpdateVirtualJoints()
    {
        for (int i = 0; i < 6; i++)
        {
            float sliderValue = jointSliders[i].value; // Get slider value (assumed range 0 to 1)
                                                       
            float jointAngle = Mathf.Lerp(actuatorLimitsMin[i], actuatorLimitsMax[i], sliderValue); // Map slider value to joint angle using the joint-specific limits

            joint6dof = articulationChain6dof[i + 1];
            ArticulationDrive currentDrive = joint6dof.xDrive;
            currentDrive.target = jointAngle;
            joint6dof.xDrive = currentDrive;
        }
    }

    // Slider value changed event handler for controlling individual joints
    void OnSliderValueChanged(int jointIndex, float value)
    {
        // Map the slider value to a joint angle using the joint-specific limits
        float jointAngle = Mathf.Lerp(actuatorLimitsMin[jointIndex], actuatorLimitsMax[jointIndex], value);

        // Update the target joint angle for the corresponding joint
        joint6dof = articulationChain6dof[jointIndex + 1];
        ArticulationDrive currentDrive = joint6dof.xDrive;
        currentDrive.target = jointAngle;
        joint6dof.xDrive = currentDrive;
    }

    void SendAllJointsToZero()
    {
        jointStateMsg = new Float64MultiArrayMsg
        {
            data = new double[6] // Assuming a 6-DOF robot
        };
        for (int i = 0; i < 6; i++)
        {
            jointStateMsg.data[i] = 0; // Populate with joint angles
        }

       // Debug.Log("Publishing the jointStateMsg --> " + jointStateMsg.data);

        // Publish the joint state message
        rosConnection.Publish(publishJointsToGen3, jointStateMsg);

        isSynchronized = true;
    }


    void PublishJointStates()
    {
        if (articulationChain6dof == null || articulationChain6dof.Length < 7)
        {
            Debug.LogError("Articulation chain is not properly initialized.");
            return;
        }

        jointStateMsg = new Float64MultiArrayMsg
        {
            data = new double[6] // Assuming a 6-DOF robot
        };

        for (int i = 0; i < 6; i++) // Loop through each joint
        {
            ArticulationBody joint6dof = articulationChain6dof[i + 1]; // Skip base link

            if (joint6dof == null)
            {
                Debug.LogWarning($"Joint {i} is null.");
                jointStateMsg.data[i] = 0;
                continue;
            }

            // Extract the current joint position (ensuring it exists)
            if (joint6dof.jointPosition.dofCount > 0)
            {
                float jointAngle = joint6dof.jointPosition[0]; // Radians by default
                jointStateMsg.data[i] = jointAngle; // Already in radians
            }
            else
            {
                Debug.LogWarning($"Joint {i} has no valid joint position.");
                jointStateMsg.data[i] = 0;
            }
        }

       // Debug.Log("Publishing the jointStateMsg --> " + jointStateMsg.data);

        // Publish the joint state message to ROS
        rosConnection.Publish(publishJointsToGen3, jointStateMsg);
    }


}
