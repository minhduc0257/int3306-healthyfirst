using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("id")]
        public int DistrictId { get; set; }
        
        [Required]
        [Column("districtName", TypeName = "text")]
        [JsonProperty("name")]
        public string DistrictName { get; set; } = null!;

        [SwaggerSchema(ReadOnly = true)]
        [JsonProperty("wards")]
        [NotMapped]
        [ValidateNever]
        public List<Ward> Wards { get; set; } = null!;
    }
}
