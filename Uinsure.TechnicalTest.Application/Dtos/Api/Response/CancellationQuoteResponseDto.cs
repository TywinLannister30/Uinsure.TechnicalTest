using System.Text.Json.Serialization;

namespace Uinsure.TechnicalTest.Application.Dtos.Api.Response;

public class CancellationQuoteResponseDto
{
    public decimal RefundAmount { get; set; }
    
    [JsonIgnore]
    public bool AlreadyCancelled { get; set; }
}
