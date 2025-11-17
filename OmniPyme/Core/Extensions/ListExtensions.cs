namespace OmniPyme.Web.Core.Extensions
{
    public static class ListExtensions
    {
        public static List<List<T>> GroupByItemsNumber<T>(this List<T> source, int groupSize)
        {
            return source.Select((item, index) => new { Index = index, Item = item })
                         .GroupBy(x => x.Index / groupSize)
                         .Select(g => g.Select(x => x.Item).ToList())
                         .ToList();
        }
    }
}
