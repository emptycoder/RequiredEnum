using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RequiredEnum.SwitchHandlers;

namespace RequiredEnum;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RequiredEnumAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(SwitchExpressions.Rule, SwitchStatements.Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(SwitchStatements.AnalyzeSwitch, OperationKind.Switch);
        context.RegisterOperationAction(SwitchExpressions.AnalyzeSwitchExpression, OperationKind.SwitchExpression);
    }
}
