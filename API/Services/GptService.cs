using API.Models.DTO;

namespace API.Services;

public class GptService : IGptService
{
    private readonly IHttpClientFactory _factory;
    private readonly string _model;

    public GptService(IHttpClientFactory factory, IConfiguration config)
    {
        _factory = factory;
        _model = config["Gpt:Model"] ?? "gpt-4o-mini";
    }

    private async Task<string> PostPromptAsync(string systemPrompt, string userText)
    {
        var http = _factory.CreateClient("gpt");
        var payload = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userText }
            }
        };

        var resp = await http.PostAsJsonAsync("", payload);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadFromJsonAsync<RespostaGPT>();
        return json?.choices?.FirstOrDefault()?.message?.content?.Trim() ?? string.Empty;
    }

    public Task<string> ClassifyPriorityAsync(string text)
    {
        var prompt = "Você é um sistema de triagem de chamados internos. Classifique o chamado exclusivamente em baixa, media ou alta prioridade, considerando impacto, urgência e interrupção de atividades. Responda apenas com uma única palavra.";
        return PostPromptAsync(prompt, text).ContinueWith(t => t.Result.ToLowerInvariant());
    }

    public Task<string> CategorizeAsync(string text)
    {
        var prompt = "Você é um sistema de triagem de chamados internos de TI. Classifique o chamado exclusivamente em uma das seguintes categorias: hardware, software, rede, acesso, impressora, e-mail, sistema corporativo, desempenho, segurança, outro. Responda somente com a categoria.";
        return PostPromptAsync(prompt, text).ContinueWith(t => t.Result.ToLowerInvariant());
    }

    public Task<string> GetSolutionAsync(string text)
    {
        var prompt = "Você é um analista sênior de TI orientando outro técnico no atendimento de um chamado interno. Analise o problema de forma objetiva e focada em continuidade operacional. Forneça até 5 ações técnicas diretas. Evite linguagem voltada ao usuário final.";
        return PostPromptAsync(prompt, text);
    }
}
