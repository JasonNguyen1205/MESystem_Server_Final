using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MESystem.Data.TRACE
{
    [Table("LINE", Schema = "TRACE")]
    public partial class ProductionLine : ComponentBase
    {
        [Column("LINE_ID")]
        public int LineId { get; set; }
        [Column("LINE_DESCRIPTION")]
        public string? LineDescription { get; set; }
        [Column("DEPARTMENT_NO")]
        public string? DepartmentNo { get; set; }
        [Column("AREA")]
        public string? Area { get; set; }
    }
}