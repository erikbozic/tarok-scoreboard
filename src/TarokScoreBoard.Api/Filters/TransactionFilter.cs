using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Npgsql;

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

      public TransactionFilter(NpgsqlConnection connection)
      {
        this.connection = connection;
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
