using System;
using System.Collections.Generic;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void ListDms(ICollection<string> documents)
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                Console.WriteLine("UserName;Document;Server");

                // Loop through available servers
                foreach (var server in serviceList)
                {
                    // Get documents on each server
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    // Loop through available documents
                    foreach (var docNode in userDocuments)
                    {
                        if (documents.Count != 0 && !documents.Contains(docNode.Name.ToLower())) continue;

                        // Get authorization meta data
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Authorization);

                        foreach (var user in metaData.Authorization.Access)
                            Console.WriteLine(String.Format("{0};{1};{2}", user.UserName, docNode.Name, server.Name));
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
