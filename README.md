# Auth0 Connector
A DNN provider utilizing Auth0 platform. Allows you to sign in to DNN using various accounts like: Google, Facebook, Twitter.

## Overview
The "Auth0 Connector" is a provider for DNN Platform that uses [Auth0 platform](https://auth0.com/) to authenticate users. Thanks to Auth0, that sits between DNN and the Identity Provider, users can easily utilize their Google, Facebook, etc, accounts in DNN website. At the login process new identity will be automatically created in DNN (username, displayname, email). Check [this Auth0 doc page](https://auth0.com/docs/connections) for list of all possible Identity Providers that you can connect.

"Auth0 Connector" is builded at the top of [OWIN standard](http://owin.org/)

## Requirements
* DNN Platform v8.0.1 or later
* Auth0 account

## Installation
Installation process is the same as for any other DNN plugin. Sign in to DNN as a 'Host' and upload extension through 'Install extension' DNN wizard. Same wizard will walk you through whole installation process. See images below for reference. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Install_01.png)
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Install_02.png)

At the end you should see similar to the figure below.
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Install_03.png)

Following changes will be automatically applied in `web.config` file:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_04.png)

Following dll's will be added to the DNN `bin` directory. All are necessary for OWIN interface. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_05.png)

## Configuration
Configuration process is devided into two sections. First you need to configure connection on Auth0 side. After that coordinates generated on Auth0 (Domain, Client ID, Client Secret) needs to be entered in DNN website.
### Configuration at the Auth0 platform
First you need to have an account in Auth0 platform, where you can create new 'Regular Web Application'. This 'Application' will be some kind of proxy bridge between DNN and Identity Providers (Google, Facebook). Whole process is welly described on Auth0 doc, see here for reference [Register a Regular Web Application](https://auth0.com/docs/dashboard/guides/applications/register-app-regular-web).

Figure below presents typical 'Application'. Please copy paste following attributes: `Domain`, `Client ID`, `Client Secret`. See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Auth0_Configure_02.png)

For **Application Type** select `Regular Web Application` needs to be selected.

Inside field **Allowed Callback URLs** enter DNN login url. See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Auth0_Configure_03.png)

Inside filed **Allowed Logout URLs** enter DNN logoff url, usually it's something like http://MyDnnDomain.com/logoff.
See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Auth0_Configure_04.png)

### Configuration on the DNN website
Sign in to DNN as a Host user and go to the `Extensions` list. Click on pencil icon where 'Auth0 Connector' is, see figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_01.png)
Then select `Site Settings` tab. Fill the form according to figure below. Click `Update` button to keep the changes.
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_02.png)

Inspect the `web.config` file, check if under `<appSettings>` node is line: 
```
<add key="owin:AppStartup" value="GS_Auth0" />
```
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/DNN_Configure_03.png)
This will enable OWIN in your DNN and marks 'Auth0 Provider' as an OWIN application. 

## Login flow
If everything is properly configured login flow will be as on figures below. First click on `Login` link that by default is in top right corner of your web browser. This will redirect you to the login pane. Click on "Login to Auth0" button. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Login_flow_01.png)

This will redirect to the second login pane, but this time it's on 'Auth0' platform (see web browser url). From this place you can select external Identity Provider (in this example 'Google') or sign in directly to the Auth0. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/images/Login_flow_02.png)

At the end you will be redirected to the DNN website as signed-in user. For first time, user will be automatically created in DNN. User profile is created, but your password isn't saved in DNN.
