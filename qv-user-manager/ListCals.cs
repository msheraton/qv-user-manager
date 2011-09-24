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

                    var assignedCal = config.NamedCALs.AssignedCALs;

                    foreach (var namedCal in assignedCal)
                        Console.WriteLine(String.Format("{0};{1};{2};{3};{4}", namedCal.UserName, namedCal.LastUsed.ToString().Replace("0001-01-01 00:00:00", ""), namedCal.QuarantinedUntil.ToString().Replace("0001-01-01 00:00:00", ""), "", server.Name));

                    // Get Document CAL's
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    foreach (var docNode in userDocuments)
                    {
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Licensing);

                        foreach (var namedCal in metaData.Licensing.AssignedCALs)
                            Console.WriteLine(String.Format("{0};{1};{2};{3};{4}", namedCal.UserName, namedCal.LastUsed.ToString().Replace("0001-01-01 00:00:00", ""), namedCal.QuarantinedUntil.ToString().Replace("0001-01-01 00:00:00", ""), docNode.Name, server.Name));
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
