﻿using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MESystem.Data.TRACE
{
    [Table("V_SHOP_ORDER_STATUS", Schema = "TRACE")]

    public partial class vShopOrderStates : ComponentBase
    {
        [Column("DEPARTMENT")]
        public string Department { get; set; }
        [Column("ORDER_NO")]
        public string OrderNo { get; set; }
        [Column("PART_NO")]
        public string PartNo { get; set; }
        [Column("PART_DESCRIPTION")]
        public string PartDescription { get; set; }
        [Column("REVISED_QTY_DUE")]
        public int QtyPlanned { get; set; }
        [Column("QTY_FINISHED")]
        public int QtyFinished { get; set; }
        [Column("REMAINING_QTY")]
        public int QtyRemaining { get; set; }
        [Column("NEED_DATE")]
        public DateTime NeedDate { get; set; }
        [Column("START_DATE")]
        public DateTime? StartDate { get; set; } 
    }
}