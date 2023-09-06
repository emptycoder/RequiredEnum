using System.Threading.Tasks;
using RequiredEnum.SwitchHandlers;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        RequiredEnum.RequiredEnumAnalyzer>;

namespace RequiredEnum.Tests;

public class SwitchStatementAnalyzerTests
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
                          switch (variable)
                          {
                              case NumbersRequired.Zero:
                                  break;
                              case NumbersRequired.Three:
                              case NumbersRequired.One:
                                  break;
                              default:
                                  break;
                          }
                      }
                      
                      enum NumbersRequired
                      {
                          Zero,
                          One,
                          Two,
                          Three
                      }
                  }
              }
              """;

        var expected = Verifier
            .Diagnostic(SwitchStatements.DiagnosticId)
            .WithSpan(10, 13, 19, 14)
            .WithArguments("Two");
        
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}
