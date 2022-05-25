using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    [Table("plans")]
    public class Plan
    {
        [Key]
        [Column("planId")]
        public int PlanId { get; set; }
        
        [SwaggerSchema(ReadOnly = true)]
        [NotMapped]
        [JsonProperty("activities")]
        [ValidateNever]
        public List<Activity> Activities { get; set; }
    }
}