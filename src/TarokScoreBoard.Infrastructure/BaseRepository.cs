using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TarokScoreBoard.Infrastructure
{
  public abstract class BaseRepository<T>
  {
    protected abstract string BaseSelect { get; set; }

    protected readonly NpgsqlConnection conn;

    public BaseRepository(NpgsqlConnection conn)
    {
      this.conn = conn;
    }

    public IEnumerable<T> GetAll(Expression<Func<IQueryable<T>, IQueryable<T>>> expression, int limit = 0, int offset = 0, bool buffered = true)
    {
      var translator = new ExpressionToQueryTranslator();
      var expressionSql = translator.Translate(expression);

      var @params = new DynamicParameters();
      foreach (var p in expressionSql.@params)
        @params.Add($"p{p.Key}", p.Value);

      var whereClause = String.IsNullOrEmpty(expressionSql.predicate) ? string.Empty :
        $"WHERE {expressionSql.predicate}";

      var orderbyClause = String.IsNullOrEmpty(expressionSql.orderby) ? string.Empty :
        $"ORDER BY {expressionSql.orderby}";

      var paging = string.Empty;
      if (limit != 0)
        paging = $" LIMIT {limit} OFFSET {offset}";

      return conn.Query<T>($@"
      {BaseSelect}
      {whereClause}
      {orderbyClause}
      {paging}
      ",
      @params,
      buffered: buffered);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<IQueryable<T>, IQueryable<T>>> expression, int limit = 0, int offset = 0)
    {
      var translator = new ExpressionToQueryTranslator();
      var expressionSql = translator.Translate(expression);

      var @params = new DynamicParameters();
      foreach (var p in expressionSql.@params)
        @params.Add($"p{p.Key}", p.Value);

      var whereClause = String.IsNullOrEmpty(expressionSql.predicate) ? string.Empty :
        $"WHERE {expressionSql.predicate}";

      var orderbyClause = String.IsNullOrEmpty(expressionSql.orderby) ? string.Empty :
        $"ORDER BY {expressionSql.orderby}";

      var paging = string.Empty;
      if (limit != 0)
        paging = $" LIMIT {limit} OFFSET {offset}";

      return await conn.QueryAsync<T>($@"
      {BaseSelect}
      {whereClause}
      {orderbyClause}
      {paging}
      ",
      @params);
    }

    public IEnumerable<T> GetAll(int limit = 0, int offset = 0, bool buffered = true)
    {
      var paging = string.Empty;
      if (limit != 0)
        paging = $" LIMIT {limit} OFFSET {offset}";

      return conn.Query<T>($@"
      {BaseSelect} 
      {paging}
      ", buffered: buffered);
    }

    public async Task<IEnumerable<T>> GetAllAsync(int limit = 0, int offset = 0)
    {
      var paging = string.Empty;
      if (limit != 0)
        paging = $" LIMIT {limit} OFFSET {offset}";

      return await conn.QueryAsync<T>($@"
      {BaseSelect}
      {paging}
      ");
    }
  }
}
