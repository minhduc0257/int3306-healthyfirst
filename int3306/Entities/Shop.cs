using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace int3306
{
    [Table("shops")]
    public partial class Shop
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [Required]
        [Column("name", TypeName = "mediumtext")]
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [Column("address", TypeName = "mediumtext")]
        [JsonProperty("address")]
        public string? Address { get; set; }
        
        [Required]
        [Column("ward", TypeName = "int(11)")]
        [JsonProperty("ward")]
        public int Ward { get; set; }
        
        [Required]
        [Column("district", TypeName = "int(11)")]
        [JsonProperty("district")]
        public int District { get; set; }
        
        [Column("phone_number", TypeName = "mediumtext")]
        [JsonProperty("phone_number")]
        public string? PhoneNumber { get; set; }
        
        [Column("is_producer", TypeName = "tinyint(4)")]
        [JsonProperty("is_product")]
        public sbyte IsProducer { get; set; }
        
        [Column("is_seller", TypeName = "tinyint(4)")]
        [JsonProperty("is_seller")]
        public sbyte IsSeller { get; set; }
    }
}
