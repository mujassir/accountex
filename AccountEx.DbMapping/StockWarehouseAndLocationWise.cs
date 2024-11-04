namespace AccountEx.DbMapping
{
    public class StockWarehouseAndLocationWise
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }

    }

}
