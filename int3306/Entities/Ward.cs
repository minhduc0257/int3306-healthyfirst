using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace int3306
{
    [Table("ward")]
    public class Ward
    {
        [Key]
        [Column("wardId", TypeName = "int(11)")]
        [JsonProperty("id")]
        public int WardId { get; set; }
        
        [Column("wardName", TypeName = "text")]
        [JsonProperty("name")]
        public string WardName { get; set; } = null!;
    }
}
