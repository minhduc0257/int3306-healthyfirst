using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace int3306
{
    [Table("certificates")]
    public partial class Certificate
    {
        [Key]
        [Column("timestamp", TypeName = "datetime")]
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        
        [Key]
        [Column("transaction_type", TypeName = "int(11)")]
        [JsonProperty("type")]
        public int TransactionType { get; set; }
        
        [Key]
        [Column("shopId", TypeName = "int(11)")]
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }
    }
}
