using System;
using RosMessageTypes.Geometry;
using Joint = RosMessageTypes.Sensor.JointStateMsg;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

public class JointStatesOutput : MonoBehaviour
{

    [SerializeField] GameObject kinova6dof;
    private double[] prevPos6dof = new double[6];
    private double[] currPos6dof = new double[6];

    private ArticulationBody joint6dof;

    private int[] rev6dof = new int[6];

    // Robot Joints
    private ArticulationBody[] articulationChain6dof;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<Joint>("my_gen3/joint_states", UpdateJoints6dof);

        articulationChain6dof = kinova6dof.GetComponentsInChildren<ArticulationBody>();
    }

    void Update()
    {
        if (Time.time > 1)
        {
            for (int i = 0; i < 6; i++)
            {
                joint6dof = articulationChain6dof[i + 1];
                ArticulationDrive currentDrive = joint6dof.xDrive;
                currentDrive.stiffness = 1e+10f;
                articulationChain6dof[i + 1].xDrive = currentDrive;
            }
        }
    }


    void UpdateJoints6dof(Joint jointMessage)
    {
        // FOR REFERENCE
        // articulationChain[0] // Base_Link
        // articulationChain[1] // Shoulder_Link
        // articulationChain[2] // Bicep_Link
        // articulationChain[3] // ForArm_Link
        // articulationChain[4] // SphereicalWrist1_Link
        // articulationChain[5] // SphereicalWrist2_Link
        // articulationChain[6] // Bracelet_Link


        currPos6dof = (jointMessage.position);
        for (int i = 0; i < 6; i++)
        {
            currPos6dof[i] = (currPos6dof[i] / Math.PI * 180);
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
