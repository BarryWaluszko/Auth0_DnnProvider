<%@ Control language="C#" Inherits="GS.Auth0.LogIn, GS.Auth0" AutoEventWireup="false"  Codebehind="LogIn.ascx.cs" %>
<li id="loginItem" runat="server" class="oauth" >
    <asp:LinkButton runat="server" ID="loginButton" CausesValidation="False">
        <span><img src="/DesktopModules/AuthenticationServices/GS_Auth0/assets/auth0-logo_v2.png" alt="Auth0 logo" height="24" width="24">&nbsp;
        <%= Localization.GetString("LoginButton", this.LocalResourceFile) %></span>
    </asp:LinkButton>
</li>