using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace ICTAce.FileHub.ApiTest;

public partial class Index : ModuleBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    private string _method = "GET";
    private int _moduleId;
    private int _id;
    private string _url = "api/MyModule";
    private string _body = string.Empty;

    private bool _loading;
    private string _error = string.Empty;
    private string _responseStatus = string.Empty;
    private string _responseBody = string.Empty;

    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    private int CurrentModuleId => _moduleId != 0 ? _moduleId : ModuleState.ModuleId;
    private int CurrentId => _id != 0 ? _id : 1;

    private async Task SendRequest()
    {
        _error = string.Empty;
        _responseStatus = string.Empty;
        _responseBody = string.Empty;

        try
        {
            _loading = true;

            using var request = new HttpRequestMessage(new HttpMethod(_method), _url);

            // set body if applicable; content type is always application/json
            if (_method != "GET" && _method != "DELETE")
            {
                if (!string.IsNullOrWhiteSpace(_body))
                {
                    // try to prettify JSON
                    try
                    {
                        using var doc = JsonDocument.Parse(_body);
                        var pretty = JsonSerializer.Serialize(doc.RootElement, _jsonOptions);
                        request.Content = new StringContent(pretty, Encoding.UTF8, "application/json");
                    }
                    catch (JsonException)
                    {
                        // not valid JSON, send raw
                        request.Content = new StringContent(_body, Encoding.UTF8, "application/json");
                    }
                }
                else
                {
                    request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                }
            }

            using var response = await Http.SendAsync(request);
            _responseStatus = $"{(int)response.StatusCode} {response.ReasonPhrase}";
            var respText = await response.Content.ReadAsStringAsync();

            // try parse JSON for pretty output
            if (!string.IsNullOrWhiteSpace(respText))
            {
                try
                {
                    using var doc = JsonDocument.Parse(respText);
                    _responseBody = JsonSerializer.Serialize(doc.RootElement, _jsonOptions);
                }
                catch (JsonException)
                {
                    _responseBody = respText;
                }
            }
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    // convenience prefill helpers for CRUD operations
    private void PrefillList()
    {
        _method = "GET";
        _url = $"api/MyModule?moduleId={CurrentModuleId}&pageNumber=1&pageSize=10";
        _body = string.Empty;
    }

    private void PrefillGet()
    {
        _method = "GET";
        _url = $"api/MyModule/{CurrentId}?moduleId={CurrentModuleId}";
        _body = string.Empty;
    }

    private void PrefillCreate()
    {
        _method = "POST";
        _url = $"api/MyModule?moduleId={CurrentModuleId}";
        var sample = new { Name = "ApiTest created " + DateTime.UtcNow.ToString("O") };
        _body = JsonSerializer.Serialize(sample, _jsonOptions);
    }

    private void PrefillUpdate()
    {
        _method = "PUT";
        _url = $"api/MyModule/{CurrentId}?moduleId={CurrentModuleId}";
        var sample = new { Name = "ApiTest updated " + DateTime.UtcNow.ToString("O") };
        _body = JsonSerializer.Serialize(sample, _jsonOptions);
    }

    private void PrefillDelete()
    {
        _method = "DELETE";
        _url = $"api/MyModule/{CurrentId}?moduleId={CurrentModuleId}";
        _body = string.Empty;
    }
}
