# ROS-Docker Framework for Digital Twin Applications
[Yehor Karpichev](https://scholar.google.com/citations?user=eDsR_O0AAAAJ&hl=en), [Mahmoud Chick Zaouali](https://scholar.google.com/citations?hl=en&user=nYQwIk4AAAAJ), [Todd Charter](https://scholar.google.com/citations?user=7qJcX5IAAAAJ&hl=en), [Homayoun Najjaran](https://scholar.google.com/citations?hl=en&user=HQ7bYVkAAAAJ)

add video demo
<!-- [![Video DEMO](https://img.youtube.com/vi/AP2FHY7DFlo/maxresdefault.jpg)](https://youtu.be/AP2FHY7DFlo) -->

This repository contains the official implementation associated with the paper [**"A Deployable and Scalable ROS-Docker Framework for Multi-Platform Digital Twin Applications"**](https://arxiv.org/). 
<!--
<section class="section" id="BibTeX">
  <div class="container is-max-desktop content">
    <h2 class="title">BibTeX</h2>
    <pre><code>@article{karpichev2025deployable,
  title={A Deployable and Scalable ROS-Docker Framework for Multi-Platform Digital Twin Applications},
  author={Karpichev, Yehor and Chick Zaouali, Mahmoud and Charter, Todd and Najjaran, Homayoun},
  journal={arXiv preprint arXiv:2312.16084},
  year={2025}
}</code></pre>
  </div>
</section>
-->

## Overview 
general overview - what the project is about + how it works


## Installation
Note: Please use the main branch if you clone the repository, other branches are currently used for development. The pipeline has been developed and tested on the Windows 10 machine.

**Setup technologies:**
Since we utilize Docker for creating a containarized environment, the project required an installation only of Docker and Unity game engine. For running RViz on Windows machine, also an X server is needed, and we explain it in the Display forwarding section.
- [Docker](https://www.docker.com/products/docker-desktop/)
- [Unity](https://unity.com/products/unity-engine)

### Unity
The implementation includes a virtual replica of the research lab with a the Kinova Gen3 robot model. When utilizing the repository, the project should be complete and ready to go. We recommend using the same Unity version (2021.3.45f), but it should work with other versions as well. 

### Setup Docker environment:
```shell
docker-compose build
```
and then:
```shell
docker-compose up
```

Alternatively: after building an image, instead of the "docker-compose up" the following command can be used:
```shell
docker run -it -p 10000:10000 IMAGE_NAME /bin/bash
```
### Launch commands:
After you've statrted your container, multiple launch commands must be run to a) start the robot driver & the vision module, b) run the unity endpoint, and c) run the scripts for starting a custom ros node for the data flow from the DT environment.

Assuming you are in the ~/catkin_workspace envrionment: 

**Kinova Driver:**
```shell
roslaunch kortex_driver kortex_driver.launch ip_address:=192.168.0.10 dof:=6 gripper:=robotiq_2f_85
```
Modify ip address, dof, the gripper, and other arguments to suit a specific model. More details are available in the Kinova [ros_kortex](https://github.com/Kinovarobotics/ros_kortex/tree/noetic-devel/kortex_driver) repo.


**Kinova Vision:** 
```shell
roslaunch kinova_vision kinova_vision_rgbd.launch device:=192.168.0.10
```
Our robot has a vision module, and a separate driver from Kinova must be launched to access it. Modify the IP and other parameters as described in the official documentation [here](https://github.com/Kinovarobotics/ros_kortex_vision).

**Unity Endpoint:**
```shell
roslaunch ros_tcp_endpoint endpoint.launch
```
You can add parameter like tcp_ip and tcp_port to specify which you wish to use. The default is tcp_ip=0.0.0.0, and tcp_port=10000. Alternatively, you may edit the endpoint.launch file directly. If port is modified, ensure that a new port is exposed when launch tyhe Docker container. 

Official github page by Unity-Technologies: [unity-endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint).


**Our Python scipt:**
```shell
python src/python_scripts/robot_control.py
```
We've developed a couple custom python scripts that launch a separate ros node & topics for passing the data from the developed Digital Twin to the MoveIt interface for controlling the physical system.



### Display Forwarding

Display forwarding is needed in order to run the Linux-native applications on the windows host machine. For example, we run and control the robot from RViz. The config line regarding the display forwarding is added as part of the docker-compose to ensure that applications like RViz and Gazebo will run smoothly. 

However, if facing any issues, the following command can be executed directly from the container environment as well: 
```shell
export DISPLAY=host.docker.internal:0
```

Note that in order to make sure that RViz launches and works properly, the installation of an X server is required. In our implementation we used the free distributions of the [VcXsrv Windows X Server](https://sourceforge.net/projects/vcxsrv/)

Upon installaton, open the application, and you may experiment with different setups, however, we recommend the settings indicated in [X Server setup]().



#### Docker commands
We list some of the basic Docker command below as these might come in handy when working with Docker images/containers. All commands are executed from the windowds terminal.

(i) To check all exiting containers:
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

(iv) To stop a running container using its ID:
```shell
docker stop CONTAINER_ID
```

(v) Delete a container:
```shell
docker rm CONTAINER_ID
```

(vi) Check existing images: 
```shell
docker images
```

(vii) Delete an image:
```shell
docker rmi IMAGE_ID
```

## TODO list:
- [ ] Update docker-compose with entrypoints (and hence replace the launch commands)
- [ ] Docker multicontainer distribution (run kortex_vision from a separate container)
- [ ] Add support for simulation (for when the phsycial robot is not available)
- [ ] Finalize gripper & virtual twin functionality in Unity
- [ ] Implement Cartesian-based control for Gen3 in Unity
- [ ] Add ROS2 implementation



