using OllamaSharp;
using OllamaSharp.Models;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class AIConverter
    {
        private readonly IOllamaApiClient _ollama;

        public AIConverter()
        {
            _ollama = new OllamaApiClient("http://localhost:11434");
        }

        public async Task<string> ConvertToBlazorAsync(string winFormsCode)
        {
            var prompt = $@"
        Convert this WinForms code to a Blazor Razor component. 
        Use modern syntax and components.
        Input code:
        {winFormsCode}";

          
            // Use GenerateAsync for code generation
            var request = new GenerateRequest
            {
                Prompt = prompt,
                Model = "codellama"
            };

            try
            {
                var responseStream = _ollama.GenerateAsync(request);
                var fullResponse = new StringBuilder();

                await foreach (var chunk in responseStream)
                {
                    if (chunk?.Response != null)
                    {
                        fullResponse.Append(chunk.Response);
                    }
                }

                // Filter out unwanted messages (e.g., "Converting to blazor...")
                var generatedCode = fullResponse.ToString();
                if (generatedCode.Contains("Note that in Blazor"))
                {
                    // Remove the extra notes from the response
                    generatedCode = generatedCode.Split("Note that in Blazor")[0].Trim();
                }

                return generatedCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating response:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}