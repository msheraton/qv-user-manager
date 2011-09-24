using System;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void ListCals()
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                Console.WriteLine("UserName;LastUsed;QuarantinedUntil;Document;Server");

                // Loop through available servers
                foreach (var server in serviceList)
                {
                    // Get Named CALs
                    var config = backendClient.GetCALConfiguration(server.ID, CALConfigurationScope.NamedCALs);

                    foreach (var c in config.NamedCALs.AssignedCALs)
                        Console.WriteLine(String.Format("{0};{1};{2};{3};{4}", c.UserName, c.LastUsed.Year > 0001 ? c.LastUsed.ToString() : "", c.QuarantinedUntil.Year > 0001 ? c.QuarantinedUntil.ToString() : "", "", server.Name));

                    // Get Document CAL's
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    foreach (var docNode in userDocuments)
                    {
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Licensing);

                        foreach (var c in metaData.Licensing.AssignedCALs)
                          Console.WriteLine(String.Format("{0};{1};{2};{3};{4}", c.UserName, c.LastUsed.Year > 0001 ? c.LastUsed.ToString() : "", c.QuarantinedUntil.Year > 0001 ? c.QuarantinedUntil.ToString() : "", docNode.Name, server.Name));
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
