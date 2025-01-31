using System;
using UnityEngine;
using UnityEngine.UI;

public class GhostModelJointController : MonoBehaviour
{
    [SerializeField] GameObject kinovaModel;  // Reference to the real Kinova model in Unity
    [SerializeField] GameObject kinovaGhost;  // Reference to the ghost model
    [SerializeField] Slider[] jointSliders;   // Sliders to manually control the ghost model's joints

    private ArticulationBody[] kinovaArticulationChain;  // Real Kinova model's articulation chain
    private ArticulationBody[] ghostArticulationChain;   // Ghost model's articulation chain

    private bool[] manualControl = new bool[6];  // Track if a joint is being manually controlled
    private bool isSynchronized = false;  // Flag to check if synchronization has been completed

    // Define the joint limits - according to the official documentation
    private float[] actuatorLimitsMin = new float[] { -180f, -128.9f, -147.8f, -180f, -120.3f, -180f };
    private float[] actuatorLimitsMax = new float[] { 180f, 128.9f, 147.8f, 180f, 120.3f, 180f };

    void Start()
    {
        // Initialize the articulation body chains for both the Kinova model and the ghost model
        kinovaArticulationChain = kinovaModel.GetComponentsInChildren<ArticulationBody>();
        ghostArticulationChain = kinovaGhost.GetComponentsInChildren<ArticulationBody>();

        // Synchronize ghost with Kinova model at the start
        SyncGhostWithKinova();

        // Add listeners to sliders for manual control
        foreach (Slider slider in jointSliders)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void Update()
    {
        // Only synchronize ghost with Kinova model if synchronization has not yet happened
        if (isSynchronized)
        {
            for (int i = 0; i < 6; i++)
            {
                // Get the current joint position from the real Kinova model
                float kinovaPosition = kinovaArticulationChain[i].xDrive.target;

                // Debug log to check kinovaPosition
                Debug.Log($"Kinova joint {i} position: {kinovaPosition}");

                // Update the ghost model's joint to match the Kinova model's position
                ArticulationBody ghostJoint = ghostArticulationChain[i];  // Fixed indexing here
                ArticulationDrive ghostDrive = ghostJoint.xDrive;
                ghostDrive.target = kinovaPosition;
                ghostJoint.xDrive = ghostDrive;

                // Update the slider only if the slider isn't being manually controlled
                if (!manualControl[i])
                {
                    SetSliderValue(i, kinovaPosition);
                }
            }
        }
    }

    // Sync ghost model's joints with Kinova model at the start
    void SyncGhostWithKinova()
    {
        for (int i = 0; i < 6; i++)
        {
            // Initialize position by reading the Kinova joint's target
            float kinovaPosition = kinovaArticulationChain[i].xDrive.target;

            // Debug log to check initial kinovaPosition
            Debug.Log($"Syncing Kinova joint {i} position: {kinovaPosition}");

            // Set the ghost model's joints to the same position
            ArticulationBody ghostJoint = ghostArticulationChain[i];
            ArticulationDrive ghostDrive = ghostJoint.xDrive;
            ghostDrive.target = kinovaPosition;
            ghostJoint.xDrive = ghostDrive;

            // Set the initial value of the slider based on the joint's current position
            SetSliderValue(i, kinovaPosition);
        }

        // Set synchronization flag
        isSynchronized = true;
    }

    // Function to map the joint position to the slider value (normalized between 0 and 1)
    private void SetSliderValue(int jointIndex, float currentPosition)
    {
        Debug.Log(currentPosition);
        float rangeMin = actuatorLimitsMin[jointIndex];
        float rangeMax = actuatorLimitsMax[jointIndex];

        // Normalize the joint position within its range
        float normalizedValue = Mathf.InverseLerp(rangeMin, rangeMax, currentPosition);

        // Debugging to ensure correct mapping
        //Debug.Log($"Setting slider for joint {jointIndex}: Value = {normalizedValue} (Range: {rangeMin} to {rangeMax})");

        // Set the slider value based on the normalized value
        jointSliders[jointIndex].value = normalizedValue;
    }


    private void OnSliderValueChanged(float value)
    {
        for (int i = 0; i < jointSliders.Length; i++)
        {
            if (jointSliders[i].value != value)
                continue;

            // Map the slider value (0 to 1) to the actuator joint's range (min to max)
            float rangeMin = actuatorLimitsMin[i];
            float rangeMax = actuatorLimitsMax[i];

            // For the neutral position (0.5 in slider), we want the position to be 0.
            // Map slider value (0 to 1) to the range [-max, max] for each joint
            float targetPosition = Mathf.Lerp(rangeMin, rangeMax, value);  // Maps slider range 0-1 to the joint's range

            // Update the corresponding ghost joint
            ArticulationBody ghostJoint = ghostArticulationChain[i];
            ArticulationDrive ghostDrive = ghostJoint.xDrive;
            ghostDrive.target = targetPosition;
            ghostJoint.xDrive = ghostDrive;

            // Debug to confirm xDrive updates
            //Debug.Log($"Updating ghost joint {i}. Old target: {ghostDrive.target}, New target: {targetPosition}");

            // Mark the joint as manually controlled
            manualControl[i] = true;

            // Optionally, disable further syncing until manual control is released
            isSynchronized = false;
        }
    }
    // Call this function to disconnect the ghost model from the Kinova model
    public void DisconnectGhost()
    {
        // Stop synchronizing the ghost model with Kinova (no more updates)
        isSynchronized = false;

        // Now the ghost model can be freely manipulated without being overwritten by the Kinova model
    }

    public void ReleaseManualControl(int jointIndex)
    {
        manualControl[jointIndex] = false;
        isSynchronized = true;
    }
}

