using System.Collections.Generic;
using System.IO;

namespace qv_user_manager
{
    class DmsUsers
    {
        public ICollection<string> GetUsersFromFile(string file)
        {
            var lines = new List<string>();

            using (var r = new StreamReader(file))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
