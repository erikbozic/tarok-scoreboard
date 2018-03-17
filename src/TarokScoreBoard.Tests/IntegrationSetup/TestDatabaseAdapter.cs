using Npgsql;
using System;
using System.IO;
using TarokScoreBoard.Core.Entities;
using Xunit.Abstractions;

namespace TarokScoreBoard.Tests.IntegrationSetup
{
  public class TestDatabaseAdapter : IDisposable
  {
    private readonly string connectionString;
    private readonly ITestOutputHelper output;
    private NpgsqlConnection tempConn;
    private const string maintenaceDbName = "postgres";

    public TestDatabaseAdapter(string connectionString, ITestOutputHelper output = null)
    {
      this.connectionString = connectionString;
      this.output = output;
    }    

    public void RecreateDatabase()
    {
      string sql = string.Empty;
      var assembly = typeof(DapperMapping).Assembly;
      var stream = assembly.GetManifestResourceStream("TarokScoreBoard.Infrastructure.Scripts.intitdb.sql");
      using (var sr = new StreamReader(stream))
        sql = sr.ReadToEnd();
      
      var realConn = new NpgsqlConnection(connectionString);

      tempConn = realConn.CloneWith($"User ID={realConn.UserName};Host={realConn.Host};Port={realConn.Port};Database={maintenaceDbName}");
      tempConn.Open();
      var testDatabaseInstanceName = realConn.Database;

      var dropCommand = tempConn.CreateCommand();
      dropCommand.CommandText = $"drop database if exists {testDatabaseInstanceName}";
      output?.WriteLine("dropping db");
      dropCommand.ExecuteNonQuery();

      var createDbCOmmand = tempConn.CreateCommand();
      createDbCOmmand.CommandText = $"create database {testDatabaseInstanceName}";
      output?.WriteLine("creating db");
      createDbCOmmand.ExecuteNonQuery();

      output?.WriteLine($"changing db to {testDatabaseInstanceName}");
      tempConn.ChangeDatabase(testDatabaseInstanceName);
      var initDbCommand = tempConn.CreateCommand();
      initDbCommand.CommandText = sql;
      output?.WriteLine($"executing init script");
      initDbCommand.ExecuteNonQuery();
    }

    public void SeedData()
    {
      var inserts = string.Empty;

      var assembly = typeof(DapperMapping).Assembly;
      var stream = assembly.GetManifestResourceStream("TarokScoreBoard.Infrastructure.Scripts.seed.sql");
      using (var sr = new StreamReader(stream))
        inserts = sr.ReadToEnd();

      var cmd = tempConn.CreateCommand();
      cmd.CommandText = inserts;
      cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
      tempConn.Close();
      tempConn.Dispose();
    }
  }
}
