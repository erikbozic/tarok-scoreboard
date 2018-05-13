using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
    [Table("game")]
    public partial class Game
    {
        public Game()
        {
            GamePlayer = new HashSet<GamePlayer>();
            Round = new HashSet<Round>();
        }

        [Column("game_id")]
        public Guid GameId { get; set; }
        [Required]
        [Column("name", TypeName = "character varying(100)")]
        public string Name { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("team_id")]
        public Guid? TeamId { get; set; }

        [InverseProperty("Game")]
        public ICollection<GamePlayer> GamePlayer { get; set; }
        [InverseProperty("Game")]
        public ICollection<Round> Round { get; set; }
    }
}
