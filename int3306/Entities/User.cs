using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace int3306
{
    [Table("users")]
    public class User
    {
        /// <summary>
        /// User ID
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        [Key]
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ValidateNever]
        public int Id { get; set; }

        /// <summary>
        /// Username (duh)
        /// </summary>
        [Column("username")]
        [JsonProperty("username")]
        [Required]
        public string Username { get; set; } = null!;

        /// <summary>
        /// Password in plain text.
        /// </summary>
        [SwaggerSchema(WriteOnly = true)]
        [Column("password")]
        [JsonProperty("password")]
        [Required]
        public string Password { get; set; } = null!;
        
        /// <summary>
        /// User privilege.
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        [Column("type")]
        [JsonProperty("type")]
        [ValidateNever]
        public UserType Type { get; set; }
    }
}