# Gazebo simulation

The branch is currently under development, and some functionality for the VR control still might be limited. 

For the general project overview and more infromation on the Docker setup as well as our comments regarding the launch commands, please refer to the [main](https://github.com/ykarpi/Gen3-Immersive-Control/tree/main) branch.

The screen recordings of Gazebo and RViz are availble [here](https://github.com/ykarpi/Gen3-Immersive-Control/tree/gazebo-support/media/Gazebo).

<p align="center">
  <img src="https://github.com/ykarpi/Gen3-Immersive-Control/blob/gazebo-support/media/Gazebo/gif/Gen3-Gazebo.gif" alt="Gazebo simulation">
</p>

<p align="center">
  <img src="https://github.com/ykarpi/Gen3-Immersive-Control/blob/gazebo-support/media/Gazebo/gif/Kinova-RViz-Gazebo-simulation.gif" alt="RViz control for Gazebo simulation">
</p>


### Launch commands with simulated arm

Assuming you've started the docker container, and currently in `~/catkin_environment`, the following commands should be executed:

**Gazebo Kinova Driver:**
```shell
roslaunch kortex_gazebo spawn_kortex_robot.launch arm:=gen3 dof:=6 gripper:=robotiq_2f_85
```
More info on the gazebo implementation and kortex control at the Kinova's official [github](https://github.com/Kinovarobotics/ros_kortex/tree/noetic-devel/kortex_gazebo). Unfortunately, there is no possibility to simulate the vision module without the physical model.

**Unity Endpoint:**
```shell
roslaunch ros_tcp_endpoint endpoint.launch
```

**Python scipt:**
```shell
python src/python_scripts/robot_control.py __ns:=my_gen3
```


