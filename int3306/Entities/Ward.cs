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
        
        [Required]
        [Column("districtId", TypeName = "int(11)")]
        [JsonProperty("districtId")]
        public int? DistrictId { get; set; }

        [ForeignKey("DistrictId")]
        [NotMapped]
        public District District { get; set; } = null!;
        
        [Required]
        [Column("wardName", TypeName = "text")]
        [JsonProperty("name")]
        public string WardName { get; set; } = null!;
    }
}
