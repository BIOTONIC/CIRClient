using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CIRClient
{
    public static class Extension
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            items.ToList().ForEach(collection.Add);
        }
    }
}
