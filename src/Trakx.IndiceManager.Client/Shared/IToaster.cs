using System.Threading.Tasks;

namespace Trakx.IndiceManager.Client.Shared
{
    public interface IToaster
    {
        Task ShowInfo(string content, string title = "Info");
        Task ShowSuccess(string content, string title = "Success");
        Task ShowWarning(string content, string title = "Warning");
        Task ShowError(string content, string title = "Error");
    }
}