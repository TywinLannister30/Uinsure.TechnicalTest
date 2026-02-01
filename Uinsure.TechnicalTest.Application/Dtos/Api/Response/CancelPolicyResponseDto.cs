using System.Text.Json.Serialization;

namespace Uinsure.TechnicalTest.Application.Dtos.Api.Response;

public class CancelPolicyResponseDto
{
    public PolicyDto Policy { get; set; }
    public decimal RefundAmount { get; set; }
    
    [JsonIgnore]
    public bool AlreadyCancelled { get; set; }
    
    [JsonIgnore]
    public bool HasClaims { get; set; }
}
