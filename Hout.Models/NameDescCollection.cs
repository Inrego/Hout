using System.Collections.Generic;

namespace Hout.Models
{
    public class NameDescCollection<TValue> : Dictionary<string, TValue> where TValue : INameDesc
    {
        public void Add(TValue item)
        {
            Add(item.Name, item);
        }
    }
}
