using System.ComponentModel.DataAnnotations.Schema;

namespace int3306
{
    [Table("grants")]
    public class Grant
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("userId")]
        public int UserId { get; set; }
        
        [Column("districtId")]
        public int DistrictId { get; set; }
        
        [Column("wardId")]
        public int WardId { get; set; }
    }
}