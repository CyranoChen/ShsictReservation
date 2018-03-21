﻿using System.Collections.Generic;
using System.Linq;
using Shsict.Core.Extension;

namespace Shsict.Core
{
    public abstract class Searchable<T> where T : class, new()
    {
        public Criteria Criteria { get; set; }

        public IEnumerable<T> Data { get; set; }

        public virtual void Search(IEnumerable<T> data)
        {
            Criteria.GetPageSize();

            var list = data as IList<T> ?? data.ToList();

            Criteria.SetTotalCount(list.Count);

            if (Criteria.TotalCount > Criteria.PagingSize && Criteria.MaxPage >= 0)
            {
                Data = list.Page(Criteria.CurrentPage, Criteria.PagingSize);
            }
            else
            {
                Data = list;
            }
        }
    }
}