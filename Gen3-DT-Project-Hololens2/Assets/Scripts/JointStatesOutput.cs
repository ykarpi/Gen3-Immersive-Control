//using System;
//using RosMessageTypes.Geometry;
//using Joint = RosMessageTypes.Sensor.JointStateMsg;
//using Unity.Robotics.ROSTCPConnector;
//using Unity.Robotics.ROSTCPConnector.ROSGeometry;
//using Unity.Robotics.UrdfImporter;
//using UnityEngine;
//using System.Collections;

//public class JointStatesOutput : MonoBehaviour
//{

//    [SerializeField] GameObject kinova6dof;
//    private double[] prevPos6dof = new double[6];
//    private double[] currPos6dof = new double[6];

//    private ArticulationBody joint6dof;

//    private int[] rev6dof = new int[6];

//    // Robot Joints
//    private ArticulationBody[] articulationChain6dof;

//    void Start()
//    {
//        // Find the kinova6dof GameObject by name under ARMarker
//        Transform kinovaTransform = transform.Find("kinova6dof");
//        if (kinovaTransform != null)
//        {
//            kinova6dof = kinovaTransform.gameObject;
//            articulationChain6dof = kinova6dof.GetComponentsInChildren<ArticulationBody>();
//        }
//        else
//        {
//            Debug.LogError("kinova6dof not found under ARMarker!");
//        }

//        ROSConnection.GetOrCreateInstance().Subscribe<Joint>("my_gen3/joint_states", UpdateJoints6dof);
//    }


//    void Update()
//    {
//        if (Time.time > 1)
//        {
//            for (int i = 0; i < 6; i++)
//            {
//                joint6dof = articulationChain6dof[i + 1];
//                ArticulationDrive currentDrive = joint6dof.xDrive;
//                currentDrive.stiffness = 1e+10f;
//                articulationChain6dof[i + 1].xDrive = currentDrive;
//            }
//        }
//    }


//    void UpdateJoints6dof(Joint jointMessage)
//    {
//        // FOR REFERENCE
//        // articulationChain[0] // Base_Link
//        // articulationChain[1] // Shoulder_Link
//        // articulationChain[2] // Bicep_Link
//        // articulationChain[3] // ForArm_Link
//        // articulationChain[4] // SphereicalWrist1_Link
//        // articulationChain[5] // SphereicalWrist2_Link
//        // articulationChain[6] // Bracelet_Link


//        currPos6dof = (jointMessage.position);
//        for (int i = 0; i < 6; i++)
//        {
//            currPos6dof[i] = (currPos6dof[i] / Math.PI * 180);
//            if (currPos6dof[i] - prevPos6dof[i] > 300)
//            {
//                rev6dof[i]--;
//            }
//            else if (currPos6dof[i] - prevPos6dof[i] < -300)
//            {
//                rev6dof[i]++;
//            }

//            joint6dof = articulationChain6dof[i + 1];
//            ArticulationDrive currentDrive = joint6dof.xDrive;
//            currentDrive.target = (float)(currPos6dof[i] + 360 * rev6dof[i]);
//            joint6dof.xDrive = currentDrive;
//            prevPos6dof[i] = currPos6dof[i];
//        }

//    }
//}


using System;
using RosMessageTypes.Sensor;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine.XR.ARSubsystems;
using Microsoft.MixedReality.OpenXR; // For ARMarker
using Unity.Robotics.UrdfImporter;

public class JointStatesOutput : MonoBehaviour
{
    [SerializeField] private ARMarkerManager markerManager;
    private GameObject kinova6dof;
    private ArticulationBody[] articulationChain6dof;

    private double[] prevPos6dof = new double[6];
    private double[] currPos6dof = new double[6];
    private int[] rev6dof = new int[6];
    private bool isRobotInitialized = false;
    private bool isMarkerFound = false;


    void Start()
    {
        if (markerManager == null)
        {
            Debug.LogError("ARMarkerManager not assigned.");
            return;
        }

        markerManager.markersChanged += OnQRCodesChanged;

        ROSConnection.GetOrCreateInstance().Subscribe<JointStateMsg>("my_gen3/joint_states", UpdateJoints6dof);
    }

    //private void OnQRCodesChanged(ARMarkersChangedEventArgs args)
    //{
    //    foreach (var addedMarker in args.added)
    //    {
    //        Debug.Log("Add a marker --> ", addedMarker);
    //    }

    //    foreach (var updatedMarker in args.updated)
    //    {
    //        Debug.Log("Updated a marker --> ", updatedMarker);
    //    }

    //    foreach (var removedMarkerId in args.removed)
    //    {
    //        Debug.Log("Removed a marker --> ", removedMarkerId);
    //    }
    //}

    private void OnQRCodesChanged(ARMarkersChangedEventArgs args)
    {
        //foreach (ARMarker marker in args.updated)
        //foreach (ARMarker marker in args.added)

        if (!isMarkerFound)
        {
            foreach (ARMarker marker in args.updated)
            {
                Debug.Log("Marker detected --> ", marker);

                Transform kinovaTransform = marker.transform.Find("kinova6dof");

                kinova6dof = kinovaTransform.gameObject;
                articulationChain6dof = kinovaTransform.GetComponentsInChildren<ArticulationBody>();

                Debug.Log("Kinova robot found and initialized at runtime.");

                isRobotInitialized = true;

                isMarkerFound = true;

                Debug.Log("The isRobotInitialized is set to --> " + isRobotInitialized);

            }
        }
        else
        {
            return;
        }
    }


    void Update()
    {
        // Optional: Set joint stiffness after start
        if (isRobotInitialized && Time.time > 1)
        {
            for (int i = 0; i < 6; i++)
            {
                var joint = articulationChain6dof[i + 1];
                var drive = joint.xDrive;
                drive.stiffness = 1e+10f;
                joint.xDrive = drive;
            }
        }
    }

    void UpdateJoints6dof(JointStateMsg jointMessage)
    {
        if (!isRobotInitialized) return;

        currPos6dof = jointMessage.position;

        for (int i = 0; i < 6; i++)
        {
            currPos6dof[i] = currPos6dof[i] / Math.PI * 180;

            if (currPos6dof[i] - prevPos6dof[i] > 300) rev6dof[i]--;
            else if (currPos6dof[i] - prevPos6dof[i] < -300) rev6dof[i]++;

            var joint = articulationChain6dof[i + 1];
            var drive = joint.xDrive;
            drive.target = (float)(currPos6dof[i] + 360 * rev6dof[i]);
            joint.xDrive = drive;

            prevPos6dof[i] = currPos6dof[i];
        }
    }
}
