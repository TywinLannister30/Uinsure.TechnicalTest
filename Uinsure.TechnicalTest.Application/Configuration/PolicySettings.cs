namespace Uinsure.TechnicalTest.Application.Configuration;

public class PolicySettings
{
    public int CoolingOffPeriodDays { get; set; }
    public int MaxStartDateAdvanceDays { get; set; }
    public int MaxPolicyholders { get; set; }
    public int MaxPostcodeLength { get; set; }
    public int RenewalWindowDays { get; set; }
}
