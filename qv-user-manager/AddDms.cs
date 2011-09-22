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
                var backendClient = new QMSBackendClient();

                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                foreach (var server in serviceList)
                {
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    foreach (var docNode in userDocuments)
                    {
                        if (documents.Count != 0 && !documents.Contains(docNode.Name.ToLower())) continue;

                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Authorization);

                        // Remove all current users
                        var numberOfUsers = metaData.Authorization.Access.Count;
                        if (numberOfUsers > 0)
                        {
                            metaData.Authorization.Access.RemoveRange(0, numberOfUsers);
                            Console.WriteLine(String.Format("Removed {0} users from '{1}' on {2}", numberOfUsers, docNode.Name, server.Name));
                        }

                        // Add new users
                        foreach (var newUser in users.Select(userName => new DocumentAccessEntry
                                                                             {
                                                                                 UserName = userName,
                                                                                 AccessMode = DocumentAccessEntryMode.Always,
                                                                                 DayOfWeekConstraints = new List<DayOfWeek>()
                                                                             }))
                        {
                            metaData.Authorization.Access.Add(newUser);
                        }

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        Console.WriteLine(String.Format("Added {0} users to '{1}' on {2}",users.Count, docNode.Name, server.Name));
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
