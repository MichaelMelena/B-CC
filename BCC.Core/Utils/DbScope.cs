using System;
using BCC.Model.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BCC.Core.Utils
{
    public class DbScope : IDisposable
    {
        public BCCContext Context { get; }
        private readonly IServiceScope _serviceScope;

        public DbScope(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScope = serviceScopeFactory.CreateScope();
            Context = _serviceScope.ServiceProvider.GetService<BCCContext>();
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }
    }
}
