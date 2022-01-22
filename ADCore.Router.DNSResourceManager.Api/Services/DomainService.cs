using ADCore.Router.DNSResourceManager.Api.Models;


namespace ADCore.Router.DNSResourceManager.Api.Services
{
    public interface IDomainService
    {
        Domain Add(Domain item);
        Domain Find(string domain);

    }
    public class DomainService : IDomainService
    {

        public Domain Add(Domain item)
        {
            MemoryContex.DB.Domains.Add(item);
            return item;
        }

        public Domain Find(string domain)
        {
            return MemoryContex.DB.Domains.Find(s => s.Name == domain);
        }
 
    }
}
