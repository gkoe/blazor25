using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.ExtensionMethods
{
    public static class IncludeHelper
    {
        public static Expression<Func<TEntity, object>>[] Include<TEntity>(
            params Expression<Func<TEntity, object>>[] includes) => includes;
    }

}
