namespace Uinsure.TechnicalTest.Application.Configuration;

public class OtlpSettings
{
    public string ServiceName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; } = 4317;
    public string Scheme { get; set; } = "http";
    public bool Enabled { get; set; } = true;
}
