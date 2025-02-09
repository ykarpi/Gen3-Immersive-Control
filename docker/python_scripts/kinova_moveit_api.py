import sys
import rospy
import moveit_commander
import moveit_msgs.msg
import geometry_msgs.msg
from math import pi

# To run this node in a given namespace with rosrun (for example 'my_gen3'), start a Kortex driver and then run : 
# rosrun kortex_examples example_move_it_trajectories.py __ns:=my_gen3
class KinovaMoveItAPI(object):
    """KinovaMoveItAPI - Interface for controlling Kinova robots using MoveIt"""

    def __init__(self):
        # Initialize the MoveIt Commander
        moveit_commander.roscpp_initialize(sys.argv)

        # Initialize the ROS node
        try:
            rospy.init_node('kinova_moveit_api', anonymous=True)
        except rospy.exceptions.ROSException:
            rospy.loginfo("Node already initialized. Skipping rospy.init_node().")

        try:
            self.is_gripper_present = rospy.get_param(rospy.get_namespace() + "is_gripper_present", False)
            if self.is_gripper_present:
                gripper_joint_names = rospy.get_param(rospy.get_namespace() + "gripper_joint_names", [])
                self.gripper_joint_name = gripper_joint_names[0]
            else:
                self.gripper_joint_name = ""
            self.degrees_of_freedom = rospy.get_param(rospy.get_namespace() + "degrees_of_freedom", 7)

            # Create the MoveItInterface necessary objects
            arm_group_name = "arm"
            self.robot = moveit_commander.RobotCommander("robot_description")
            self.scene = moveit_commander.PlanningSceneInterface(ns=rospy.get_namespace())
            self.arm_group = moveit_commander.MoveGroupCommander(arm_group_name, ns=rospy.get_namespace())
            self.display_trajectory_publisher = rospy.Publisher(rospy.get_namespace() + 'move_group/display_planned_path',
                                                            moveit_msgs.msg.DisplayTrajectory,
                                                            queue_size=20)

            if self.is_gripper_present:
                gripper_group_name = "gripper"
                self.gripper_group = moveit_commander.MoveGroupCommander(gripper_group_name, ns=rospy.get_namespace())


            rospy.loginfo("KinovaMoveItAPI initialized successfully.")
            self.is_initialized = True

        except Exception as e:
            rospy.logerr(f"Failed to initialize KinovaMoveItAPI: {e}")
            self.is_initialized = False

    def reach_named_position(self, target):
        """Move the arm to a named position."""
        rospy.loginfo(f"Moving to named target: {target}")
        self.arm_group.set_named_target(target)
        success = self.arm_group.go(wait=True)
        return success

    def reach_joint_angles(self, joint_angles, tolerance=0.01):
        """Move the arm to specified joint angles."""
        rospy.loginfo("Moving to specified joint angles.")
        self.arm_group.set_goal_joint_tolerance(tolerance)
        self.arm_group.set_joint_value_target(joint_angles)
        success = self.arm_group.go(wait=True)
        return success

    # def get_cartesian_pose(self):
    #     """Retrieve the current Cartesian pose of the arm."""
    #     pose = self.arm_group.get_current_pose().pose
    #     rospy.loginfo(f"Current Cartesian pose: {pose}")
    #     return pose

    # def reach_cartesian_pose(self, pose, tolerance=0.01, constraints=None):
    #     """Move the arm to a specified Cartesian pose."""
    #     rospy.loginfo("Moving to specified Cartesian pose.")
    #     self.arm_group.set_goal_position_tolerance(tolerance)
    #     if constraints:
    #         self.arm_group.set_path_constraints(constraints)
    #     self.arm_group.set_pose_target(pose)
    #     success = self.arm_group.go(wait=True)
    #     self.arm_group.clear_path_constraints()
    #     return success

    def reach_gripper_position(self, relative_position):
        gripper_group = self.gripper_group
        
        # We only have to move this joint because all others are mimic!
        gripper_joint = self.robot.get_joint(self.gripper_joint_name)
        gripper_max_absolute_pos = gripper_joint.max_bound()
        gripper_min_absolute_pos = gripper_joint.min_bound()
        try:
            val = gripper_joint.move(relative_position * (gripper_max_absolute_pos - gripper_min_absolute_pos) + gripper_min_absolute_pos, True)
            return val
        except:
            return False 

