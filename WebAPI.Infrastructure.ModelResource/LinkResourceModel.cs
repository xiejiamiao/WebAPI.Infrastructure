namespace WebAPI.Infrastructure.ResourceModel
{
    public class LinkResourceModel
    {
        public LinkResourceModel(string href,string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
        
        public string Href { get; }
        public string Rel { get; }
        public string Method { get; }
    }
}