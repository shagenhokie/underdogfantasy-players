using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using LiteDB.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UnderdogFantasy.Players.DataImportTool
{
    public static class Program
    {
        private const string SportFieldName = "sport";
        private const string NameBriefFieldName = "name_brief";
        //private const string LastNameInitialFieldName = "last_name_initial";
        private const string FirstNameFieldName = "firstname";
        private const string LastNameFieldName = "lastname";

        private const string SportValueBaseball = "baseball";
        private const string SportValueBasketball = "basketball";
        private const string SportValueFootball = "football";

        public static async Task Main(string[] args)
        {
            using var host = Host
                .CreateDefaultBuilder(args)
                .Build();

            var config = host.Services.GetRequiredService<IConfiguration>();

            var baseballPlayers = DeserializePlayersFromJsonFile(config.GetValue<string>("BaseballPlayersJsonFilePath"))
                .AddFields((SportFieldName, _ => SportValueBaseball));

            var basketballPlayers = DeserializePlayersFromJsonFile(config.GetValue<string>("BasketballPlayersJsonFilePath"))
                .AddFields((SportFieldName, _ => SportValueBasketball));

            var footballPlayers = DeserializePlayersFromJsonFile(config.GetValue<string>("FootballPlayersJsonFilePath"))
                .AddFields((SportFieldName, _ => SportValueFootball));

            var allPlayers =
                baseballPlayers
                    .Concat(basketballPlayers)
                    .Concat(footballPlayers)
                    .AddFields
                    (
                        (NameBriefFieldName, ResolveNameBrief)
                        //(LastNameInitialFieldName, x => ((string)x[LastNameFieldName])[0].ToString())
                    )
                    .ToList();

            using var liteEngine = new LiteEngine(config.GetValue<string>("LiteDatabaseFilePath"));

            var allPlayersCollection = config.GetValue<string>("LiteDatabaseAllPlayersCollection");

            liteEngine.DropCollection(allPlayersCollection);

            liteEngine.Insert(allPlayersCollection, allPlayers, BsonAutoId.Guid);

            // create indexes
            //
            // id (non-unique) NOTE: we'd prefer this to be unique, however there are dupes in the source data, e.g. where id = "1930"; not going to worry about this for now
            liteEngine.EnsureIndex(allPlayersCollection, "id", "$.id", unique: false);
            //
            // sport (non-unique)
            liteEngine.EnsureIndex(allPlayersCollection, "sport", "$.sport", unique: false);
            //
            // last_name_initial (non-unique)
            liteEngine.EnsureIndex(allPlayersCollection, "last_name_initial", "SUBSTRING($.lastname, 0, 1)", unique: false);
            //
            // age (non-unique)
            liteEngine.EnsureIndex(allPlayersCollection, "age", "$.age", unique: false);
            //
            // position (non-unique)
            liteEngine.EnsureIndex(allPlayersCollection, "position", "$.position", unique: false);

            await host.RunAsync();
        }

        private static IEnumerable<BsonDocument> DeserializePlayersFromJsonFile(string pathToJsonFile) =>
            JsonSerializer
                .DeserializeArray(File.ReadAllText(pathToJsonFile))
                .Select(x => x.AsDocument);

        private static BsonValue ResolveNameBrief(BsonDocument x)
        {
            var sport = (string)x[SportFieldName];
            var firstName = (string)x[FirstNameFieldName];
            var lastName = (string)x[LastNameFieldName];

            return sport switch
            {
                SportValueBaseball => $"{(!string.IsNullOrWhiteSpace(firstName) ? firstName[0].ToString() : "")}. {(!string.IsNullOrWhiteSpace(lastName) ? lastName[0].ToString() : "")}.",
                SportValueBasketball => $"{firstName} {(!string.IsNullOrWhiteSpace(lastName) ? lastName[0].ToString() : "")}.",
                SportValueFootball => $"{(!string.IsNullOrWhiteSpace(firstName) ? firstName[0].ToString() : "")}. {lastName}",
                _ => string.Empty,
            };
        }
    }

    internal static class Extensions
    {
        public static IEnumerable<BsonDocument> AddFields(this IEnumerable<BsonDocument> docs, params (string Key, Func<BsonDocument, BsonValue> ValueFunc)[] fields)
        {
            foreach (var x in docs)
            {
                foreach (var (Key, ValueFunc) in fields)
                {
                    x.Add(Key, ValueFunc(x));
                }

                yield return x;
            }
        }
    }
}
