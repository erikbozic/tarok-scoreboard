using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("team")]
    public partial class Team
    {
        [Column("team_id")]
        public Guid TeamId { get; set; }
        [Required]
        [Column("team_user_id", TypeName = "character varying(255)")]
        public string TeamUserId { get; set; }
        [Required]
        [Column("team_name", TypeName = "character varying(255)")]
        public string TeamName { get; set; }
        [Required]
        [Column("passphrase")]
        public byte[] Passphrase { get; set; }
        [Required]
        [Column("salt")]
        public byte[] Salt { get; set; }
    }
}
