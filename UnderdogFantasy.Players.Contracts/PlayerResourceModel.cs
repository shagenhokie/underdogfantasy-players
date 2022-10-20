using System.Text.Json.Serialization;

namespace UnderdogFantasy.Players.Contracts
{
    public class PlayerResourceModel
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("name_brief")]
		public string NameBrief { get; set; }

		[JsonPropertyName("first_name")]
		public string FirstName { get; set; }

		[JsonPropertyName("last_name")]
		public string LastName { get; set; }

		[JsonPropertyName("age")]
		public string Age { get; set; }

		[JsonPropertyName("position")]
		public string Position { get; set; }

		[JsonPropertyName("average_position_age_diff")]
		public string AveragePositionAgeDiff { get; set; }
	}
}
