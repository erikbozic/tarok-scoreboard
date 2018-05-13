using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("game_player")]
    public partial class GamePlayer
    {
        [Column("player_id")]
        public Guid PlayerId { get; set; }
        [Column("game_id")]
        public Guid GameId { get; set; }
        [Column("position")]
        public int? Position { get; set; }
        [Required]
        [Column("name", TypeName = "character varying(100)")]
        public string Name { get; set; }

        [ForeignKey("GameId")]
        [InverseProperty("GamePlayer")]
        public Game Game { get; set; }
    }
}
