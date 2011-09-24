using System;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void RemoveCals()
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                // Number of inactive days
                const int days = -30;

                // Loop through available servers
                foreach (var server in serviceList)
                {
                    /**********************
                     *     NAMED CALS
                     **********************/

                    // Get CAL configuration
                    var config = backendClient.GetCALConfiguration(server.ID, CALConfigurationScope.NamedCALs);

                    // Get number of users BEFORE modifications
                    var numberOfUsers = config.NamedCALs.AssignedCALs.Count;

                    // Iterate through all CAL's and remove the inactive ones
                    foreach (var c in config.NamedCALs.AssignedCALs.ToList().Where(u => u.LastUsed.Year > 0001 && u.LastUsed.CompareTo(DateTime.UtcNow.AddDays(days)) == -1))
                        config.NamedCALs.AssignedCALs.Remove(c);
 
                    // Save changes
                    backendClient.SaveCALConfiguration(config);

                    // Get number of users BEFORE modifications
                    var removedUsers = numberOfUsers - config.NamedCALs.AssignedCALs.Count;

                    if (removedUsers > 0)
                        Console.WriteLine(String.Format("Removed {0} CAL's on {1}", removedUsers, server.Name));

                    /**********************
                     *   DOCUMENT CALS
                     **********************/

                    // Get Document CAL's
                    var userDocuments = backendClient.GetUserDocuments(server.ID);

                    foreach (var docNode in userDocuments)
                    {
                        // Get licensing meta data
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Licensing);

                        // Get number of users BEFORE modifications
                        numberOfUsers = metaData.Licensing.AssignedCALs.Count;

                        // Iterate through all CAL's and remove the inactive ones
                        foreach (var c in metaData.Licensing.AssignedCALs.ToList().Where(u => u.LastUsed.Year > 0001 && u.LastUsed.CompareTo(DateTime.UtcNow.AddDays(days)) == -1))
                            metaData.Licensing.AssignedCALs.Remove(c);

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        // Get number of users AFTER modifications
                        removedUsers = numberOfUsers - metaData.Licensing.AssignedCALs.Count;

                        if (removedUsers > 0)
                            Console.WriteLine(String.Format("Removed {0} DocumentCAL's from '{1}' on {2}", removedUsers, docNode.Name, server.Name));
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
