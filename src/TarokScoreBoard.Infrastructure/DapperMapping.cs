// #autogenerated
using Dapper;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  public static class DapperMapping
  {
    public static void ConfigureColumnMapping()
    {
      SqlMapper.SetTypeMap(
        typeof(Game),
        new CustomPropertyTypeMap(
          typeof(Game),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(Round),
        new CustomPropertyTypeMap(
          typeof(Round),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(RoundResult),
        new CustomPropertyTypeMap(
          typeof(RoundResult),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(RoundModifier),
        new CustomPropertyTypeMap(
          typeof(RoundModifier),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(GamePlayer),
        new CustomPropertyTypeMap(
          typeof(GamePlayer),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(TeamPlayer),
        new CustomPropertyTypeMap(
          typeof(TeamPlayer),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));

      SqlMapper.SetTypeMap(
        typeof(Team),
        new CustomPropertyTypeMap(
          typeof(Team),
          (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
              prop.GetCustomAttributes(false)
                .OfType<ColumnAttribute>()
                .Any(attr => attr.Name == columnName))));


    }
  }
}
