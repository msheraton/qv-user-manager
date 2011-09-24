using System;
using System.Collections.Generic;
using NDesk.Options;

namespace qv_user_manager
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var list = "";
            var add = "";
            var remove = "";
            var documents = new List<string>();
            var users = new List<string>();
            var input = "";
            var version = false;
            var help = false;

            try
            {
                var p = new OptionSet {
                    { "l|list=", "List CALs or usernames to console {CAL|DMS}", v => list = v.ToLower() },
                    { "a|add=", "Add users or assign CALs from --input {CAL|DMS}", v => add = v.ToLower() },
                    { "r|remove=", "Remove users or inactive CALs {CAL|DMS}", v => remove = v.ToLower() },
                    { "d|document=", "Document to perform DMS actions on", v => documents.Add(v.ToLower()) },
                    { "i|input=", "Input file to read usernames from", v => input = v },
                    { "u|user=", "Username to be added to DMS", v => users.Add(v) },
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
                    Console.WriteLine("qv-user-manager 20110924\n");
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

            if (!String.IsNullOrEmpty(input))
            {
                var dmsUsers = new DmsUsers();
                users = dmsUsers.GetUsersFromFile(input);
            }

            switch (list)
            {
                case "dms":
                    ListDms(documents);
                    break;
                case "cal":
                    ListCals();
                    break;
            }

            switch (add)
            {
                case "dms":
                    AddDms(documents, users);
                    break;
                case "cal":
                    //AddCals(input);
                    Console.WriteLine("Adding of CAL's is not supported yet");
                    break;
            }

            switch (remove)
            {
                case "dms":
                    RemoveDms(documents, users);
                    break;
                case "cal":
                    RemoveCals();
                    break;
            }

        
        }
    }
}
