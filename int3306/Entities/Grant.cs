using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    [Table("grants")]
    public class Grant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [JsonProperty("id")]
        [SwaggerSchema(ReadOnly = true)]
        [ValidateNever]
        public int Id { get; set; }
        
        [Column("userId")]
        [JsonProperty("userId")]
        [Required]
        public int UserId { get; set; }
        
        [Column("districtId")]
        [JsonProperty("districtId")]
        public int? DistrictId { get; set; }
        
        
        [Column("wardId")]
        [JsonProperty("wardId")]
        public int? WardId { get; set; }
    }
}