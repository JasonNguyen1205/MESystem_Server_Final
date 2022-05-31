using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MESystem.Data.TRACE
{
    public partial class TotalQuantitys : ComponentBase
    {
        public int TOTAL_QUANTITIY { get; set; }
    }
}