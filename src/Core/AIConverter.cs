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

        public async Task<string> ConvertToBlazorAsync(List<ControlInfo> controls)
        {
            var prompt = $@"
            Convert the following WinForms controls into a single Blazor Razor component. 
            Use modern syntax and components. Do not include explanations, comments, or boilerplate code.
            Generate only the Razor component code.

            Controls to convert:
            {string.Join("\n", controls.Select(c => $"{c.Type} {c.Name} (Parent: {c.Parent})"))}

            Specific instructions:
            - Combine all controls into a single Blazor component.
            - Preserve the hierarchy of controls (e.g., buttons inside panels).
            - For ComboBox controls, use a Blazor Dropdown component with items and selected item binding.
            - For TreeView controls, use a Blazor TreeView component with nodes and selected node binding.
            - For ListView controls, use a Blazor Table or List component with items, columns, and selected item binding.
            - Do not include any explanations, comments, or boilerplate code in the output.
            - Generate only the Razor component code.
            - Don't take this as a tutorial. It is a real project.
        ";

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

                // Filter out unwanted messages
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