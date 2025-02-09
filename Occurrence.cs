namespace SuperFileSearcher
{
    public struct Occurrence
    {
        public readonly int LineNumber { get; }
        public readonly string Line { get; }
        public readonly string Path { get; }
        public readonly string Name { get; }

        public Occurrence(int lineNumber,string line, string path, string name) : this()
        {
            LineNumber = lineNumber;
            Line = line;
            Path= path;
            Name = name;
        }
         
    }
}