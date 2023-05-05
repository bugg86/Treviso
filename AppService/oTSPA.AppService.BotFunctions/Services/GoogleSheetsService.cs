using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace oTSPA.AppService.BotFunctions.Services;

public class GoogleSheetsService
{
    public SheetsService Service { get; set; }
    const string APPLICATION_NAME = "oTSPA";
    static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    
    public GoogleSheetsService()
    {
        InitializeService();
    }
    
    private void InitializeService()
    {
        var credential = GetCredentialsFromFile();
        Service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = APPLICATION_NAME
        });
    }
    private GoogleCredential GetCredentialsFromFile()
    {
        GoogleCredential credential;
        using var stream = new FileStream("../../../client_secrets.json", FileMode.Open, FileAccess.Read);
        credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        
        return credential;
    }
}