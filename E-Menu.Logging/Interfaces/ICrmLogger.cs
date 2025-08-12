using System;
using System.Threading.Tasks;

namespace E_Menu.Logging.Interfaces
{
    public interface ICrmLogger
    {
        Task LogInfoAsync(string message, object context = null);
        Task LogWarningAsync(string message, object context = null);
        Task LogErrorAsync(string message, Exception exception, object context = null);
    }
}
