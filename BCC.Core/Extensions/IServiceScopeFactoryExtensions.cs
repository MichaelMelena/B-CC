using System;
using System.Collections.Generic;
using System.Text;
using  Microsoft.Extensions.DependencyInjection;
using  BCC.Core.Utils;
namespace BCC.Core.Extensions
{
    public static  class IServiceScopeFactoryExtensions
    {
        public static  DbScope GetDbScope(this IServiceScopeFactory iServiceScopeFactory )
        {
            return  new DbScope(iServiceScopeFactory);
        }
    }
}
