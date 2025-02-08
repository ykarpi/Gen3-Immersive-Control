# ROS-Docker Framework for Digital Twin Applications

general overview - what the project is about + how it works

## Installation
Note: Please use the main branch if you clone the repository, other branches are currently used for development.

breif intro on used setup & technologies

#### Build an image:
```shell
docker-compose build
```
and then:
```shell
docker-compose up
```

Alternatively: after building an image, use the following:
```shell
docker run -it -p 10000:10000 IMAGE_NAME /bin/bash
```
#### Launch commands:

Assuming you are in the ~/catkin_workpsace envrionment: 

for Kinova Driver:
```shell
roslaunch kortex_driver kortex_driver.launch ip_address:=192.168.0.10 dof:=6 gripper:=robotiq_2f_85
```
for Kinova Vision: 
```shell
roslaunch kinova_vision kinova_vision_rgbd.launch device:=192.168.0.10
```
for Unity endpoint:
```shell
roslaunch ros_tcp_endpoint endpoint.launch
```
for bidirectional connection:
```shell
python src/python_scripts/robot_control.py
```

Optionally (separatly) you can launch the following to just view the robot in RViz:
```shell
roslaunch kortex_description visualize.launch dof:=6
```

#### Issues and other Docker commands

display forwarding:
```shell
export DISPLAY=host.docker.internal:0
```

(i) To check all containers, including running, stopped and exited ones:
```shell
docker ps -a
```
(ii) To start a stopped container using its ID:
```shell
docker start CONTAINER_ID
```

(iii) To launch an interactive bash shell inside a running container: 
```shell
docker exec -it CONTAINER_ID /bin/bash
```

(iiii) To stop a running container using its ID
```shell
docker stop CONTAINER_ID
```

## Unity
- Discuss Unity implementation and Interactions

## TODO list:
- [ ] Update docker-compose with entrypoints (and hence replace the launch commands)
- [ ] Docker multicontainer distribution (run kortex_vision from a separate container)
- [ ] Finalize gripper & virtual twin functionality in Unity
- [ ] Implement Cartesian-based control for Gen3 in Unity
- [ ] Add ROS2 implementation



