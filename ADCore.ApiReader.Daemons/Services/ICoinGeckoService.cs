using System.Threading.Tasks;

namespace ADCore.ApiReader.Daemons.Services
{
    public interface ICoinGeckoService
    {
        Task GetCoinList();
    }
}
