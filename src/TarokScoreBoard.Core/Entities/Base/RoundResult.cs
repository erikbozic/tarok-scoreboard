﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("round_result")]
    public partial class RoundResult
    {
        [Column("game_id")]
        public Guid GameId { get; set; }
        [Column("round_id")]
        public Guid RoundId { get; set; }
        [Column("round_score_change")]
        public int? RoundScoreChange { get; set; }
        [Column("player_id")]
        public Guid PlayerId { get; set; }
        [Column("player_score")]
        public int PlayerScore { get; set; }
        [Column("player_radelc_count")]
        public int PlayerRadelcCount { get; set; }
        [Column("player_radelc_used")]
        public int PlayerRadelcUsed { get; set; }

        [ForeignKey("RoundId")]
        [InverseProperty("RoundResult")]
        public Round Round { get; set; }
    }
}
