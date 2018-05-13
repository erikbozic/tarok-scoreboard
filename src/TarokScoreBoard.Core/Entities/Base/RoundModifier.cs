using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("round_modifier")]
    public partial class RoundModifier
    {
        [Column("round_id")]
        public Guid RoundId { get; set; }
        [Column("team")]
        public int Team { get; set; }
        [Column("announced")]
        public bool Announced { get; set; }
        [Column("contra")]
        public int Contra { get; set; }
        [Column("modifier_type", TypeName = "character varying(64)")]
        public string ModifierType { get; set; }

        [ForeignKey("RoundId")]
        [InverseProperty("RoundModifier")]
        public Round Round { get; set; }
    }
}
