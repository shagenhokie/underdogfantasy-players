using System;
using System.Collections.Generic;

namespace UnderdogFantasy.Players.Contracts
{
    public class PlayerDocumentModel
	{
		public Guid DocumentId { get; set; }
		public string Id { get; set; }
		public string EliasId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string NameBrief { get; set; }
		public int? Age { get; set; }
		public string Sport { get; set; }
		public string Position { get; set; }
		public string Throws { get; set; }
		public string Bats { get; set; }
		public string ByeWeek { get; set; }
		public string ProTeam { get; set; }
		public string Jersey { get; set; }
		public string ProStatus { get; set; }
		public int? IsEligibleForOffenseAndDefense { get; set; }
		public string Photo { get; set; }
		public Dictionary<string, object> Icons { get; set; }
	}
}
