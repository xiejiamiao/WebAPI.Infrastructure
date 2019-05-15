using System;

namespace WebAPI.Infrastructure.DomainModel
{
    public class Order
    {
        public Guid Id { get; set; }
        
        public string OrderNo { get; set; }
        
        public string ReciverName { get; set; }
        
        public string ReciverMobile { get; set; }
        
        public string ReciverProvince { get; set; }
        
        public string ReciverCity { get; set; }
        
        public string ReciverDistrict { get; set; }
        
        public string ReciverDetailAddress { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public decimal ShippingAmount { get; set; }
    }
}