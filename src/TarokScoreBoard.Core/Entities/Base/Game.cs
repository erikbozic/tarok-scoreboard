// #autogenerated
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("game")]
  public partial class Game
  { 
    [Column("date")]
    public DateTime Date { get; set; }

    [Key]
    [Column("game_id")]
    public Guid GameId { get; set; }

    [Column("name")]
    public string Name { get; set; }
  }
}