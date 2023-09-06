using System.Threading.Tasks;
using RequiredEnum.SwitchHandlers;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        RequiredEnum.RequiredEnumAnalyzer>;

namespace RequiredEnum.Tests;

public class SwitchExpressionAnalyzerTests
{
    [Fact]
    public async Task RequiredEnumForSwitchExpression_AlertDiagnostic()
    {
        const string text = 
            """
            using System;

            namespace Test
            {
                public class Program
                {
                    public void Main()
                    {
                        NumbersRequired variable = NumbersRequired.One;
                        bool data = ((int) variable) == 1;
                        int number = variable switch
                        {
                            NumbersRequired.Zero when data is true => 0,
                            NumbersRequired.One => 1,
                            _ => throw new Exception()
                        };
                    }
                    
                    enum NumbersRequired
                    {
                        Zero,
                        One,
                        Two
                    }
                }
            }
            """;

        var expected = Verifier
            .Diagnostic(SwitchExpressions.DiagnosticId)
            .WithSpan(11, 26, 16, 14)
            .WithArguments("Two");
        
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}
