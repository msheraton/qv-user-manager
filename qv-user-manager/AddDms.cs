using System;
using System.Collections.Generic;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        public static void AddDms(ICollection<string> documents, ICollection<string> users)
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                // Loop through available servers
                foreach (var server in serviceList)
                {
                    // Get documents on each server
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    // Loop through available documents
                    foreach (var docNode in userDocuments)
                    {
                        // Continue if no matching documents
                        if (documents.Count != 0 && !documents.Contains(docNode.Name.ToLower())) continue;

                        // Get authorization metadata
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Authorization);

                        // Get number of users on each document
                        var numberOfUsers = metaData.Authorization.Access.Count;

                        // Add new users
                        foreach (var user in users.Select(u => new DocumentAccessEntry
                        {
                            UserName = u,
                            AccessMode = DocumentAccessEntryMode.Always,
                            DayOfWeekConstraints = new List<DayOfWeek>()
                        }))
                        {
                            var exist = false;

                            var dmsuser = metaData.Authorization.Access.ToList();

                            var user1 = user.UserName.ToLower();

                            // Check if the username exists already
                            foreach (var current in dmsuser.Where(current => current.UserName.ToLower() == user1))
                                exist = true;

                            if (!exist)
                                metaData.Authorization.Access.Add(user);
                        }

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        // Get number of users AFTER modifications
                        var addedUsers = metaData.Authorization.Access.Count - numberOfUsers;

                        if (addedUsers > 0)
                            Console.WriteLine(String.Format("Added {0} users to '{1}' on {2}", addedUsers, docNode.Name, server.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
