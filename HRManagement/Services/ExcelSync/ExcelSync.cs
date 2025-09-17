//using Azure.Identity;
//using Microsoft.Identity.Client;  // Not using this now and also delete from installed packages (identity client)
//using Microsoft.Graph;            // To access Microsoft Graph API

//namespace HRManagement.Services.ExcelSync
//{
//    public class ExcelSync
//    {
//        public async Task SyncExcelDataAsync()
//        {

//            string tenantId = "<TENANT_ID>";
//            string clientId = "<CLIENT_ID>";
//            string clientSecret = "<CLIENT_SECRET>";

//            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
//            var scopes = new[] { "https://graph.microsoft.com/.default" };

//            var graphClient = new GraphServiceClient(credential, scopes);



//            string excelFilePath = "/Documents/EmployeesDataExcel.xlsx";

//            var drive = await graphClient.Me.Drive.GetAsync();

//            var driveItem = await graphClient.Me.Drive.ItemWithPath(excelFilePath).GetAsync();





//        }

//    }
//}




