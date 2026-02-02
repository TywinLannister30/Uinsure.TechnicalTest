using System.Diagnostics;

namespace Uinsure.TechnicalTest.API.Configuration;

public static class Tracing
{
    public const string SourceName = "Uinsure.TechnicalTest";
    public static readonly ActivitySource Source = new(SourceName);
}
