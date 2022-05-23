using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    public enum CertificateType
    {
        Grant = 1,
        Revoke = 2
    }

    [Table("certificates")]
    public partial class Certificate
    {
        [Key]
        [SwaggerSchema(ReadOnly = true)]
        [Column("id")]
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [SwaggerSchema(ReadOnly = true)]
        [Column("timestamp", TypeName = "datetime")]
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        
        [Required]
        [Column("transaction_type", TypeName = "int(11)")]
        [JsonProperty("type")]
        public CertificateType TransactionType { get; set; }
        
        [Required]
        [Column("shopId", TypeName = "int(11)")]
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }
        
        [Column("validity")]
        [JsonProperty("valid_until")]
        public DateTime? Validity { get; set; }
        
        [SwaggerSchema(ReadOnly = true)]
        [ForeignKey("ShopId")]
        [JsonProperty("shop")]
        [ValidateNever]
        public Shop? Shop { get; set; }
    }
}
