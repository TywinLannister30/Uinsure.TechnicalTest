using System.Text.Json.Serialization;

namespace Uinsure.TechnicalTest.Application.Dtos.Api.Response;

public class RenewPolicyResponseDto
{
    public PolicyDto Policy { get; set; }
    
    [JsonIgnore]
    public bool AlreadyCancelled { get; set; }

    [JsonIgnore]
    public bool IsInRenewalWindow { get; set; } = true;

    [JsonIgnore]
    public bool PolicyEnded { get; set; }
}
