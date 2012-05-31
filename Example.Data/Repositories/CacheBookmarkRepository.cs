using System;
using System.Collections.Generic;
using System.Web;
using Utilities.Extensions;

namespace Example.Data.Repositories
{
    public class CacheBookmarkRepository : BookmarkRepository
    {
        private static readonly object _cachLockObject;

        public override IEnumerable<string> GetTags(string tagName)
        {
            var cachekey = UrnId.Create("GetTags",tagName);

            var result = HttpRuntime.Cache[cachekey].As<IEnumerable<string>>();

            if(result.IsNull())
            {
                lock(_cachLockObject)
                {
                    result = HttpRuntime.Cache[cachekey].As<IEnumerable<string>>();

                    if(result.IsNull())
                    {
                        result = base.GetTags(tagName);
                        HttpRuntime.Cache.Insert(cachekey,result,null,DateTime.Now.AddSeconds(60),TimeSpan.Zero);//Expiration should be moved to configuration
                    }
                }
            }

            return result;
        }
    }
}