using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using oTSPA.AppService.BotFunctions.Services;
using oTSPA.AppService.BotFunctions.Services.Interfaces;

namespace BotFunctions.Functions;

public class CsvToJsonTest
{
    private readonly IGoogleSheetsController _controller;

    public CsvToJsonTest(IGoogleSheetsController controller)
    {
        _controller = controller;
    }
    [Function("CsvToJsonTest")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        // MemoryStream reqContent = new();
        // await req.Body.CopyToAsync(reqContent);
        //
        // string jsonString = DataManipulationTool.DataTableToJson(DataManipulationTool.GetDataTableFromCSVFile(reqContent));

        string jsonString = _controller.GetSchedule();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonString ?? ""));

        return response;
        
    }
}