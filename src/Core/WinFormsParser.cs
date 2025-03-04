using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class WinFormsParser
    {
        public static List<ControlInfo> ParseControls(string code)
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

                // Check if the control is nested inside a container
                var parentContainer = FindParentContainer(assignment);

                // Handle DataGridView
                if (controlType == "DataGridView")
                {
                    var columns = FindDataGridViewColumns(assignment);
                    var dataSource = FindDataSource(assignment);

                    controls.Add(new ControlInfo
                    {
                        Name = controlName,
                        Type = controlType,
                        Parent = parentContainer,
                        Properties = new Dictionary<string, string>
                        {
                            { "Columns", string.Join(", ", columns) },
                            { "DataSource", dataSource }
                        }
                    });
                }

                // Handle ComboBox
                if (controlType == "ComboBox")
                {
                    var items = FindComboBoxItems(assignment);
                    var selectedItem = FindSelectedItem(assignment);

                    controls.Add(new ControlInfo
                    {
                        Name = controlName,
                        Type = controlType,
                        Parent = parentContainer,
                        Properties = new Dictionary<string, string>
                        {
                            { "Items", string.Join(", ", items) },
                            { "SelectedItem", selectedItem }
                        }
                    });
                }

                // Handle TreeView
                if (controlType == "TreeView")
                {
                    var nodes = FindTreeViewNodes(assignment);
                    var selectedNode = FindSelectedNode(assignment);

                    controls.Add(new ControlInfo
                    {
                        Name = controlName,
                        Type = controlType,
                        Parent = parentContainer,
                        Properties = new Dictionary<string, string>
                        {
                            { "Nodes", string.Join(", ", nodes) },
                            { "SelectedNode", selectedNode }
                        }
                    });
                }

                if (controlType == "ListView")
                {
                    var items = FindListViewItems(assignment);
                    var columns = FindListViewColumns(assignment);
                    var selectedItem = FindSelectedItem(assignment);

                    controls.Add(new ControlInfo
                    {
                        Name = controlName,
                        Type = controlType,
                        Parent = parentContainer,
                        Properties = new Dictionary<string, string>
                        {
                            { "Items", string.Join(", ", items) },
                            { "Columns", string.Join(", ", columns) },
                            { "SelectedItem", selectedItem }
                        }
                    });
                }
                else
                {
                    controls.Add(new ControlInfo
                    {
                        Name = controlName,
                        Type = controlType,
                        Parent = parentContainer
                    });
                }
            }

            return controls;
        }

        private static string? FindParentContainer(AssignmentExpressionSyntax assignment)
        {
            // Traverse the syntax tree to find the parent container
            var parent = assignment.Parent;

            while (parent != null)
            {
                if (parent is AssignmentExpressionSyntax parentAssignment &&
                    parentAssignment.Right is ObjectCreationExpressionSyntax creation &&
                    creation.Type.ToString().EndsWith("Panel")) // Check for Panel, GroupBox, etc.
                {
                    var left = parentAssignment.Left;
                    if (left is MemberAccessExpressionSyntax memberAccess)
                    {
                        return memberAccess.Name.Identifier.Text;
                    }
                    else if (left is IdentifierNameSyntax identifier)
                    {
                        return identifier.Identifier.Text;
                    }
                }

                parent = parent.Parent;
            }

            // No parent container found
            return null;
        }

        private static List<string> FindDataGridViewColumns(AssignmentExpressionSyntax assignment)
        {
            var columns = new List<string>();

            // Find column declarations (e.g., "dataGridView1.Columns.Add("Name", "Name");")
            var columnAdditions = assignment.Parent?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression.ToString().EndsWith(".Columns.Add"));

            foreach (var addition in columnAdditions ?? Enumerable.Empty<InvocationExpressionSyntax>())
            {
                var columnName = addition.ArgumentList.Arguments.FirstOrDefault()?.ToString().Trim('"');
                if (!string.IsNullOrEmpty(columnName))
                {
                    columns.Add(columnName);
                }
            }

            return columns;
        }

        private static List<string> FindComboBoxItems(AssignmentExpressionSyntax assignment)
        {
            var items = new List<string>();

            // Find ComboBox.Items.Add calls (e.g., "comboBox1.Items.Add("Item 1");")
            var itemAdditions = assignment.Parent?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression.ToString().EndsWith(".Items.Add"));

            foreach (var addition in itemAdditions ?? Enumerable.Empty<InvocationExpressionSyntax>())
            {
                var item = addition.ArgumentList.Arguments.FirstOrDefault()?.ToString().Trim('"');
                if (!string.IsNullOrEmpty(item))
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private static string FindSelectedItem(AssignmentExpressionSyntax assignment)
        {
            // Find SelectedItem assignment (e.g., "comboBox1.SelectedItem = "Item 1";")
            var selectedItemAssignment = assignment.Parent?
                .DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .FirstOrDefault(a => a.Left.ToString().EndsWith(".SelectedItem"));

            return selectedItemAssignment?.Right.ToString() ?? "UnknownSelectedItem";
        }

        private static string FindDataSource(AssignmentExpressionSyntax assignment)
        {
            // Find DataSource assignment (e.g., "dataGridView1.DataSource = GetCustomers();")
            var dataSourceAssignment = assignment.Parent?
                .DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .FirstOrDefault(a => a.Left.ToString().EndsWith(".DataSource"));

            return dataSourceAssignment?.Right.ToString() ?? "UnknownDataSource";
        }

        private static List<string> FindTreeViewNodes(AssignmentExpressionSyntax assignment)
        {
            var nodes = new List<string>();

            // Find TreeView.Nodes.Add calls (e.g., "treeView1.Nodes.Add("Node 1");")
            var nodeAdditions = assignment.Parent?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression.ToString().EndsWith(".Nodes.Add"));

            foreach (var addition in nodeAdditions ?? Enumerable.Empty<InvocationExpressionSyntax>())
            {
                var node = addition.ArgumentList.Arguments.FirstOrDefault()?.ToString().Trim('"');
                if (!string.IsNullOrEmpty(node))
                {
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        private static List<string> FindListViewItems(AssignmentExpressionSyntax assignment)
        {
            var items = new List<string>();

            // Find ListView.Items.Add calls (e.g., "listView1.Items.Add("Item 1");")
            var itemAdditions = assignment.Parent?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression.ToString().EndsWith(".Items.Add"));

            foreach (var addition in itemAdditions ?? Enumerable.Empty<InvocationExpressionSyntax>())
            {
                var item = addition.ArgumentList.Arguments.FirstOrDefault()?.ToString().Trim('"');
                if (!string.IsNullOrEmpty(item))
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private static List<string> FindListViewColumns(AssignmentExpressionSyntax assignment)
        {
            var columns = new List<string>();

            // Find ListView.Columns.Add calls (e.g., "listView1.Columns.Add("Column 1");")
            var columnAdditions = assignment.Parent?
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(i => i.Expression.ToString().EndsWith(".Columns.Add"));

            foreach (var addition in columnAdditions ?? Enumerable.Empty<InvocationExpressionSyntax>())
            {
                var column = addition.ArgumentList.Arguments.FirstOrDefault()?.ToString().Trim('"');
                if (!string.IsNullOrEmpty(column))
                {
                    columns.Add(column);
                }
            }

            return columns;
        }

        private static string FindSelectedNode(AssignmentExpressionSyntax assignment)
        {
            // Find SelectedNode assignment (e.g., "treeView1.SelectedNode = "Node 1";")
            var selectedNodeAssignment = assignment.Parent?
                .DescendantNodes()
                .OfType<AssignmentExpressionSyntax>()
                .FirstOrDefault(a => a.Left.ToString().EndsWith(".SelectedNode"));

            return selectedNodeAssignment?.Right.ToString() ?? "UnknownSelectedNode";
        }
    }

    public class ControlInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Parent { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new();
    }
}