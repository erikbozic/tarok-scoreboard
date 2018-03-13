// #autogenerated
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("team")]
  public partial class Team
  { 
    [Column("passphrase")]
    public byte[] Passphrase { get; set; }

    [Column("salt")]
    public byte[] Salt { get; set; }

    [Key]
    [Column("team_id")]
    public Guid TeamId { get; set; }

    [Column("team_name")]
    public string TeamName { get; set; }

    [Column("team_user_id")]
    public string TeamUserId { get; set; }
  }
}