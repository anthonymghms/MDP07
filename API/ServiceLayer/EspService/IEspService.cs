using System.Threading.Tasks;

namespace ServiceLayer.EspService
{
    public interface IEspService
    {
        Task Vibrate(string ipEspAddress);
        Task StopVibrate(string ipEspAddress);
    }
}
