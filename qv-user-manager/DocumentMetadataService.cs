/*
The MIT License (MIT)

Copyright (c) 2011 Rikard Braathen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using qv_user_manager.QMSBackendService;

namespace qv_user_manager
{
    class DocumentMetadataService
    {
        /// <summary>
        /// Add DMS users
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="users"></param>
        public static void Add(ICollection<string> documents, ICollection<string> users)
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

                        // Filter users already in DMS from the supplied list of users to avoid duplicates
                        var uniqueUsers = users.Except(metaData.Authorization.Access.Select(user => user.UserName).ToList());

                        // Get number of users on each document
                        var numberOfUsers = metaData.Authorization.Access.Count;

                        // Add new users
                        foreach (var user in uniqueUsers.Select(u => new DocumentAccessEntry
                        {
                            UserName = u,
                            AccessMode = DocumentAccessEntryMode.Always,
                            DayOfWeekConstraints = new List<DayOfWeek>()
                        }))
                        {
                            metaData.Authorization.Access.Add(user);
                        }

                        // Save changes
                        backendClient.SaveDocumentMetaData(metaData);

                        // Get number of users AFTER modifications
                        var addedUsers = metaData.Authorization.Access.Count - numberOfUsers;

                        if (addedUsers <= 0)
                            Console.WriteLine(String.Format("Nothing to add to '{0}' on {1}", docNode.Name, server.Name));
                        else
                            Console.WriteLine(String.Format("Added {0} users to '{1}' on {2}", addedUsers, docNode.Name, server.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// List DMS users
        /// </summary>
        /// <param name="documents"></param>
        public static void List(ICollection<string> documents)
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

        /// <summary>
        /// Remove DMS users
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="users"></param>
        public static void Remove(ICollection<string> documents, List<string> users)
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

                        if (removedUsers <= 0)
                            Console.WriteLine(String.Format("Nothing to remove from '{0}' on {1}", docNode.Name, server.Name));
                        else
                            Console.WriteLine(String.Format("Removed {0} users from '{1}' on {2}", removedUsers, docNode.Name, server.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DocInfo(ICollection<string> documents)
        {
            try
            {
                // Initiate backend client
                var backendClient = new QMSBackendClient();

                // Get a time limited service key
                ServiceKeyClientMessageInspector.ServiceKey = backendClient.GetTimeLimitedServiceKey();

                // Get available QlikView Servers
                var serviceList = backendClient.GetServices(ServiceTypes.QlikViewServer);

                Console.WriteLine("Document;Server;Preloaded;LoadedDays;LoadedBetween;Plugin;Mobile;AjaxZfc;Download;Category;SourceDocument");

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
                        var metaData = backendClient.GetDocumentMetaData(docNode, DocumentMetaDataScope.All);

                        // Check if PreloadMode is Restricted, if so get the dates
                        var preloadMode = metaData.Server.Preload.Mode.ToString();
                        var loadedDays = "";
                        var between = "";
                        if (preloadMode == "Restricted")
                        {
                            loadedDays = metaData.Server.Preload.DaysOfWeek.Aggregate(loadedDays, (current, dayOfWeek) => current + (dayOfWeek.ToString().Substring(0, 2) + " ")).Trim();
                            between = metaData.Server.Preload.StartTime.ToShortTimeString() + "-" + metaData.Server.Preload.EndTime.ToShortTimeString();
                        }

                        // Check which clients are enabled
                        var accessMethods = metaData.Server.Access.Methods.ToString();
                        var pluginClient = accessMethods.Contains("PluginClient") ? 1 : 0;
                        var ajaxClient = accessMethods.Contains("ZeroFootprintClient") ? 1 : 0;
                        var download = accessMethods.Contains("Download") ? 1 : 0;
                        var mobileClient = accessMethods.Contains("MobileClient") ? 1 : 0;

                        // Check if we have subfolders
                        var relativePath = docNode.RelativePath;
                        if (relativePath != "")
                            relativePath += "\\";

                        Console.WriteLine(String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", relativePath + docNode.Name,
                                                        server.Name, preloadMode, loadedDays, between, pluginClient, mobileClient, ajaxClient, download, metaData.DocumentInfo.Category, metaData.DocumentInfo.SourceName));
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
