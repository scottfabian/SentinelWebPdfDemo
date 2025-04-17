using FabToolKit.Tkx.Sentinel.Rest;

namespace SentinelWebPdfDemo.Services;

public class SentinelApiFactory
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public SentinelApiFactory(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public SentinelApiClient GetClient(ActionType actionType)
    {
        string baseUrl;

        switch (actionType)
        {
            case ActionType.Print:
                baseUrl = _config["SentinelPrintURL"]!;
                break;
            case ActionType.Email:
                baseUrl = _config["SentinelEmailURL"]!;
                break;
            default:
                baseUrl = string.Empty;
                break;
        }

        return new SentinelApiClient(baseUrl, _httpClient);
    }
}
