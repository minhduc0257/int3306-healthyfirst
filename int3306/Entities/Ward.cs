using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    [Table("ward")]
    public class Ward
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        [Column("wardId", TypeName = "int(11)")]
        [JsonProperty("id")]
        public int WardId { get; set; }
        
        [Required]
        [Column("districtId", TypeName = "int(11)")]
        [JsonProperty("districtId")]
        public int? DistrictId { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        [ForeignKey("DistrictId")]
        [NotMapped]
        [ValidateNever]
        public District District { get; set; } = null!;
        
        [Required]
        [Column("wardName", TypeName = "text")]
        [JsonProperty("name")]
        public string WardName { get; set; } = null!;
    }
}
