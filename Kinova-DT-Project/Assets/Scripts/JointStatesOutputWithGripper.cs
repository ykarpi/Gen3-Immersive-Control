using System;
using RosMessageTypes.Geometry;
using Joint = RosMessageTypes.Sensor.JointStateMsg;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;
using UnityEngine.UIElements;

public class JointStatesOutputWithGripper : MonoBehaviour
{
    [SerializeField]
    GameObject kinova6dof;

    //[SerializeField]
    //GameObject right_outer_knuckle_6dof;
    //[SerializeField]
    //GameObject right_inner_knuckle_6dof;
    //[SerializeField]
    //GameObject left_outer_knuckle_6dof;
    //[SerializeField]
    //GameObject left_inner_knuckle_6dof;
    //[SerializeField]
    //GameObject right_inner_finger_6dof;
    //[SerializeField]
    //GameObject left_inner_finger_6dof;


    private ArticulationBody joint6dof;

    private float[] prevPos6dof = new float[] { 0, 0, 0, 0, 0, 0 };

    private float[] currPos6dof = new float[] { 0, 0, 0, 0, 0, 0 };

    private int[] rev6dof = new int[] { 0, 0, 0, 0, 0, 0 };

    // Robot Joints
    private ArticulationBody[] articulationChain6dof;

    private float time;

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<Joint>("my_gen3/joint_states", UpdateJoints6dof);
        articulationChain6dof = kinova6dof.GetComponentsInChildren<ArticulationBody>();

        //articulationChain6dof[7] = right_outer_knuckle_6dof.GetComponent<ArticulationBody>();
        //articulationChain6dof[8] = right_inner_knuckle_6dof.GetComponent<ArticulationBody>();
        //articulationChain6dof[9] = left_inner_finger_6dof.GetComponent<ArticulationBody>();
        //articulationChain6dof[10] = left_outer_knuckle_6dof.GetComponent<ArticulationBody>();
        //articulationChain6dof[11] = left_inner_knuckle_6dof.GetComponent<ArticulationBody>();
        //articulationChain6dof[12] = right_inner_finger_6dof.GetComponent<ArticulationBody>();
    }


    void Update()
    {
        if (Time.time > 1)
        {
            for (int i = 0; i < 7; i++)
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
        // articulationChain[1] // Shoulder_Link
        // articulationChain[2] // Bicep_Link
        // articulationChain[3] // ForArm_Link
        // articulationChain[4] // SphereicalWrist1_Link
        // articulationChain[5] // SphereicalWrist2_Link
        // articulationChain[6] // Bracelet_Link

        Debug.Log((jointMessage.position).Length);

        for (int i = 0; i < 6; i++)
        {

            currPos6dof[i] = (float)(jointMessage.position[i] / Math.PI * 180);
           // Debug.Log(currPos6dof[i]);
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
            currentDrive.target = (float)(jointMessage.position[i] / Math.PI * 180 + 360 * rev6dof[i]);
            joint6dof.xDrive = currentDrive;
            prevPos6dof[i] = currPos6dof[i];
        }

        //// Process gripper joints
        //float fingerPosition = (float)(jointMessage.position[6] * 180.0 / Math.PI);

        //for (int i = 7; i < 10; i++) // Left fingers
        //{
        //    joint6dof = articulationChain6dof[i];
        //    ArticulationDrive currentDrive = joint6dof.xDrive;
        //    currentDrive.target = fingerPosition;
        //    joint6dof.xDrive = currentDrive;
        //}

        //for (int i = 10; i < 13; i++) // Right fingers (mirrored movement)
        //{
        //    joint6dof = articulationChain6dof[i];
        //    ArticulationDrive currentDrive = joint6dof.xDrive;
        //    currentDrive.target = -fingerPosition;
        //    joint6dof.xDrive = currentDrive;
        //}
    }

    //// Right knuckles: negative, left: positive
    //for (int i = 6; i < 9; i++)
    //{
    //    joint6dof = articulationChain6dof[i];
    //    ArticulationDrive currentDrive = joint6dof.xDrive;
    //    currentDrive.target = (float)(-jointMessage.position[7] / Math.PI * 180);
    //    joint6dof.xDrive = currentDrive;
    //}
    //for (int i = 9; i < 12; i++)
    //{
    //    joint6dof = articulationChain6dof[i];
    //    ArticulationDrive currentDrive = joint6dof.xDrive;
    //    currentDrive.target = (float)(jointMessage.position[7] / Math.PI * 180);
    //    joint6dof.xDrive = currentDrive;
    //}

}