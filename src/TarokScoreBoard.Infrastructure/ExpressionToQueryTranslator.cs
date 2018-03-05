using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TarokScoreBoard.Infrastructure
{
  // from: https://stackoverflow.com/questions/7731905/how-to-convert-an-expression-tree-to-a-partial-sql-query
  public class ExpressionToQueryTranslator : ExpressionVisitor
  {
    private StringBuilder sb;
    private string orderBy = string.Empty;
    private string whereClause = string.Empty;



    private readonly IDictionary<int, object> queryParameters = new Dictionary<int, object>();
    private int paramCount = 0;

    public (string predicate, string orderby, IDictionary<int, object> @params) Translate(Expression expression)
    {
      this.sb = new StringBuilder();
      this.Visit(expression);
      this.whereClause = this.sb.ToString();
      return (this.whereClause, this.orderBy, this.queryParameters);
    }

    private static Expression StripQuotes(Expression e)
    {
      while (e.NodeType == ExpressionType.Quote)
      {
        e = ((UnaryExpression)e).Operand;
      }
      return e;
    }

    protected override Expression VisitMethodCall(MethodCallExpression m)
    {
      if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
      {
        this.Visit(m.Arguments[0]);
        LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
        this.Visit(lambda.Body);
        return m;
      }
      else if (m.Method.Name == "OrderBy")
      {
        if (this.ParseOrderByExpression(m, "ASC"))
        {
          Expression nextExpression = m.Arguments[0];
          return this.Visit(nextExpression);
        }
      }
      else if (m.Method.Name == "OrderByDescending")
      {
        if (this.ParseOrderByExpression(m, "DESC"))
        {
          Expression nextExpression = m.Arguments[0];
          return this.Visit(nextExpression);
        }
      }

      throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
    }

    protected override Expression VisitUnary(UnaryExpression u)
    {
      switch (u.NodeType)
      {
        case ExpressionType.Not:
          sb.Append(" NOT ");
          this.Visit(u.Operand);
          break;
        case ExpressionType.Convert:
          this.Visit(u.Operand);
          break;
        default:
          throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
      }
      return u;
    }

    protected override Expression VisitBinary(BinaryExpression b)
    {
      sb.Append("(");
      this.Visit(b.Left);

      switch (b.NodeType)
      {
        case ExpressionType.And:
          sb.Append(" AND ");
          break;

        case ExpressionType.AndAlso:
          sb.Append(" AND ");
          break;

        case ExpressionType.Or:
          sb.Append(" OR ");
          break;

        case ExpressionType.OrElse:
          sb.Append(" OR ");
          break;

        case ExpressionType.Equal:
          if (IsNullConstant(b.Right))
            sb.Append(" IS ");
          else
            sb.Append(" = ");
          break;

        case ExpressionType.NotEqual:
          if (IsNullConstant(b.Right))
            sb.Append(" IS NOT ");
          else
            sb.Append(" <> ");
          break;

        case ExpressionType.LessThan:
          sb.Append(" < ");
          break;

        case ExpressionType.LessThanOrEqual:
          sb.Append(" <= ");
          break;

        case ExpressionType.GreaterThan:
          sb.Append(" > ");
          break;

        case ExpressionType.GreaterThanOrEqual:
          sb.Append(" >= ");
          break;

        default:
          throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

      }

      this.Visit(b.Right);
      sb.Append(")");
      return b;
    }

    protected override Expression VisitConstant(ConstantExpression c)
    {
      IQueryable q = c.Value as IQueryable;

      if (q == null && c.Value == null)
      {
        sb.Append("NULL");
      }
      else if (q == null)
      {
        paramCount++;
        sb.Append($":p{paramCount}");
        queryParameters[paramCount] = c.Value;
      }

      return c;
    }

    protected override Expression VisitMember(MemberExpression m)
    {
      if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
      {
        sb.Append(m.Member.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().First().Name);
        return m;
      }
      if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
      {
        object container = ((ConstantExpression)m.Expression).Value;
        object value = ((FieldInfo)m.Member).GetValue(container);
        paramCount++;
        sb.Append($":p{paramCount}");
        queryParameters[paramCount] = value;
        return m;
      }

      throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
    }

    protected bool IsNullConstant(Expression exp)
    {
      return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
    }

    private bool ParseContainsExpression(MethodCallExpression expression)
    {
      return true;
    }

    private bool ParseOrderByExpression(MethodCallExpression expression, string order)
    {
      UnaryExpression unary = (UnaryExpression)expression.Arguments[1];
      LambdaExpression lambdaExpression = (LambdaExpression)unary.Operand;

      lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

      if (lambdaExpression.Body is MemberExpression body)
      {
        var columnName = body.Member.GetCustomAttributes(typeof(ColumnAttribute), false).Cast<ColumnAttribute>().First().Name;
        if (string.IsNullOrEmpty(orderBy))
        {
          orderBy = string.Format("{0} {1}", columnName, order);
        }
        else
        {
          orderBy = string.Format("{0}, {1} {2}", orderBy, columnName, order);
        }

        return true;
      }
      return false;
    }
  }
}
