using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace UnderdogFantasy.Players.WebApi.Controllers
{
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerDocumentStorage _playerDocumentStorage;
        private readonly ICalculatePlayerStatistics _stats;

        public PlayersController(IPlayerDocumentStorage playerDocumentStorage, ICalculatePlayerStatistics stats)
        {
            _playerDocumentStorage = playerDocumentStorage;
            _stats = stats;
        }

        [HttpGet("{id}")]
        public IActionResult GetPlayerById(string id)
        {
            var thePlayer = _playerDocumentStorage.GetPlayerById(id);

            return thePlayer != null
                ? Ok(ToResourceModel(thePlayer))
                : (IActionResult)NotFound();
        }

        //[HttpGet("{sport}/{id}")]
        //public IActionResult GetPlayerBySportAndId(string sport, string id)
        //{
        //    var thePlayer = _playerDocumentStorage.GetPlayerBySportAndId(sport, id);

        //    return thePlayer != null
        //        ? Ok(ToResourceModel(thePlayer))
        //        : (IActionResult)NotFound();
        //}

        [HttpGet]
        public IEnumerable<Contracts.PlayerResourceModel> SearchPlayers(
            [FromQuery] string sport,
            [FromQuery(Name = "last_name_initial")] char? lastNameInitial,
            [FromQuery] int? age,
            [FromQuery(Name = "min_age")] int? minAge,
            [FromQuery(Name = "max_age")] int? maxAge,
            [FromQuery] string position)
        {
            var thePlayerDocs = _playerDocumentStorage.SearchPlayers(sport, lastNameInitial, age, minAge, maxAge, position);

            var thePlayers = thePlayerDocs
                .Select(ToResourceModel)
                .ToList();

            return thePlayers;
        }

        private Contracts.PlayerResourceModel ToResourceModel(Contracts.PlayerDocumentModel p) =>
            new Contracts.PlayerResourceModel
            {
                Id = p.Id,
                NameBrief = p.NameBrief,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age.ToString(), // note: this is fine since Nullable<T> handles null values in ToString()
                Position = p.Position,
                AveragePositionAgeDiff = p.Age != null && !string.IsNullOrWhiteSpace(p.Position)
                    ? Math.Round(p.Age.Value - _stats.GetAverageAgeForPosition(p.Position), 1).ToString()
                    : string.Empty
            };
    }
}
