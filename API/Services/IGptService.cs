namespace API.Services;

public interface IGptService
{
    Task<string> ClassifyPriorityAsync(string text);
    Task<string> CategorizeAsync(string text);
    Task<string> GetSolutionAsync(string text);
}