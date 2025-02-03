
# Build an image:
docker-compose build

and then:
docker-compose up

Alternatively: after building an image, use the following:
docker run -it -p 10000:10000 IMAGE_NAME /bin/bash

# Launch commands:

Assuming you are in the ~/catkin_workpsace envrionment: 

for Kinova Driver --> roslaunch kortex_driver kortex_driver.launch ip_address:=192.168.0.10 dof:=6 gripper:=robotiq_2f_85

for Kinova Vision --> roslaunch kinova_vision kinova_vision_rgbd.launch device:=192.168.0.10

for Unity endpoint --> roslaunch ros_tcp_endpoint endpoint.launch

for bidirectional connection --> python src/python_scripts/robot_control.py

Optionally (separatly) you can launch the following to just view the robot in RViz --> roslaunch kortex_description visualize.launch dof:=6


# Display Forwarding 
display forwarding --> export DISPLAY=host.docker.internal:0

# Other Docker commands

(i) docker ps -a

(i) docker start CONTAINER_ID

(ii) docker exec -it CONTAINER_ID /bin/bash

(iii) docker stop CONTAINER_ID




