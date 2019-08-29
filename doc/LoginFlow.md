## Login flow
If everything is properly configured login flow will be as on scenario below. First click on `Login` link that by default is in top right corner of your web browser. You will be redirected to the DNN login page. Click on "Login to Auth0" button. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Login_flow_01.png)

This will redirect you to the second login pane, but this time it's on 'Auth0' platform (see web browser url). From this place you can select external Identity Provider (in this example 'Google') or sign in directly to the Auth0. 
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Login_flow_02.png)

At the end you will be redirected to the DNN website as signed-in user.
![alt text](https://raw.githubusercontent.com/BarryWaluszko/Auth0_DnnProvider/doc/doc/images/Login_flow_03.png)
If this is first time when you sign-in, user will be automatically created in DNN with basic profile. Remember that your password isn't saved in DNN, it never leaves the Identity Provider.