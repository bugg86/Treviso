using System.Collections;
using Google.Apis.Sheets.v4;
using oTSPA.AppService.BotFunctions.Services.Interfaces;

namespace oTSPA.AppService.BotFunctions.Services;

public class GoogleSheetsController : IGoogleSheetsController
{
    private const string SPREADSHEET_ID = "1LsdszKFMXI1K3tOS2BLvmUkzGJrvadiJSPn9eNZU4Nw";
    private const string SCHEDULE_SHEET = "Schedule";
    private readonly SpreadsheetsResource.ValuesResource _googleSheetValues;
    
    public GoogleSheetsController(GoogleSheetsService service)
    {
        _googleSheetValues = service.Service.Spreadsheets.Values;
    }

    public string GetSchedule()
    {
        var request = _googleSheetValues.Get(SPREADSHEET_ID, SCHEDULE_SHEET);

        var response = request.Execute();
        var values = response.Values;
        string returnString = string.Empty;

        foreach (IList row in response.Values)
        {
            foreach (object cell in row)
            {
                returnString += cell.ToString();
            }
        }
        return returnString;
    }
}