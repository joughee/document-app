using DemoApp.Infrastructure.Configuration;
using DemoApp.Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApp.Api.Extensions
{
    public static class ServiceCollectionDataAccessExtensions
    {
        public static IServiceCollection AddDb(this IServiceCollection services, DataAccessSettings daSettings)
        {
            IDataAccessManager dataAccess;
            switch (daSettings.DatabaseProvider)
            {
                case "Postgresql":
                    dataAccess = new PgDataAccess(daSettings);
                    break;
                default:
                    dataAccess = new PgDataAccess(daSettings);
                    break;
            }

            services.AddSingleton(dataAccess);

            return services;
        }
    }
}