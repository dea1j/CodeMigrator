using OllamaSharp;
using System.Threading.Tasks;

namespace CodeMigrator.Core
{
    public class AIConverter
    {
        private readonly OllamaApiClient _ollama;

        public AIConverter()
        {
            _ollama = new OllamaApiClient("http://localhost:11434");
            _ollama.SelectedModel = "codellama";
        }

        public async Task<string> ConvertToBlazorAsync(string winFormsCode)
        {
            var prompt = $@"
                Convert this WinForms code to a Blazor Razor component. 
                Use modern syntax and components.
                Input code:
                {winFormsCode}
            ";

            var response = await _ollama.Generate(prompt);
            return response.Response;
        }
    }
}