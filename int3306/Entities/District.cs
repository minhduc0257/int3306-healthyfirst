using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace int3306
{
    [Table("district")]
    public partial class District
    {
        [Key]
        [Column("districtId", TypeName = "int(11)")]
        public int DistrictId { get; set; }
        [Column("districtName", TypeName = "text")]
        public string DistrictName { get; set; } = null!;
    }
}
