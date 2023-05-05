using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using oTSPA.AppService.BotFunctions.Services;

namespace BotFunctions.Functions;

public static class CsvToJsonTest
{
    [Function("CsvToJsonTest")]
    public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        MemoryStream reqContent = new();
        await req.Body.CopyToAsync(reqContent);
        
        string jsonString = DataManipulationTool.DataTableToJson(DataManipulationTool.GetDataTableFromCSVFile(reqContent));

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonString ?? ""));

        return response;
        
    }
}