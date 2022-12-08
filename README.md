# TAIA_Survivor
Author: Marc Torres Vilagut

This repository is for the development of a university subject using Unity's and Python's MLAgents to teach an A.I. how to survive in an enviroment.

## Installation
MLAgents enviroment installation Process:

0. Install PYTHON 3.7+ and UNITY 2021.3.10f1
1. Open CMD
2. GOTO the folder of the project: `cd <path>/TAIA_Survivor`

3. Create Virtual Enviroment: `python -m venv venv`

4. Activate venv: `venv\Scripts\activate`

5. Upgrade pip: `python -m pip install --upgrade pip`

6. Install Dependencies:
	6.1. Pytorch: `pip3 install torch~=1.7.1 -f https://download.pytorch.org/whl/torch_stable.html`
	6.2. MLAgents: `pip install mlagents`
7. Check MLAgents install  
	`mlagents-learn --help`  
		It may throw an error like:  
    `<path>\venv\lib\site-packages\torch\cuda__init__.py:52: UserWarning: CUDA initialization: Found no NVIDIA driver on your system. Please check that you have an NVIDIA GPU and installed a driver from http://www.nvidia.com/Download/index.aspx (Triggered internally at  ..\c10\cuda\CUDAFunctions.cpp:100.)`  
			`return torch._C._cuda_getDeviceCount() > 0`  
		
    Its asking for the NVIDIA's CUDA Library. It's intsallation is optional. To do so:  
		  7.1. GOTO NVIDIA page [http://www.nvidia.com/Download/index.aspx] and find the needed drivers  
		  7.2. Download and Install drivers  
