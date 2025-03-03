using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

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
                var controlName = ((IdentifierNameSyntax)assignment.Left).Identifier.Text;
                var controlType = ((ObjectCreationExpressionSyntax)assignment.Right).Type.ToString();
                controls.Add(new ControlInfo { Name = controlName, Type = controlType });
            }

            return controls;
        }
    }

    public class ControlInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}