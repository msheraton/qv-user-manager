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
                var backendClient = new QMSBackendClient();

                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                Console.WriteLine("UserName;Document;Server");

                foreach (var server in serviceList)
                {
                    // Get DMS Authorizations
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    foreach (var docNode in userDocuments)
                    {
                        if (documents.Count != 0 && !documents.Contains(docNode.Name.ToLower())) continue;

                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Authorization);

                        foreach (var user in metaData.Authorization.Access)
                        {
                            Console.WriteLine(String.Format("{0};{1};{2}", user.UserName, docNode.Name, server.Name));
                        }
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
