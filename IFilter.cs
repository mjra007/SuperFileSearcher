namespace SuperFileSearcher
{
    public interface IFilter
    {

        public Guid ID();
        bool DoesMatch(string line);
    }
}