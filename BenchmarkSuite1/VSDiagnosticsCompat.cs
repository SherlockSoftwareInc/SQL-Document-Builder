using System;

namespace Microsoft.VSDiagnostics;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class CPUUsageDiagnoserAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class DotNetObjectAllocDiagnoserAttribute : Attribute
{
}
