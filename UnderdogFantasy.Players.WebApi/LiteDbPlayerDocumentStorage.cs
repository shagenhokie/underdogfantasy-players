using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Options;

namespace UnderdogFantasy.Players.WebApi
{
    public class LiteDbPlayerDocumentStorage : IPlayerDocumentStorage
    {
        private readonly LiteDatabaseOptions _options;
        private readonly ILiteDatabase _liteDb;

        public LiteDbPlayerDocumentStorage(IOptions<LiteDatabaseOptions> options, ILiteDatabase liteDb)
        {
            _options = options.Value;
            _liteDb = liteDb;
        }

        public Contracts.PlayerDocumentModel GetPlayerById(string id)
        {
            var allPlayersCollection = _liteDb.GetCollection<Contracts.PlayerDocumentModel>(_options.AllPlayersCollection);

            var thePlayer = allPlayersCollection
                .Find(Query.EQ("id", id))
                .FirstOrDefault();

            return thePlayer;
        }

        public Contracts.PlayerDocumentModel GetPlayerBySportAndId(string sport, string id)
        {
            var allPlayersCollection = _liteDb.GetCollection<Contracts.PlayerDocumentModel>(_options.AllPlayersCollection);

            // note: starting with "id" index since LiteDB is currently only able to use a single index per query; it chooses leftmost; and "id" index seek results in typically only a single match (although there are dupes across sports, e.g. 1930
            var query = Query.And(Query.EQ("id", id), Query.EQ("sport", sport));

            var thePlayer = allPlayersCollection
                .Find(query)
                .FirstOrDefault();

            return thePlayer;
        }

        public IEnumerable<Contracts.PlayerDocumentModel> SearchPlayers(string sport, char? lastNameInitial, int? age, int? minAge, int? maxAge, string position)
        {
            var queries = new[]
                {
                    age != null ? Query.EQ("age", age) : null,
                    minAge != null ? Query.GTE("age", minAge) : null,
                    maxAge != null ? Query.And(Query.LTE("age", maxAge), Query.Not("age", null)) : null,
                    !string.IsNullOrWhiteSpace(position) ? Query.EQ("position", position) : null,
                    lastNameInitial != null ? Query.EQ("SUBSTRING(lastname, 0, 1)", lastNameInitial.ToString()) : null,
                    !string.IsNullOrWhiteSpace(sport) ? Query.EQ("sport", sport) : null
                }
                .Where(q => q != null)
                .ToArray();

            var allPlayersCollection = _liteDb.GetCollection<Contracts.PlayerDocumentModel>(_options.AllPlayersCollection);

            var playersQuery = queries.Length == 0
                ? allPlayersCollection.Find(Query.All())
                : allPlayersCollection.Find(queries.Length == 1 ? queries[0] : Query.And(queries));

            var thePlayers = playersQuery
                .ToList();

            return thePlayers;
        }
    }

    public interface IPlayerDocumentStorage
    {
        Contracts.PlayerDocumentModel GetPlayerById(string id);

        Contracts.PlayerDocumentModel GetPlayerBySportAndId(string sport, string id);

        IEnumerable<Contracts.PlayerDocumentModel> SearchPlayers(string sport, char? lastNameInitial, int? age, int? minAge, int? maxAge, string position);
    }
}
