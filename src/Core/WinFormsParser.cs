using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class WinFormsParser
    {
        public List<ControlInfo> ParseControls(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();
            var controls = new List<ControlInfo>();

            // Find all control declarations (e.g., "this.button1 = new Button();")
            var assignments = root.DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .Where(a => a.Right is ObjectCreationExpressionSyntax);

            foreach (var assignment in assignments)
            {
                // Handle both "button1 = new Button()" and "this.button1 = new Button()"
                var left = assignment.Left;
                string controlName;

                if (left is MemberAccessExpressionSyntax memberAccess)
                {
                    // Handle "this.button1"
                    controlName = memberAccess.Name.Identifier.Text;
                }
                else if (left is IdentifierNameSyntax identifier)
                {
                    // Handle "button1"
                    controlName = identifier.Identifier.Text;
                }
                else
                {
                    // Skip unsupported syntax
                    continue;
                }

                var controlType = ((ObjectCreationExpressionSyntax)assignment.Right).Type.ToString();
                controls.Add(new ControlInfo { Name = controlName, Type = controlType });
            }

            return controls;
        }
    }

    public class ControlInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}