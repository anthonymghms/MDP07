using System.Threading.Tasks;

namespace ServiceLayer.EspService
{
    public interface IEspService
    {
        Task Vibrate(bool start);
    }
}
