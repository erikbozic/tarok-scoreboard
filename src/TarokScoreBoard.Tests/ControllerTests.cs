using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using TarokScoreBoard.Shared.DTO;
using TarokScoreBoard.Tests.IntegrationSetup;
using System.Text;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;
using TarokScoreBoard.Shared;
using FluentAssertions;
using FluentAssertions.Collections;

namespace TarokScoreBoard.Tests
{
  [TestCaseOrderer("TarokScoreBoard.Tests.PriorityOrderer", "TarokScoreBoard.Tests")]
  public class ControllerTests : IClassFixture<BackendTestFixture>
  {
    private readonly BackendTestFixture fixture;
    private readonly ITestOutputHelper output;

    public ControllerTests(BackendTestFixture fixture, ITestOutputHelper output)
    {
      this.fixture = fixture;
      this.output = output;
    }

    [Fact(DisplayName = "Unauthorized delete"), TestPriority(-100)]
    public async Task DeleteUnauthorized()
    {
      var response = await fixture.Client.DeleteAsync("/api/scoreboard/4ac472ed-5e78-4573-b24e-6506198f1f13");

      var unauthorized = response.StatusCode == System.Net.HttpStatusCode.Unauthorized;

      Assert.True(unauthorized);
    }

    [Fact(DisplayName = "get games anonymous"), TestPriority(-90)]
    public async Task GetGamesTest()
    {
      var response = await fixture.Client.GetAsync("/api/game");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<GameDTO[]>>(responseString);

      Assert.True(result.Data.Length == 1);
      Assert.True(!String.IsNullOrEmpty(responseString));
    }

    [Fact(DisplayName = "get scoreboard anonymous"), TestPriority(-90)]
    public async Task GetScoreboardTest()
    {
      var response = await fixture.Client.GetAsync("/api/scoreboard/4ac472ed-5e78-4573-b24e-6506198f1f13");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<RoundDTO[]>>(responseString);

      Assert.True(result.Data.Length == 62);
    }

    [Fact(DisplayName = "get teams anonymous"), TestPriority(-80)]
    public async Task GetTeamsTest()
    {
      var response = await fixture.Client.GetAsync("/api/team");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<TeamDTO[]>>(responseString);

      Assert.True(result.Data.Length == 1);
    }

    [Fact(DisplayName = "Create team and try login"), TestPriority(-70)]
    public async Task CreateTeamAndLogin()
    {
      var createTeamDto = new CreateTeamDTO()
      {
        Name = "test team",
        TeamId = "test1",
        Passphrase = "test1",
        Members = new List<TeamPlayerDTO>()
        {
          new TeamPlayerDTO("erik"),
          new TeamPlayerDTO("jan"),
          new TeamPlayerDTO("nejc"),
          new TeamPlayerDTO("luka")
        }
      };

      var content = RequestBodyFromObject(createTeamDto);
      var response = await fixture.Client.PostAsync("/api/team",content);
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<TeamDTO>>(responseString);

      var loginDto = new LoginDTO()
      {
        TeamId = "test1",
        Passphrase = "test1"
      };

      content = RequestBodyFromObject(loginDto);
      response = await fixture.Client.PostAsync("/api/team/login", content);
      response.EnsureSuccessStatusCode();

      responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
    }

    [Fact(DisplayName = "login and set token on client"), TestPriority(100)]
    public async Task LoginAndPersistToken()
    {
      var loginDto = new LoginDTO()
      {
        TeamId = "hribovci",
        Passphrase = "g00"
      };

      var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

      var response = await fixture.Client.PostAsync("/api/team/login", content);
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<LoginResponseDTO>>(responseString);
      this.fixture.accessToken = result.Data.AccessToken;
      this.fixture.team = result.Data.Team;
      fixture.Client.DefaultRequestHeaders.Add("access-token", this.fixture.accessToken.ToString());

      Assert.True(!String.IsNullOrEmpty(responseString));
    }

    [Fact(DisplayName = "delete round, authorized"), TestPriority(101)]
    public async Task DeleteAuthorized()
    {
      var response = await fixture.Client.DeleteAsync("/api/scoreboard/4ac472ed-5e78-4573-b24e-6506198f1f13");

      var deleted = response.StatusCode == System.Net.HttpStatusCode.NoContent;

      Assert.True(deleted);
    }

    [Fact(DisplayName = "Create a game"), TestPriority(102)]
    public async Task CreateGame()
    {
      var createGameDto = new CreateGameDTO()
      {
        Name = "new game",
        Players = this.fixture.team.Members.Select(m => new PlayerDTO(m.Name)
        {
          PlayerId = m.PlayerId
        }).ToList()
      };

      var content = RequestBodyFromObject(createGameDto);
      var response = await fixture.Client.PostAsync("/api/game", content);

      response.EnsureSuccessStatusCode();

      var result = await DeserializeResponse<GameDTO>(response);
      this.fixture.gameId = result.Data.GameId;

      result.Data.Name.Should().Be("new game");
      result.Data.Players.Should().HaveCount(4, "the created game has 4 players");
      
      result.Data.Players.Should().
      Contain(p => this.fixture.team.Members.Select(m => m.PlayerId).Contains(p.PlayerId),
       "All the players should have ids that belong to the team members");

      result.Data.Players.Where(p => p.IsMaestro == true).Should().HaveCount(0, "no player should be maestro because this is this tema's first game");
      result.Data.TeamId.Should().Be(fixture.team.TeamId);         
    }

    [Fact(DisplayName = "Post round"), TestPriority(103)]
    public async Task PostRound()
    {
      var p1 = this.fixture.team.Members[0];
      var p2 = this.fixture.team.Members[1];
      var p3 = this.fixture.team.Members[2];

      var createRoundDto = new CreateRoundDTO()
      {
        GameId = this.fixture.gameId,
        ContraFactor = 1,
        GameType = 30,
        IsKlop = false,
        LeadPlayerId = p1.PlayerId,
        SupportingPlayerId = p2.PlayerId,
        PagatFangPlayerId = p3.PlayerId,
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

      var content = RequestBodyFromObject(createRoundDto);
      var response = await fixture.Client.PostAsync($"/api/scoreboard", content);

      response.EnsureSuccessStatusCode();

      var result = await DeserializeResponse<RoundDTO>(response);

      result.Data.Modifiers.Should().HaveCount(3, "we posted a round with three modifiers");
      result.Data.RoundResults.Should().HaveCount(4," there's three four players in the game.");
      result.Data.RoundNumber.Should().Be(1, "it's the first round");
      result.Data.LeadPlayerId.Should().Be(p1.PlayerId, " player one was the leading player");
      result.Data.SupportingPlayerId.Should().Be(p2.PlayerId, "player two was the supporintg player");
      result.Data.PagatFangPlayerId.Should().Be(p3.PlayerId, "player three had his pagat stolen as the last card");
      result.Data.RoundResults.First(p => p.PlayerId == p1.PlayerId).PlayerScore.Should().Be(105, " i say so");
    }
    
    private async Task<ResponseDTO<T>> DeserializeResponse<T>(HttpResponseMessage response) where T : class
    {
      var responseString = await response.Content.ReadAsStringAsync();
      output.WriteLine(responseString);
      var result = JsonConvert.DeserializeObject<ResponseDTO<T>>(responseString);
      return result;
    }

    private StringContent RequestBodyFromObject(object obj)
    {
      var postBody = JsonConvert.SerializeObject(obj);
      return new StringContent(postBody, Encoding.UTF8, "application/json");
    }
  }
}
