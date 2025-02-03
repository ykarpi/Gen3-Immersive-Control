FROM ros:noetic-ros-core-focal

# install bootstrap tools
RUN apt-get update && apt-get install --no-install-recommends -y \
    build-essential \
    python3-rosdep \
    python3-rosinstall \
    python3-vcstools \
    git \    
    python3 python3-pip \
    sudo \
    && rm -rf /var/lib/apt/lists/*

# Link Python 3 to /usr/bin/python
RUN ln -s /usr/bin/python3 /usr/bin/python

# Install Conan
RUN python3 -m pip install conan==1.59 && \
    conan config set general.revisions_enabled=1 && \
    conan profile new default --detect > /dev/null && \
    conan profile update settings.compiler.libcxx=libstdc++11 default


RUN mkdir -p /root/catkin_workspace/src && \
    cd /root/catkin_workspace/src && \
    git clone -b noetic-devel https://github.com/Kinovarobotics/ros_kortex.git && \
    git clone https://github.com/Kinovarobotics/ros_kortex_vision.git && \
    git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git && \
    git clone https://github.com/moveit/moveit.git

# Copy python_scripts folder
COPY python_scripts /root/catkin_workspace/src/python_scripts

# Bootstrap rosdep
RUN apt-get update && \
    apt-get install -y ros-noetic-gazebo-ros && \
    rosdep init && \
    rosdep update --rosdistro $ROS_DISTRO && \
    rosdep install --from-paths /root/catkin_workspace/src --ignore-src -y || true

#Iinstall ros packages
RUN apt-get update && apt-get install -y --no-install-recommends \
    ros-noetic-ros-base=1.5.0-1* \
    ros-noetic-catkin \
    ros-noetic-rgbd-launch \
    && rm -rf /var/lib/apt/lists/* 

# Install GStreamer dependencies
RUN apt-get update && apt-get install -y \
    gstreamer1.0-tools \
    gstreamer1.0-libav \
    libgstreamer1.0-dev \
    libgstreamer-plugins-base1.0-dev \
    libgstreamer-plugins-good1.0-dev \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-base \
    && rm -rf /var/lib/apt/lists/*

# Install additional dependencies
RUN apt-get update && apt-get install -y \
    libxcb-xinerama0 \
    libxcb-xinput0 \
    libx11-dev \
    libgl1-mesa-glx \
    libxcb1 \
    libglu1-mesa \
    qt5-qmake \
    qtbase5-dev \
    qtchooser \
    iputils-ping \
    ros-noetic-camera-info-manager \
    ros-noetic-rqt-image-view \
    ros-noetic-camera-calibration-parsers \
    ros-noetic-image-transport-plugins \
    && rm -rf /var/lib/apt/lists/*


# Set up the ROS environment
RUN echo "source /opt/ros/noetic/setup.bash" >> /root/.bashrc 

# Build the workspace
RUN /bin/bash -c "source /opt/ros/noetic/setup.bash && cd /root/catkin_workspace && catkin_make"

RUN echo "source /root/catkin_workspace/devel/setup.bash" >> /root/.bashrc

# Set default working directory
WORKDIR /root/catkin_workspace
