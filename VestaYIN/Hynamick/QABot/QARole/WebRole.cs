using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace QARole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += this.OnRoleEnvironmentChanging;

            return base.OnStart();
        }

        private void OnRoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            var configurationChanges = e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>();

            if (configurationChanges.Any())
            {
                // If Azure settings changed, take the instance offline, applies the configuration change, and then brings the instance back online.
                // For more details, please refer to this article: https://msdn.microsoft.com/en-us/library/azure/gg432963.aspx
                e.Cancel = true;
            }
        }
    }
}
