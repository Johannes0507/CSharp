using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace az204_auth_MSALConsoleVersion
{
    class MSALAuth 
    {
        private const string _clientId = "2bac0d7b-6502-412d-bb98-d0fd8c9e77c7";
        private const string _tenantId = "1ff87c21-64ac-489e-8e5b-2a48d0d86c54";
        public static async Task Main(string[] args)
        {
            string[] scopes = { "user.read" };
            var app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .WithRedirectUri("http://localhost")
                .Build();

            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            Console.WriteLine($"Token:\t{result.AccessToken}");
        }

    }
}

