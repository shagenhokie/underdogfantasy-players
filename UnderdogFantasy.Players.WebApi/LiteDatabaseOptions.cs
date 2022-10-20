namespace UnderdogFantasy.Players.WebApi
{
    public class LiteDatabaseOptions
    {
        public const string LiteDatabaseSection = "LiteDatabase";

        public string FilePath { get; set; }

        public string AllPlayersCollection { get; set; }
    }
}
