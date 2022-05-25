using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    public enum ActivityStep
    {
        Check = 1,
        Sampling = 2,
        Result = 3,
        Penalty = 4
    }
    
    [Table("activity")]
    public class Activity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [Column("planId")]
        public int PlanId { get; set; }
        
        [SwaggerSchema(ReadOnly = true)]
        [ForeignKey("PlanId")]
        [NotMapped]
        [ValidateNever]
        public Plan Plan { get; set; }
        
        [Required]
        [Column("shopId")]
        public int ShopId { get; set; }
        
        [Column("result")]
        public bool? Result { get; set; }
        
        [Required]
        [Column("startTime")]
        public DateTime StartTime { get; set; }
        
        [Required]
        [Column("endTime")]
        public DateTime EndTime { get; set; }
        
        [Required]
        [Column("currentStep")]
        public ActivityStep CurrentStep { get; set; }
    }
}