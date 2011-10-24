using System;
using System.Collections.Generic;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    partial class Program
    {
        static void AddCals(ICollection<string> documents, ICollection<string> users)
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                if (documents.Count == 0)
                // NAMED CALS
                {
                    // Loop through available servers
                    foreach (var server in serviceList)
                    {
                        // Get Named CALs
                        var config = backendClient.GetCALConfiguration(server.ID, CALConfigurationScope.NamedCALs);

                        // Get number of users BEFORE modifications
                        var numberOfCals = config.NamedCALs.AssignedCALs.Count;

                        // Add CAL's (already existing CAL's seems unaffected, but how to check for them? Is it necessary?)
                        foreach (var user in users.Select(u => new AssignedNamedCAL
                        {
                            UserName = u.ToUpper(),
                        }))
                        {
                            config.NamedCALs.AssignedCALs.Add(user);
                        }

                        // Save changes
                        backendClient.SaveCALConfiguration(config);

                        // Get number of users AFTER modifications
                        var addedCals = config.NamedCALs.AssignedCALs.Count - numberOfCals;

                        if (addedCals <= 0)
                            Console.WriteLine(String.Format("Nothing to add on {0}", server.Name));
                        else
                            Console.WriteLine(String.Format("Added {0} CALs on {1}", addedCals, server.Name));

                        var inLicense = config.NamedCALs.InLicense;
                        var assigned = config.NamedCALs.Assigned;

                        // Warn if not enough available CAL's
                        if (addedCals >= inLicense)
                            Console.WriteLine(String.Format("WARNING: Attempted to assign {0} CALs on {1} but the license only allows {2} CALs.", addedCals, server.Name, inLicense));
                        else if (assigned >= inLicense)
                            Console.WriteLine("WARNING: All available CALs in the license have been assigned.");
                    }
                }
                // DOCUMENT CALS
                else
                {
                    // Loop through available servers
                    foreach (var server in serviceList)
                    {
                        // Get documents on each server
                        var userDocuments = backendClient.GetUserDocuments(server.ID);

                        // Loop through available documents
                        foreach (var docNode in userDocuments)
                        {
                            // Continue if no matching documents
                            if (!documents.Contains(docNode.Name.ToLower())) continue;

                            var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.Licensing);

                            // Get allocated CAL's for document
                            var allocatedCals = metaData.Licensing.CALsAllocated;

                            // Allocate more CAL's if necessary
                            if (users.Count > allocatedCals)
                                metaData.Licensing.CALsAllocated = users.Count();

                            // Add document CAL's
                            foreach (var user in users.Select(u => new AssignedNamedCAL
                            {
                            UserName = u
                            }))
                            {
                                metaData.Licensing.AssignedCALs.Add(user);
                            }

                            // Save changes
                            backendClient.SaveDocumentMetaData(metaData);

                            Console.WriteLine(String.Format("Added {0} Document CALs to '{1}' on {2}", users.Count(), docNode.Name, server.Name));
                        }
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
