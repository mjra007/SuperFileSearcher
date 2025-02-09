using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFileSearcher
{
    public class Repo
    {

        public string RepoPath { get; private set; }

        public Repo(string path)
        {
            RepoPath = path;
        }

        public IEnumerable<File> GetFiles(string searchPattern)
        {
            return System.IO.Directory.EnumerateFiles(RepoPath, searchPattern).Select(x=>new File(x));
        }

    }
}
