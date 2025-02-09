namespace SuperFileSearcher
{
    public class File
    {
        private string _path;
        private string _name;

        public File(string path)
        {
            _path = path;
            _name = System.IO.Path.GetFileName(path);
        }

        public IEnumerable<Occurrence> FindOcurrence(IEnumerable<Filter> filters)
        { 
            int lineNumber = 1;
            foreach (string line in FileReader.StreamFile(_path, filters))
            { 
                foreach (var filter in filters)
                {
                    if (filter.DoesMatch(line))
                    {
                        yield return new Occurrence(lineNumber, line, _path, _name);
                    }
                }
                lineNumber++;
            }
        }

    }
}
