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
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using NDesk.Options;

namespace qv_user_manager
{
    class Program
    {
        enum ExitCode
        {
            Success = 0,
            Error = 1
        }

        static void Main(string[] args)
        {
            if (!VerifyServerConfig())
            {
                Environment.ExitCode = (int)ExitCode.Error;
                return;
            }

            var list = "";
            var add = "";
            var remove = "";
            var docs = "";
            var prefix = "";
            var version = false;
            var help = false;

            try
            {
                var p = new OptionSet {
                    { "l|list=", "List CALs, users or documents [{CAL|DMS|DOCS}]", v => list = v.ToLower() },
                    { "a|add=", "Add users or assign CALs [{CAL|DMS}]", v => add = v.ToLower() },
                    { "r|remove=", "Remove specified users or inactive CALs [{CAL|DMS}]", v => remove = v.ToLower() },
                    { "d|document=", "QlikView document(s) to perform actions on", v => docs = v.ToLower() },
                    { "p|prefix=", "Use specified prefix for all users and CALs", v => prefix = v },
                    { "V|version", "Show version information", v => version = v != null },
                    { "?|h|help", "Show usage information", v => help = v != null },
                };

                p.Parse(args);

                if (help || args.Length == 0)
                {
                    Console.WriteLine("Usage: qv-user-manager [options]");
                    Console.WriteLine("Handles QlikView CALs and DMS user authorizations.");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    p.WriteOptionDescriptions(Console.Out);
                    Console.WriteLine();
                    Console.WriteLine("Options can be in the form -option, /option or --long-option");
                    return;
                }

                if (version)
                {
                    Console.WriteLine("qv-user-manager 20111028\n");
                    Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY.");
                    Console.WriteLine("This is free software, and you are welcome to redistribute it");
                    Console.WriteLine("under certain conditions.\n");
                    Console.WriteLine("Code: git clone git://github.com/braathen/qv-user-manager.git");
                    Console.WriteLine("Home: <https://github.com/braathen/qv-user-manager>");
                    Console.WriteLine("Bugs: <https://github.com/braathen/qv-user-manager/issues>");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Split list of multiple documents
            var documents = new List<string>(docs.Split(new[] { ';', ',', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            // Remove possible duplicate documents
            documents = documents.Distinct().ToList();

            var users = new List<string>();

            // Look for console redirection or piped data
            try
            {
                var isKeyAvailable = Console.KeyAvailable;
            }
            catch (InvalidOperationException)
            {
                string s;
                while ((s = Console.ReadLine()) != null)
                {
                    users.Add(prefix + s.Trim());
                }
            }

            // Remove possible duplicate users
            users = users.Distinct().ToList();

            switch (remove)
            {
                case "dms":
                    DocumentMetadataService.Remove(documents, users);
                    break;
                case "cal":
                    ClientAccessLicenses.Remove();
                    break;
            }

            switch (add)
            {
                case "dms":
                    DocumentMetadataService.Add(documents, users);
                    break;
                case "cal":
                    ClientAccessLicenses.Add(documents, users);
                    break;
            }

            switch (list)
            {
                case "dms":
                    DocumentMetadataService.List(documents);
                    break;
                case "cal":
                    ClientAccessLicenses.List();
                    break;
                case "docs":
                    DocumentMetadataService.DocInfo(documents);
                    break;
                case "calinfo":
                    ClientAccessLicenses.CalInfo();
                    break;
            }

            Environment.ExitCode = (int)ExitCode.Success;
        }

        /// <summary>
        /// Verify that the user has changed the server settings
        /// </summary>
        /// <returns></returns>
        private static bool VerifyServerConfig()
        {
            try
            {
                var clientSection = ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

                var propertyInformation = clientSection.ElementInformation.Properties[string.Empty];

                var endpointCollection = propertyInformation.Value as ChannelEndpointElementCollection;

                var address = endpointCollection[0].Address.ToString();

                if (address.Contains("your-server-address"))
                {
                    Console.WriteLine("The server address needs to be configured in the qv-user-manager.config file." + System.Environment.NewLine);
                    
                    Console.WriteLine("Current value: " + address);

                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }

            return true;
        }
    }
}
