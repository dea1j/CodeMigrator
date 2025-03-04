using Core;
using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace CodeMigrator.CLI
{
    [Command(Name = "migrate-tool", Description = "Migrate legacy code to modern .NET")]
    [Subcommand(typeof(ConvertCommand))]
    public class Program
    {
        public static async Task<int> Main(string[] args) => await CommandLineApplication.ExecuteAsync<Program>(args);

        // Add a dummy OnExecute to satisfy the library (optional)
        private Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }

    [Command("convert", Description = "Convert a legacy project")]
    public class ConvertCommand
    {
        [Required]
        [Option("-i|--input", Description = "Input directory or file")]
        public string InputPath { get; } = string.Empty;

        [Option("-o|--output", Description = "Output directory")]
        public string OutputPath { get; } = "./output";

        private readonly WinFormsParser _parser = new();
        private readonly AIConverter _converter = new();

        public async Task<int> OnExecute()
        {
            // Ensure the output directory exists
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }

            // Step 1: Parse WinForms code
            var code = File.ReadAllText(InputPath);
            var controls = WinFormsParser.ParseControls(code);

            // Step 2: Extract the input file name (without extension)
            var inputFileName = Path.GetFileNameWithoutExtension(InputPath);
            var outputFileName = $"{inputFileName}.razor";
            var outputFilePath = Path.Combine(OutputPath, outputFileName);

            // Step 3: Convert the entire form to Blazor
            var blazorCode = await _converter.ConvertToBlazorAsync(controls);

            // Step 4: Write the generated Blazor code to a file
            File.WriteAllText(outputFilePath, blazorCode);

            Console.WriteLine($"Generated: {outputFilePath}");
            return 0;
        }
    }
}