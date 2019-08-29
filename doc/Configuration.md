## Configuration
Configuration process is devided into two sections. First you need to configure connection on Auth0 side. After that coordinates generated on Auth0 (Domain, Client ID, Client Secret) needs to be entered in DNN website.
### Configuration at the Auth0 platform
First you need to have an account in Auth0 platform, where you can create new 'Regular Web Application'. This 'Application' will be some kind of proxy bridge between DNN and Identity Providers (Google, Facebook). Whole process is welly described on Auth0 doc, see here for reference [Register a Regular Web Application](https://auth0.com/docs/dashboard/guides/applications/register-app-regular-web).

Figure below presents typical 'Application'. Please copy paste following attributes: `Domain`, `Client ID`, `Client Secret`. See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Auth0_Configure_02.png)

For **Application Type** select `Regular Web Application` needs to be selected.

Inside field **Allowed Callback URLs** enter DNN login url. See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Auth0_Configure_03.png)

Inside filed **Allowed Logout URLs** enter DNN logoff url, usually it's something like http://MyDnnDomain.com/logoff.
See figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Auth0_Configure_04.png)

### Configuration on the DNN website
Sign in to DNN as a Host user and go to the `Extensions` list. Click on pencil icon where 'Auth0 Connector' is, see figure below for reference:
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/DNN_Configure_01.png)
Then select `Site Settings` tab. Fill the form according to figure below. Click `Update` button to keep the changes.
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/DNN_Configure_02.png)

Inspect the `web.config` file, check if under `<appSettings>` node is line: 
```
<add key="owin:AppStartup" value="GS_Auth0" />
```
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/DNN_Configure_03.png)
This will enable OWIN in your DNN and marks 'Auth0 Provider' as an OWIN application. 