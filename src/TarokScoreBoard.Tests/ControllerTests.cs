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
      Assert.True(result.Data.Name == "new game");
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

      Assert.True(result.Data.RoundNumber == 1);
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
