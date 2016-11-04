using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace FutureTech.Google.Drive
{
    internal class Apps
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "GDriveApps.Sharing";
        public Apps()
        {
        }
        public void Iterate()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var request = service.Files.List();
            request.PageSize = 500;

            var result = request.Execute();
            do
            {
                result = request.Execute();
                foreach (var file in result.Files)
                {
                    try
                    {
                        var filePermission = service.Permissions.List(file.Id);
                        filePermission.Fields = "kind,permissions";
                        var permissions = filePermission.Execute().Permissions;
                        Console.WriteLine(file.Name);
                        foreach (var permission in permissions)
                        {
                            if (permission.Role != "owner")
                            {

                                if (permission.DisplayName != null)
                                {
                                    Console.Write("-");
                                    Console.Write(permission.DisplayName);
                                }
                                Console.WriteLine();
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //if (file.Shared.HasValue && file.Shared.Value)
                    //    Console.WriteLine(file.SharingUser.DisplayName);
                }
                request.PageToken = result.NextPageToken;
            } while (result.NextPageToken != null);

            Console.Read();
        }

    }
}