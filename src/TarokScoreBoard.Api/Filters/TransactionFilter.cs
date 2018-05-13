using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TarokScoreBoard.Infrastructure;

namespace TarokScoreBoard.Api.Filters
{
  public class TransactionFilterAttribute : TypeFilterAttribute
  {
    public TransactionFilterAttribute() : base(typeof(TransactionFilter))
    {
    }

    private class TransactionFilter : IActionFilter
    {
      private readonly NpgsqlConnection connection;
      private NpgsqlTransaction transaction;

      public TransactionFilter(TarokDbContext dbContext)
      {
        this.connection = (NpgsqlConnection)dbContext.Database.GetDbConnection();
      }

      public void OnActionExecuting(ActionExecutingContext context)
      {
        if (connection.State != System.Data.ConnectionState.Open)
          connection.Open();

        transaction = this.connection.BeginTransaction();
      }

      public void OnActionExecuted(ActionExecutedContext context)
      {
        if(context.Exception == null)
          this.transaction.Commit();
      }
    }
  }
}
