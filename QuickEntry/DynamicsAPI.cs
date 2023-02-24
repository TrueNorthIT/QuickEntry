using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace QuickEntry
{
    public class DynamicsAPI
    {
        private Guid userGuid;
        private Guid bookableGuid;
        private CrmServiceClient service;

        public static T GetAliasedValue<T>(Entity e, string key) 
        {
            var a = e.GetAttributeValue<AliasedValue>(key);
            var v = a.Value;
            return (T)v;
        }

        private CrmServiceClient Authenticate(string cachePath, string email, string crmUrl, string prompt="Auto")
        {
            string ConnectionString = "AuthType = Office365;" +
            $"Username = {email};" +
            $"Url = {crmUrl};" + //https://truenorthit.crm4.dynamics.com
            $"LoginPrompt={prompt}";

            if (prompt == "Auto") ConnectionString += $"TokenCacheStorePath={cachePath}/msal_cache.data";

            CrmServiceClient svc = new CrmServiceClient(ConnectionString);

            return svc;
        }

        private void GetUserIDFromEmail(string email)
        {
            var query = new QueryExpression("systemuser");
            query.ColumnSet.AddColumns("internalemailaddress", "systemuserid");
            query.Criteria.AddCondition("internalemailaddress", ConditionOperator.Equal, email);
            var users = service.RetrieveMultiple(query);
            if (users.Entities.Count == 0) throw new Exception("This email is not associated with a system user!");
            userGuid = users.Entities.First().Id;
        }

        private void GetBookableFromUserId()
        {

            var query = new QueryExpression("bookableresource");
            query.ColumnSet.AddColumns("bookableresourceid");
            query.Criteria.AddCondition("userid", ConditionOperator.Equal, userGuid);
            var bookable = service.RetrieveMultiple(query);
            bookableGuid = bookable.Entities.First().Id;
        }

        public EntityCollection GetUsersProjects()
        {
            var projects = new Dictionary<Guid, Project>();
            var query = new QueryExpression("msdyn_project");
            query.ColumnSet.AddColumns("msdyn_subject");
            query.Criteria.AddCondition("msdyn_projectteam", "msdyn_bookableresourceid", ConditionOperator.Equal, bookableGuid);

            var query_msdyn_projectteam = query.AddLink("msdyn_projectteam", "msdyn_projectid", "msdyn_project");
            query_msdyn_projectteam.Columns.AddColumns("msdyn_bookableresourceid", "msdyn_resourcecategory");
            var query_msdyn_projecttask = query.AddLink("msdyn_projecttask", "msdyn_projectid", "msdyn_project");
            query_msdyn_projecttask.Columns.AddColumns("msdyn_subject", "msdyn_projecttaskid");

            var result = service.RetrieveMultiple(query);
            return result;
        }

        public Guid AddTimesheetEntry(int duration, string title, string comments, EntityReference project, EntityReference task, EntityReference resource )
        {
            var timeEntry = new Entity("msdyn_timeentry")
            {
                ["msdyn_date"] = DateTime.UtcNow,
                ["msdyn_duration"] = duration,
                ["msdyn_description"] = title,
                ["msdyn_externaldescription"] = comments,
                ["ownerid"] = new EntityReference("systemuser", userGuid),
                ["msdyn_project"] = project,
                ["msdyn_projecttask"] = task,
                ["msdyn_bookableresource"] = resource
            };

            return service.Create(timeEntry);
        }

        public DynamicsAPI(string cahcePath, string email, string crmUrl) 
        {
            // Try login with seemless signin if we have any exceptions then show the login prompt
            service = Authenticate(cahcePath, email, crmUrl);
            if (service.LastCrmException != null) service = Authenticate(cahcePath, email, crmUrl, "Always");


            // OK now let's try and get the users GUID from their email
            GetUserIDFromEmail(email);
            GetBookableFromUserId();


        }

    }
}
