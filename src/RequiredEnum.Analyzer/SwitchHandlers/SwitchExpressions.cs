using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using RequiredEnum.Utils;

namespace RequiredEnum.SwitchHandlers;

public static class SwitchExpressions
{
    public const string DiagnosticId = "ER0001";
    public static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Not all enum members were used in switch cases",
        "'{0}' case wasn't handle",
        "Enum",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "");
    
    public static void AnalyzeSwitchExpression(OperationAnalysisContext context)
    {
        if (context.Operation is not ISwitchExpressionOperation switchOperation ||
            context.Operation.Syntax is not SwitchExpressionSyntax switchOperationSyntax)
            return;

        var typeSymbol = switchOperation.Value.Type;
        if (typeSymbol?.TypeKind is not TypeKind.Enum
            || !typeSymbol.Name.StartsWith("Required"))
            return;

        var enumCheckList = typeSymbol.GetEnumMemberNames();
        foreach (var switchArmPattern in switchOperation.Arms
                     .Select(arm => arm.Syntax)
                     .Where(syntax => syntax is SwitchExpressionArmSyntax)
                     .Cast<SwitchExpressionArmSyntax>()
                     .Select(armSyntax => armSyntax.Pattern))
            NameMarker(switchArmPattern);

        context.ReportIfNecessary(enumCheckList, Rule, switchOperationSyntax.GetLocation());
        return;

        void NameMarker(PatternSyntax patternSyntax)
        {
            switch (patternSyntax)
            {
                case ConstantPatternSyntax
                {
                    Expression: MemberAccessExpressionSyntax { Name: IdentifierNameSyntax identifierSyntax }
                }:
                    var identifier = identifierSyntax.Identifier.Text;
                    if (enumCheckList.ContainsKey(identifier))
                        enumCheckList[identifier] = true;
                    break;
                case BinaryPatternSyntax binaryPatternSyntax:
                    NameMarker(binaryPatternSyntax.Left);
                    NameMarker(binaryPatternSyntax.Right);
                    break;
            }
        }
    }
}
