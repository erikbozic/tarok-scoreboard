using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
    [Table("round")]
    public partial class Round
    {
        public Round()
        {
            RoundModifier = new HashSet<RoundModifier>();
            // RoundResult = new HashSet<RoundResult>();
        }

        [Column("round_id")]
        public Guid RoundId { get; set; }
        [Column("game_id")]
        public Guid GameId { get; set; }
        [Column("game_type")]
        public int? GameType { get; set; }
        [Column("difference")]
        public int? Difference { get; set; }
        [Column("won")]
        public bool Won { get; set; }
        [Column("lead_player_id")]
        public Guid? LeadPlayerId { get; set; }
        [Column("supporting_player_id")]
        public Guid? SupportingPlayerId { get; set; }
        [Column("is_klop")]
        public bool IsKlop { get; set; }
        [Column("contra_factor")]
        public int ContraFactor { get; set; }
        [Column("mond_fang_player_id")]
        public Guid? MondFangPlayerId { get; set; }
        [Column("pagat_fang_player_id")]
        public Guid? PagatFangPlayerId { get; set; }
        [Column("round_number")]
        public int RoundNumber { get; set; }

        [ForeignKey("GameId")]
        [InverseProperty("Round")]
        public Game Game { get; set; }
        [InverseProperty("Round")]
        public ICollection<RoundModifier> RoundModifier { get; set; }
        [InverseProperty("Round")]
        public ICollection<RoundResult> RoundResult { get; set; }
    }
}
