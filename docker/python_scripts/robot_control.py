import rospy
from std_msgs.msg import Float64MultiArray
from kinova_moveit_api import KinovaMoveItAPI
from math import pi
from std_msgs.msg import String, Float64
from geometry_msgs.msg import Pose

class UnityToKinovaControl:
    def __init__(self):
        """Initialize the node and robot control interface."""
        rospy.init_node('unity_joint_position_listener', anonymous=True)
        
        self.kinova = KinovaMoveItAPI()
        self.last_joint_positions = None  # Store the last joint positions to compare
        # self.last_cartesian_pose = None  # Store the last joint positions to compare
        self.last_named_position = None  # Store the last named position to compare

        if not self.kinova.is_initialized:
            rospy.logerr("Failed to initialize KinovaMoveItAPI. Exiting...")
            rospy.signal_shutdown("KinovaMoveItAPI initialization failed.")
            return
        
        # Register the shutdown handler
        rospy.on_shutdown(self.shutdown)
        
        # Subscribe to the Unity topic
        rospy.Subscriber('/unity/my_gen3/joint_positions', Float64MultiArray, self.joint_positions_callback)
        rospy.Subscriber('/unity/my_gen3/named_robot_position', String, self.named_positions_callback)
        # rospy.Subscriber('/unity/my_gen3/cartesian_position', Pose, self.cartesian_position_callback)
        rospy.Subscriber('/unity/my_gen3/gripper', Float64, self.gripper_callback)

        
        rospy.loginfo("Node initialized and subscribed to /unity/my_gen3 topics.")


    def shutdown(self):
        # Clean up or stop robot control here
        rospy.loginfo("Shutting down the UnityToKinovaControl node...")
        # Add any cleanup or robot stop logic here if needed
        # For example, if you're using MoveIt, stop the robot arm or gripper
        self.stop_robot()

    def stop_robot(self):
            # Add the logic to stop or safely shutdown your robot control here
            rospy.loginfo("Stopping the robot...")


    def joint_positions_callback(self, msg):
        """Callback to handle joint positions published by Unity."""
        joint_positions = list(msg.data)
        rospy.loginfo(f"Received joint positions: {joint_positions}")

        joint_positions[0] = 1.248
        joint_positions[1] = -0.539
        joint_positions[2] = -1.285
        joint_positions[3] = 0
        joint_positions[4] = 1.085
        joint_positions[5] = -0.14

        # Check if the current joint positions are the same as the last ones
        if self.last_joint_positions == joint_positions:
            rospy.loginfo("Robot is already at the desired joint positions. Skipping move.")
            return  # Do not command the robot to move if the position is the same

        try:
            # Send joint positions to the robot
            success = self.kinova.reach_joint_angles(joint_positions)
            if success:
                rospy.loginfo("Robot successfully moved to the specified joint positions.")
                self.last_joint_positions = joint_positions  # Update the last joint positions
            else:
                rospy.logwarn("Failed to move the robot to the specified joint positions.")
        except Exception as e:
            rospy.logerr(f"Error while moving the robot: {e}")



    def named_positions_callback(self, msg):
        """Callback to handle named positions published by Unity."""
        named_position = msg.data
        rospy.loginfo(f"Received named position: {named_position}")

        # Check if the current named position is the same as the last one
        if self.last_named_position == named_position:
            rospy.loginfo("Robot is already at the desired named position. Skipping move.")
            return

        try:
            # Send named position to the robot
            success = self.kinova.reach_named_position(named_position)
            if success:
                rospy.loginfo(f"Robot successfully moved to the named position: {named_position}")
                self.last_named_position = named_position  # Update the last named position
            else:
                rospy.logwarn(f"Failed to move the robot to the named position: {named_position}")
        except Exception as e:
            rospy.logerr(f"Error while moving the robot to the named position: {e}")


    def gripper_callback(self, msg):
        """Callback to handle gripper position from Unity."""
        gripper_position = msg.data 
        rospy.loginfo(f"Received gripper position: {gripper_position}")

        success = self.kinova.reach_gripper_position(gripper_position)

        if success:
            rospy.loginfo("Gripper successfully moved to the specified position.")
        else:
            rospy.logwarn("Failed to move the gripper to the specified position.")



    # def cartesian_position_callback(self, msg):
    #     """Handle Cartesian pose updates."""
    #     try:
    #         rospy.loginfo("Reaching Cartesian Pose...")

    #         # Get the current Cartesian pose
    #         actual_pose = self.kinova.get_cartesian_pose()

    #         # Modify the pose based on the received message
    #         target_pose = Pose()
    #         target_pose.position.x = msg.position.x
    #         target_pose.position.y = msg.position.y
    #         target_pose.position.z = msg.position.z
    #         target_pose.orientation.x = msg.orientation.x
    #         target_pose.orientation.y = msg.orientation.y
    #         target_pose.orientation.z = msg.orientation.z
    #         target_pose.orientation.w = msg.orientation.w

    #         rospy.loginfo(f"Moving robot to new Cartesian pose: {target_pose}")

    #         # Move to the new Cartesian pose
    #         success = self.kinova.reach_cartesian_pose(pose=target_pose, tolerance=0.01, constraints=None)

    #         if success:
    #             rospy.loginfo(f"Successfully reached the new Cartesian pose: {target_pose}")
    #             self.last_cartesian_pose = target_pose  # Update the last Cartesian pose
    #         else:
    #             rospy.logwarn("Failed to reach the desired Cartesian pose.")
                
    #     except Exception as e:
    #         rospy.logerr(f"Error in Cartesian control: {e}")



    def spin(self):
        """Keep the node running."""
        rospy.spin()



def main():
    # Create your node instance
    control_node = UnityToKinovaControl()
    
    try:
        control_node.spin()
    except rospy.ROSInterruptException:
        rospy.loginfo("ROS Interrupt received. Node will shut down.")
    finally:
        rospy.loginfo("Exiting the node...")



if __name__ == '__main__':
    main()
