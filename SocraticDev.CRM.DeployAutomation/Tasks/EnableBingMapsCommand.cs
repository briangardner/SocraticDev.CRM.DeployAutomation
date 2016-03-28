using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace SocraticDev.CRM.DeployAutomation.Tasks
{
    internal class EnableBingMapsCommand : DeployerBaseCommand<EnableBingMapsSettings>
    {
        public EnableBingMapsCommand(IOrganizationService service) : base(service) { }

        internal EnableBingMapsCommand(IOrganizationService service, EnableBingMapsSettings settings) : base(service, settings) { }

        public override void Execute()
        {
            var orgs = Service.RetrieveMultiple(new QueryExpression("organization")
            {
                ColumnSet = new ColumnSet("enablebingmapsintegration")
            });
            var org = orgs.Entities.FirstOrDefault();
            if (org == null)
            {
                throw new Exception("No Orgs found");
            }
            bool bingEnabled;
            bool.TryParse(org.Attributes["enablebingmapsintegration"].ToString(), out bingEnabled);
            if (bingEnabled == Settings.EnableBingMaps)
            {
                return;
            }
            org.Attributes["enablebingmapsintegration"] = Settings.EnableBingMaps;
            Service.Update(org);
        }
    }

    public class EnableBingMapsSettings
    {
        public bool EnableBingMaps { get; set; }
    }
}
