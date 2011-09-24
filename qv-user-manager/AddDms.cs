using System;
using System.Collections.Generic;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void AddDms(ICollection<string> documents, ICollection<string> users)
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

                        // Remove all current users before adding new ones
                        if (numberOfUsers > 0)
                        {
                            metaData.Authorization.Access.RemoveRange(0, numberOfUsers);

                            Console.WriteLine(String.Format("Removed {0} users from '{1}' on {2}", numberOfUsers, docNode.Name, server.Name));
                        }

                        // Add new users
                        foreach (var newUser in users.Select(u => new DocumentAccessEntry
                        {
                            UserName = u,
                            AccessMode = DocumentAccessEntryMode.Always,
                            DayOfWeekConstraints = new List<DayOfWeek>()
                        }))
                        {
                            metaData.Authorization.Access.Add(newUser);
                        }

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        Console.WriteLine(String.Format("Added {0} users to '{1}' on {2}", users.Count, docNode.Name, server.Name));
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
