# WinForms to Blazor Converter CLI

![WinForms to Blazor](https://img.shields.io/badge/WinForms%20to%20Blazor-Converter-blueviolet)

A powerful command-line tool that automatically converts legacy **WinForms** applications into **Blazor** components using AI-powered code transformation.

## ✨ Features

- 🖥 **Convert Legacy WinForms Code** → Transforms Windows Forms UI controls into Blazor components.
- 🤖 **AI-Powered Conversion** → Uses LLM (CodeLlama) for modern .NET migration.
- 📁 **Batch Processing** → Convert entire projects or single files.
- ⚡ **Lightweight & Fast** → Built with `.NET 8`, ensuring high performance.
- 📂 **Custom Output Directory** → Save converted files to your desired location.

---

## 🚀 Getting Started

### 1️⃣ Prerequisites
- .NET SDK 8.0 or later
- **CodeLlama AI** installed locally via [Ollama](https://ollama.com)

### 2️⃣ Installation
Clone the repository:
```sh
git clone https://github.com/your-repo/winforms-to-blazor-cli.git
cd winforms-to-blazor-cli
```

Build the project:
```sh
dotnet build
```

### 3️⃣ Usage
Run the CLI with the **convert** command:
```sh
dotnet run -- convert -i path/to/Form1.cs -o output/
```
Example:
```sh
migrate-tool convert -i MyLegacyApp/Form1.cs -o ConvertedBlazor/
```

### 🔹 Example Input (WinForms Code)
```csharp
public class Form1 : Form
{
    private Button button1;
    public Form1()
    {
        this.button1 = new Button();
    }
}
```

### 🔹 Example Output (Blazor Component)
```razor
<Button @onclick="OnClick">Click me</Button>

@code {
    void OnClick()
    {
        Console.WriteLine("Button clicked");
    }
}
```

---

## ⚙️ CLI Commands & Options
| Command | Description |
|---------|-------------|
| `convert` | Convert a legacy WinForms file or project |
| `-i, --input` | **(Required)** Input file or directory |
| `-o, --output` | Output directory (default: `./output`) |

---

## 🛠 Tech Stack
- **C#** (.NET 8)
- **Blazor** (for modern UI conversion)
- **AI Model** (CodeLlama)
- **McMaster.Extensions.CommandLineUtils** (CLI support)

---

## 🤝 Contributing
Pull requests are welcome! If you find any issues or want to improve the conversion logic, feel free to contribute.

### Steps to Contribute:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature-new`)
3. Commit changes (`git commit -m 'Added new feature'`)
4. Push to the branch (`git push origin feature-new`)
5. Open a Pull Request

---

⭐ If you like this project, don't forget to **star** this repository!

