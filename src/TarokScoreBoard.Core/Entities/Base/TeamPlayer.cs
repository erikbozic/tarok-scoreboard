using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("team_player")]
    public partial class TeamPlayer
    {
        [Column("team_id")]
        public Guid TeamId { get; set; }
        [Column("player_id")]
        public Guid PlayerId { get; set; }
        [Required]
        [Column("name", TypeName = "character varying(100)")]
        public string Name { get; set; }
    }
}
