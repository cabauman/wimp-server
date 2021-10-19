using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WIMP_Server.DataServices.Http;
using WIMP_Server.Models;

namespace WIMP_Server.Data
{
    public static class PrepareDatabase
    {
        public static void Prepare(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var wimpDbContext = serviceScope.ServiceProvider.GetService<WimpDbContext>();
            Migrate(wimpDbContext);

            if (!wimpDbContext.Ships.Any())
            {
                PopulateShips(
                    serviceScope.ServiceProvider.GetService<IEsiDataClient>(),
                    wimpDbContext,
                    serviceScope.ServiceProvider.GetService<IMapper>());
            }

            if (!wimpDbContext.StarSystems.Any() ||
                !wimpDbContext.Stargates.Any())
            {
                PopulateSystemsAndStargates(
                    serviceScope.ServiceProvider.GetService<IEsiDataClient>(),
                    wimpDbContext,
                    serviceScope.ServiceProvider.GetService<IMapper>());
            }
        }

        private static void Migrate(WimpDbContext context)
        {
            context.Database.Migrate();
            context.SaveChanges();
        }

        private static void PopulateShips(IEsiDataClient esiDataClient, WimpDbContext context, IMapper mapper)
        {
            Task.WaitAll(esiDataClient.GetAllShips().ContinueWith(ships =>
            {
                context.Ships.AddRange(mapper.Map<IEnumerable<Ship>>(ships.Result));
                context.SaveChanges();
            }));
        }

        public static void PopulateSystemsAndStargates(IEsiDataClient esiDataClient, WimpDbContext context, IMapper mapper)
        {
            Task.WaitAll(esiDataClient.GetAllSystems().ContinueWith(systems =>
            {
                context.StarSystems.AddRange(mapper.Map<IEnumerable<StarSystem>>(systems.Result));
                context.SaveChanges();

                Task.WaitAll(esiDataClient.GetAllStargatesInSystems(systems.Result).ContinueWith(stargates =>
                {
                    context.Stargates.AddRange(mapper.Map<IEnumerable<Stargate>>(stargates.Result));
                    context.SaveChanges();
                }));
            }));
        }
    }
}