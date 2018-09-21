using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Dynamic;
using System.Linq;

namespace Roro.Workflow
{
    public static class Expression
    {
        public sealed class VariableListProvider
        {
            public dynamic v { get; }

            public VariableListProvider(IEditablePage page)
            {
                this.v = new VariableList(page);
            }
        }

        public sealed class VariableList : DynamicObject
        {
            private readonly IEditablePage _page;

            public VariableList(IEditablePage page)
            {
                this._page = page;   
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (this._page.Nodes.Where(x => x is VariableNode).Cast<VariableNode>()
                                    .FirstOrDefault(x => x.Name == binder.Name) is VariableNode variableNode)
                {
                    result = variableNode.RuntimeValue;
                    return true;
                }
                return base.TryGetMember(binder, out result);
            }
        }

        public static object Evaluate(string code, IEditablePage page) => Evaluate<object>(code, page);

        public static T Evaluate<T>(string code, IEditablePage page)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return default(T);
            }

            try
            {
                var result = CSharpScript.EvaluateAsync<T>(
                    code,
                    ScriptOptions.Default
                        .WithImports("System")
                        .WithReferences("Microsoft.CSharp"),
                    new VariableListProvider(page)).Result;
                return result;
            }
            catch (CompilationErrorException)
            {
                throw; // string.Join(Environment.NewLine, e.Diagnostics)
            }
        }
    }
}