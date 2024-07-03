using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System.Security.Principal;

namespace Infrastructure.UnitTests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
internal sealed class SkipIfNotAdministratorAttribute : NUnitAttribute, IApplyToTest
{
    public void ApplyToTest(Test test)
    {
        if (IsAdministrator()) return;

        test.RunState = RunState.Ignored;
        test.Properties.Set(PropertyNames.SkipReason, "Access is denied, should run as administrator.");
    }

    private static bool IsAdministrator()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        using var identity = WindowsIdentity.GetCurrent();
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
#pragma warning restore CA1416 // Validate platform compatibility
    }
}