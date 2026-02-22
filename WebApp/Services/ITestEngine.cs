using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApp.ViewModels;

public interface ITestEngine
{
    Task<TaskResultViewModel> CheckAsync(int taskId, string participantId, IFormCollection form);
}
