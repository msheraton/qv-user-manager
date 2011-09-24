using System;
using System.Collections.Generic;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void RemoveDms(ICollection<string> documents, List<string> users)
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                // Convert all usernames to lowercase
                users = users.ConvertAll(d => d.ToLower());

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

                        // Get number of users BEFORE modifications
                        var numberOfUsers = metaData.Authorization.Access.Count;

                        if (users.Count == 0)
                            // Remove all users if no users were specified
                            metaData.Authorization.Access.RemoveRange(0, numberOfUsers);
                        else
                        {
                            // Remove matching users
                            foreach (var u in metaData.Authorization.Access.ToList().Where(u => users.Contains(u.UserName.ToLower())))
                                metaData.Authorization.Access.Remove(u);
                        }

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        // Get number of users AFTER modifications
                        var removedUsers = numberOfUsers - metaData.Authorization.Access.Count;

                        if (removedUsers > 0)
                            Console.WriteLine(String.Format("Removed {0} users from '{1}' on {2}", removedUsers, docNode.Name, server.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: " + ex.Message);
            }

        }
    }
}
