using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TarokScoreBoard.Api;

namespace TarokScoreBoard.Tests.IntegrationSetup
{
  public class TestStartup : Startup
  {
    public TestStartup(IConfiguration configuration) : base(configuration)
    {
    }

  }
}
