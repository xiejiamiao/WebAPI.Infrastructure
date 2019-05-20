namespace WebAPI.Infrastructure.ModelDomain.QueryParameter
{
    public class OrderQueryParameter : BaseQueryParameter
    {
        public string OrderNo { get; set; }
        
        public string ReciverName { get; set; }
        
        public string ReciverMobile { get; set; }
        
        public string ReciverProvince { get; set; }
        
        public string ReciverCity { get; set; }
        
        public string ReciverDistrict { get; set; }
        
    }
}