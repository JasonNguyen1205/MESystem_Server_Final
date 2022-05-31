using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MESystem.Data.TRACE
{
    [Table("SI_FAMILY", Schema = "TRACE")]

    public partial class SiFamily : ComponentBase
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("FAMILY")]
        public string? Family { get; set; }
    }
}