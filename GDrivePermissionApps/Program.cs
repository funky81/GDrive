using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDrivePermissionApps
{

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Drive.v3.Data;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    namespace DriveQuickstart
    {
        class Program
        {
            static void Main(string[] args)
            {
                new Apps().Iterate();

            }

        }
    }
}
