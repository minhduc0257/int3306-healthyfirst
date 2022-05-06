using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace int3306
{
    [Table("shops")]
    public partial class Shop
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [Column("name", TypeName = "mediumtext")]
        public string? Name { get; set; }
        
        [Column("address", TypeName = "mediumtext")]
        public string? Address { get; set; }
        
        [Required]
        [Column("ward", TypeName = "int(11)")]
        public int Ward { get; set; }
        
        [Required]
        [Column("district", TypeName = "int(11)")]
        public int District { get; set; }
        
        [Column("phone_number", TypeName = "mediumtext")]
        public string? PhoneNumber { get; set; }
        
        [Column("is_producer", TypeName = "tinyint(4)")]
        public sbyte IsProducer { get; set; }
        
        [Column("is_seller", TypeName = "tinyint(4)")]
        public sbyte IsSeller { get; set; }
    }
}
