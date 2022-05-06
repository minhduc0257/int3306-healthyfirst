using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace int3306
{
    [Table("certificates")]
    public partial class Certificate
    {
        [Key]
        [Column("timestamp", TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Key]
        [Column("transaction_type", TypeName = "int(11)")]
        public int TransactionType { get; set; }
        [Key]
        [Column("shopId", TypeName = "int(11)")]
        public int ShopId { get; set; }
    }
}
