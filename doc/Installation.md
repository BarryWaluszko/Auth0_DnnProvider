## Installation
Installation process is the same as for any other DNN plugin. Sign in to DNN as a 'Host' and upload extension through 'Install extension' DNN wizard. Same wizard will walk you through whole installation process. See images below for reference. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Install_01.png)
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Install_02.png)

At the end you should see similar to the figure below.
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Install_03.png)

Following changes will be automatically applied in `web.config` file:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_04.png)

Following dll's will be added to the DNN `bin` directory. All are necessary for OWIN interface. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/DNN_Configure_05.png)