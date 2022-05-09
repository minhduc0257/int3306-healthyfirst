using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace int3306
{
    [Table("district")]
    public partial class District
    {
        [Key]
        [Column("districtId", TypeName = "int(11)")]
        [JsonProperty("id")]
        public int DistrictId { get; set; }
        
        [Required]
        [Column("districtName", TypeName = "text")]
        [JsonProperty("name")]
        public string DistrictName { get; set; } = null!;

        [JsonIgnore]
        [NotMapped]
        public List<Ward> Wards { get; set; } = null!;
    }
}
