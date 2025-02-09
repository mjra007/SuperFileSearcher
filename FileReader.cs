using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    public class FileReader
    {

        public static IEnumerable<string> StreamFile(string path, IEnumerable<Filter> filters)
        {
            using (StreamReader sr = System.IO.File.OpenText(path))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    yield return s;
                }
            }
        }

    }
}
