using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarokScoreBoard.Core.Entities
{
  [Table("team_access_token")]
    public partial class TeamAccessToken
    {
        [Column("team_id")]
        public Guid TeamId { get; set; }
        [Column("access_token")]
        public Guid AccessToken { get; set; }
        [Column("date_issued", TypeName = "date")]
        public DateTime DateIssued { get; set; }
    }
}
