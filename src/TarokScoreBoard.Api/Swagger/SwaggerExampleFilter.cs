using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TarokScoreBoard.Core.Entities;
using TarokScoreBoard.Shared;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Swagger
{
  public class SwaggerExampleFilter : ISchemaFilter
  {
    Guid gameId = Guid.Parse("20d0b6c4-91e7-4555-8b7b-06301bd0a43f");
    Guid roundId = Guid.Parse("c1aa894a-030d-42ce-9600-10b61606839e");
    Guid teamId = Guid.NewGuid();

    private IDictionary<Guid, GamePlayer> fourPlayers;
    private GamePlayer erik;
    private GamePlayer jan;
    private GamePlayer nejc;
    private GamePlayer luka;

    public SwaggerExampleFilter()
    {
      erik = new GamePlayer("Erik") { PlayerId = Guid.NewGuid() };
      jan = new GamePlayer("Jan")   { PlayerId = Guid.NewGuid() };
      nejc = new GamePlayer("Nejc") { PlayerId = Guid.NewGuid() };
      luka = new GamePlayer("Luka") { PlayerId = Guid.NewGuid() };

      fourPlayers = new Dictionary<Guid, GamePlayer>()
      {
        {erik.PlayerId, erik },
        {jan.PlayerId, jan },
        {luka.PlayerId,  luka },
        {nejc.PlayerId, nejc }
      };
    }

    public void Apply(Schema model, SchemaFilterContext context)
    {
      var type = context.SystemType;
      model.Example = GetExampleForType(type);
    }

    private object GetExampleForType(Type type)
    {
      switch (type.Name)
      {
        case nameof(CreateGameDTO):
          return new CreateGameDTO()
          {
             Name = "Game 10.03.2018",
             Players =  fourPlayers.Select(p => new PlayerDTO(p.Value.Name)).ToList()
          };
        case nameof(CreateRoundDTO):
          return new CreateRoundDTO()
          {
             GameId = gameId,
             ContraFactor = 1,
             GameType = 30,
             IsKlop = false,
             LeadPlayerId = jan.PlayerId,
             SupportingPlayerId = erik.PlayerId,        
             PagatFangPlayerId = luka.PlayerId,
             ScoreDifference = 20,
             Won = true,
             Modifiers = new List<ModifierDTO>()
             {
               new ModifierDTO()
               {
                 Announced = false,
                 ContraFactor = Shared.Enums.Contra.None,
                 ModifierType = ModifierTypeDbEnum.PAGAT_ULTIMO,
                 Team = Shared.Enums.TeamModifier.Playing
               },
               new ModifierDTO()
               {
                 Announced = true,
                 ContraFactor = Shared.Enums.Contra.Contra,
                 ModifierType = ModifierTypeDbEnum.TRULA,
                 Team = Shared.Enums.TeamModifier.Playing
               },
               new ModifierDTO()
               {
                 Announced = false,
                 ContraFactor = Shared.Enums.Contra.None,
                 ModifierType = ModifierTypeDbEnum.KRALJI,
                 Team = Shared.Enums.TeamModifier.NonPlaying
               },
             }
          };
        case nameof(GameDTO):
          return new GameDTO()
          {
            Date = new DateTime(2018, 03, 10),
            GameId = gameId,
            Name = "Tarok escapade of 2018.03.10",
            TeamId = teamId,
            Players = fourPlayers.Select(p => new GamePlayerDTO() { Name = p.Value.Name, PlayerId = p.Key, Position = p.Value.Position }).ToList()
          };
        case nameof(RoundDTO):
          return new RoundDTO()
          {
            ContraFactor = 1,
            GameId = gameId,
            Difference = 20,
            GameType = 30,
            IsKlop = false,
            LeadPlayerId = jan.PlayerId,
            RoundId = roundId,
            RoundNumber = 1,
            SupportingPlayerId = erik.PlayerId,
            PagatFangPlayerId = luka.PlayerId,
            Won = true,
            RoundResults = new List<RoundResultDTO>()
            {
              new RoundResultDTO()
              {
                PlayerId = jan.PlayerId,
                PlayerScore = 210,
                PlayerRadelcCount = 1,
                PlayerRadelcUsed = 1 ,
                RoundScoreChange = 0
              },
              new RoundResultDTO()
              {
                PlayerId = erik.PlayerId,
                PlayerScore = 210,
                PlayerRadelcCount = 1,
                PlayerRadelcUsed = 0,
                RoundScoreChange = 60
              },
              new RoundResultDTO()
              {
                PlayerId = luka.PlayerId,
                PlayerScore = 0,
                PlayerRadelcCount = 1,
                PlayerRadelcUsed = 0,
                RoundScoreChange = 60
              },
              new RoundResultDTO()
              {
                PlayerId = nejc.PlayerId,
                PlayerScore = 0,
                PlayerRadelcCount = 1,
                PlayerRadelcUsed = 0, 
                RoundScoreChange = 0
              }
            },
            Modifiers = new List<RoundModifierDTO>()
            {
              new RoundModifierDTO()
               {
                 Announced = false,
                 Contra = 1,
                 ModifierType = ModifierTypeDbEnum.PAGAT_ULTIMO,
                 Team = 1
               },
               new RoundModifierDTO()
               {
                 Announced = true,
                 Contra = 2,
                 ModifierType = ModifierTypeDbEnum.TRULA,
                 Team = 1
               },
               new RoundModifierDTO()
               {
                 Announced = false,
                 Contra = 1,
                 ModifierType = ModifierTypeDbEnum.KRALJI,
                 Team = -1
               },
            }
          };
        case nameof(CreateTeamDTO):
          return new CreateTeamDTO()
          {
            Passphrase = "Neko geslo ki ga poznajo vsi igralci",
            TeamId = "Nek unique id, ki si ga je lahko zapomnit", // TODO Change to TeamUsername
            Name = "Naša tarok skupina",
            Members = fourPlayers.Select(p => new TeamPlayerDTO(p.Value.Name) { PlayerId = null }).ToList()
          };
        case nameof(Team):
          return new Team()
          {
            TeamName = "Naša tarok skupina",
            Passphrase = Encoding.UTF8.GetBytes("to bom še odstranil v responsu"), // TODO
            TeamId = teamId,
            Members = new List<TeamPlayer>()
            {
              new TeamPlayer()
              {
                TeamId = teamId,
                Name = luka.Name,
                PlayerId = luka.PlayerId
              },new TeamPlayer()
              {
                TeamId = teamId,
                Name = nejc.Name,
                PlayerId = nejc.PlayerId
              },new TeamPlayer()
              {
                TeamId = teamId,
                Name = erik.Name,
                PlayerId = erik.PlayerId
              },new TeamPlayer()
              {
                TeamId = teamId,
                Name = jan.Name,
                PlayerId = jan.PlayerId
              },
            }
          };
        case nameof(LoginDTO):
          return new LoginDTO()
          {
            TeamId = "hribovci",
            Passphrase = "g00"      
          };
        case nameof(LoginResponseDTO):
          return new LoginResponseDTO(Guid.NewGuid(), new TeamDTO()
          {
            TeamId = teamId,
            TeamName = "Hribi team",
            TeamUserId = "hribovci",
            Members = fourPlayers.Select(p => new TeamPlayerDTO(p.Value.Name) { PlayerId = p.Value.PlayerId }).ToList()
          });
        case nameof(TeamDTO):
          return new TeamDTO()
          {
            TeamUserId = "hribovci",
            TeamId = teamId,
            TeamName = "Hribi team",
            Members = fourPlayers.Select(p => new TeamPlayerDTO(p.Value.Name) { PlayerId = p.Value.PlayerId }).ToList()
          };
        case nameof(TeamPlayer):
          return new TeamPlayer()
          {
            Name = "Gregor",
            PlayerId = Guid.NewGuid(),
            TeamId = teamId
          };

        default:
          return null;
      }
    }
  }
}
