using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace ShipRate.Common.ExternalServices.Accouting
{
    public class NosaExternalAccountingClient : ExternalAccountingClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public NosaExternalAccountingClient(
            string endpoint,
            string? username = null,
            string? password = null,
            HttpMessageHandler? handler = null)
        {
            _endpoint = endpoint;

            _httpClient = handler == null
                ? new HttpClient()
                : new HttpClient(handler);

            if (!string.IsNullOrWhiteSpace(username))
            {
                var auth = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{username}:{password}")
                );

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", auth);
            }
        }

        public override void Login()
        {
            throw new NotImplementedException();
        }

        public override async Task<ExternalAccoutingClientResponse<string>> SendAccDocumentAsync(AccDocument accDocument)
        {
            string bodyXml = AccDocumentXmlBuilder.Build(accDocument);
            string soapAction = "";
            var envelope = BuildEnvelope(bodyXml);

            using var request =
                new HttpRequestMessage(HttpMethod.Post, _endpoint);

            request.Headers.Add("SOAPAction", soapAction);

            request.Content = new StringContent(
                envelope,
                Encoding.UTF8,
                "text/xml"
            );

            using var response = await _httpClient.SendAsync(request);

            var responseXml = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            string result = ExtractResult(responseXml);
            return new ExternalAccoutingClientResponse<string>((int)response.StatusCode, result, "0", result);
        }
        private static string ExtractResult(string soapResponse)
        {
            var doc = XDocument.Parse(soapResponse);

            var fault = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "Fault");

            if (fault != null)
                throw new InvalidOperationException($"SOAP Fault: {fault}");

            var body = doc.Descendants()
                .First(x => x.Name.LocalName == "Body");

            return body.Elements().FirstOrDefault()?.Value ?? string.Empty;
        }

        private static string BuildEnvelope(string bodyXml) =>
           $"""
        <?xml version="1.0" encoding="utf-8"?>
        <soap:Envelope
            xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
            <soap:Body>
                {bodyXml}
            </soap:Body>
        </soap:Envelope>
        """;
    }




}

