using LiteDB;
using LiteDB.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UnderdogFantasy.Players.WebApi
{
    public static class LiteDatabaseServiceRegistrationExtensions
    {
        public static IServiceCollection AddLiteDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<LiteDatabaseOptions>(config.GetSection(LiteDatabaseOptions.LiteDatabaseSection));

            services.AddSingleton<ILiteDatabase, LiteDatabase>(serviceProvider =>
            {
                var mapper = BsonMapper.Global;

                mapper.Entity<Contracts.PlayerDocumentModel>()
                    .Id(x => x.DocumentId)
                    .Field(x => x.Id, "id")
                    .Field(x => x.EliasId, "elias_id")
                    .Field(x => x.FirstName, "firstname")
                    .Field(x => x.LastName, "lastname")
                    .Field(x => x.FullName, "fullname")
                    .Field(x => x.NameBrief, "name_brief")
                    .Field(x => x.Age, "age")
                    .Field(x => x.Sport, "sport")
                    .Field(x => x.Position, "position")
                    .Field(x => x.Throws, "throws")
                    .Field(x => x.Bats, "bats")
                    .Field(x => x.ByeWeek, "bye_week")
                    .Field(x => x.ProTeam, "pro_team")
                    .Field(x => x.Jersey, "jersey")
                    .Field(x => x.ProStatus, "pro_status")
                    .Field(x => x.IsEligibleForOffenseAndDefense, "eligible_for_offense_and_defense")
                    .Field(x => x.Photo, "photo")
                    .Field(x => x.Icons, "icons");

                var dbOptions = serviceProvider.GetRequiredService<IOptions<LiteDatabaseOptions>>().Value;

                var liteEngine = new LiteEngine(dbOptions.FilePath);

                // create indexes
                //
                // id (non-unique) NOTE: we'd prefer this to be unique, however there are dupes in the source data, e.g. where id = "1930"; not going to worry about this for now
                liteEngine.EnsureIndex(dbOptions.AllPlayersCollection, "id", "$.id", unique: false);
                //
                // sport (non-unique)
                liteEngine.EnsureIndex(dbOptions.AllPlayersCollection, "sport", "$.sport", unique: false);
                //
                // last_name_initial (non-unique)
                liteEngine.EnsureIndex(dbOptions.AllPlayersCollection, "last_name_initial", "SUBSTRING($.lastname, 0, 1)", unique: false);
                //
                // age (non-unique)
                liteEngine.EnsureIndex(dbOptions.AllPlayersCollection, "age", "$.age", unique: false);
                //
                // position (non-unique)
                liteEngine.EnsureIndex(dbOptions.AllPlayersCollection, "position", "$.position", unique: false);

                var liteDb = new LiteDatabase(liteEngine);

                return liteDb;
            });

            return services;
        }
    }
}
