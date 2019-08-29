# Auth0 Connector
A DNN provider utilizing Auth0 platform. Allows you to sign in to DNN using various accounts like: Google, Facebook, Twitter.

## Overview
The "Auth0 Connector" is a provider for DNN Platform that uses [Auth0 platform](https://auth0.com/) to authenticate users. Thanks to Auth0, that sits between DNN and the Identity Provider, users can easily utilize their Google, Facebook, etc, accounts in DNN website. At the login process new identity will be automatically created in DNN (username, displayname, email). Check [this Auth0 doc page](https://auth0.com/docs/connections) for list of all possible Identity Providers that you can connect.

"Auth0 Connector" is builded at the top of [OWIN standard](http://owin.org/)
    
## Table of Contents
* [Requirements](doc/Requirements.md)
* [Installation](doc/Installation.md)
* [Configuration](doc/Configuration.md)
* [Login flow](doc/LoginFlow.md)