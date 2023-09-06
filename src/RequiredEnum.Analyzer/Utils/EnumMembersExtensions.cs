using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RequiredEnum.Utils;

public static class EnumMembersExtensions
{
    public static Dictionary<string, bool> GetEnumMemberNames(this ITypeSymbol typeSymbol) =>
        typeSymbol
            .GetMembers()
            .Where(member => member.Kind is SymbolKind.Field)
            .Select(member => member.Name)
            .ToDictionary(name => name, _ => false);

    public static void ReportIfNecessary(
        this OperationAnalysisContext context,
        Dictionary<string, bool> enumCheckList,
        DiagnosticDescriptor descriptor,
        Location location)
    {
        foreach (var keyValuePair in enumCheckList)
        {
            if (keyValuePair.Value) continue;

            var diagnostic = Diagnostic.Create(
                descriptor,
                location,
                keyValuePair.Key);

            context.ReportDiagnostic(diagnostic);
        }
    }
}