using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LiteDB;
using Microsoft.Extensions.Options;

namespace UnderdogFantasy.Players.WebApi
{
    public class PlayerStatisticsCalculator : ICalculatePlayerStatistics
    {
        private readonly LiteDatabaseOptions _options;
        private readonly ILiteDatabase _liteDb;
        private readonly Lazy<Dictionary<string, double>> _averageAgeByPosition;

        public PlayerStatisticsCalculator(IOptions<LiteDatabaseOptions> options, ILiteDatabase liteDb)
        {
            _options = options.Value;

            _liteDb = liteDb;

            _averageAgeByPosition = new Lazy<Dictionary<string, double>>(
                () => GetAverageAgeByPosition().ToDictionary(x => x.Position, x => x.AverageAge),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public double GetAverageAgeForPosition(string position) =>
            _averageAgeByPosition.Value[position];

        private IEnumerable<(string Position, double AverageAge)> GetAverageAgeByPosition()
        {
            using var reader = _liteDb.Execute($"SELECT @key, AVG(*.age) FROM {_options.AllPlayersCollection} where position != '' group by position");

            while (reader.Read())
            {
                var doc = reader.Current.AsDocument;

                yield return (doc.Values.ElementAt(0), doc.Values.ElementAt(1).AsDouble);
            }
        }
    }

    public interface ICalculatePlayerStatistics
    {
        public double GetAverageAgeForPosition(string position);
    }
}
