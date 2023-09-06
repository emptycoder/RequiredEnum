using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using RequiredEnum.Utils;

namespace RequiredEnum.SwitchHandlers;

public static class SwitchStatements
{
    public const string DiagnosticId = "ER0002";
    public static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Not all enum members were used in switch cases",
        "'{0}' case wasn't handle",
        "Enum",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "");
    
    public static void AnalyzeSwitch(OperationAnalysisContext context)
    {
        if (context.Operation is not ISwitchOperation switchOperation ||
            context.Operation.Syntax is not SwitchStatementSyntax switchOperationSyntax)
            return;

        var typeSymbol = switchOperation.Value.Type;
        if (typeSymbol?.TypeKind is not TypeKind.Enum
            || !typeSymbol.Name.StartsWith("Required"))
            return;

        var enumCheckList = typeSymbol.GetEnumMemberNames();
        foreach (var caseOperation in switchOperation.Cases
                     .Select(@case => @case.Syntax)
                     .Where(syntax => syntax is SwitchSectionSyntax)
                     .Cast<SwitchSectionSyntax>()
                     .SelectMany(syntax => syntax.Labels)
                     .Where(label => label is CaseSwitchLabelSyntax)
                     .Cast<CaseSwitchLabelSyntax>()
                     .Select(label => label.Value))
            NameMarker(caseOperation);
        
        context.ReportIfNecessary(enumCheckList, Rule, switchOperationSyntax.GetLocation());
        return;

        void NameMarker(ExpressionSyntax syntax)
        {
            switch (syntax)
            {
                case MemberAccessExpressionSyntax { Name: IdentifierNameSyntax identifierSyntax }:
                    var identifier = identifierSyntax.Identifier.Text;
                    if (enumCheckList.ContainsKey(identifier))
                        enumCheckList[identifier] = true;
                    break;
            }
        }
    }    
}
