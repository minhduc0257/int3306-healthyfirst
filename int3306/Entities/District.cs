using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    [Table("district")]
    public class District
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        [Column("districtId", TypeName = "int(11)")]
        [JsonProperty("id")]
        public int DistrictId { get; set; }
        
        [Required]
        [Column("districtName", TypeName = "text")]
        [JsonProperty("name")]
        public string DistrictName { get; set; } = null!;

        [SwaggerSchema(ReadOnly = true)]
        [JsonIgnore]
        [NotMapped]
        public List<Ward> Wards { get; set; } = null!;
    }
}
