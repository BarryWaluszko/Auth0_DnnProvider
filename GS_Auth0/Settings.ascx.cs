/*
' Copyright (c) 2019 Glanton
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Authentication.OAuth;
using DotNetNuke.UI.WebControls;
using GS.Auth0.Components;

namespace GS.Auth0
{
    public partial class Settings : AuthenticationSettingsBase
    {
        protected PropertyEditorControl SettingsEditor;

        protected string AuthSystemApplicationName
        {
            get { return Constants.PROVIDER_NAME; }
        }

        public override void UpdateSettings()
        {
            if (SettingsEditor.IsValid && SettingsEditor.IsDirty)
            {
                var config = (Auth0ConfigBase)SettingsEditor.DataSource;                
                Auth0ConfigBase.UpdateConfig(config);
            }
        }
       
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Auth0ConfigBase config = Auth0ConfigBase.GetConfig(AuthSystemApplicationName, PortalId);
            SettingsEditor.DataSource = config;
            SettingsEditor.DataBind();
        }
    }

}

