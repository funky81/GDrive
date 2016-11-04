using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FutureTech.Google.Drive
{
    public class Share
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }
    }
    internal class Apps
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "GDriveApps.Sharing";
        public Apps()
        {
        }
        public Dictionary<string, List<Share>> GetList()
        {
            var list = new Dictionary<string, List<Share>>();

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
                //request.Q = "not 'funky81.milis@gmail.com' in readers";
                result = request.Execute();
                foreach (var file in result.Files)
                {
                    try
                    {
                        var filePermission = service.Permissions.List(file.Id);
                        filePermission.Fields = "kind,permissions";
                        var permissions = filePermission.Execute().Permissions;
                        //Console.WriteLine(file.Name);
                        foreach (var permission in permissions)
                        {
                            if (permission.Role != "owner")
                            {

                                if (permission.DisplayName != null)
                                {
                                    var share = new Share()
                                    {
                                        Email = permission.EmailAddress,
                                        FileId = file.Id,
                                        FileName = file.Name,
                                        Role = permission.Role,
                                        UserName = permission.DisplayName,
                                        Type = file.MimeType
                                    };
                                    if (!list.ContainsKey(file.Id))
                                    {
                                        list.Add(file.Id, new List<Share>());
                                    }
                                    var value = list.FirstOrDefault(x => x.Key == file.Id);
                                    value.Value.Add(share);
                                    //Console.Write("-");
                                    //Console.Write(permission.DisplayName);
                                }
                                //Console.WriteLine();
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                    }
                    //if (file.Shared.HasValue && file.Shared.Value)
                    //    Console.WriteLine(file.SharingUser.DisplayName);
                }
                request.PageToken = result.NextPageToken;
            } while (result.NextPageToken != null);

            //Console.Read();
            return list;
        }
        internal void Iterate()
        {
            var list = GetList();
            foreach (var key in list.Keys)
            {
                foreach (var value in list.Where(x => x.Key == key))
                {
                    var shareList = value.Value;
                    foreach (var listX in shareList.ToList())
                    {
                        Console.WriteLine(listX.FileName + "-" + listX.Type + "-" + listX.Role + "-" + listX.UserName + "-" + listX.Email);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}