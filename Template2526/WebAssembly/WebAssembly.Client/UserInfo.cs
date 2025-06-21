namespace WebAssembly.Client
{
    // Add properties to this class and update the server and client AuthenticationStateProviders
    // to expose more information about the authenticated user to the client.
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        // NEU: Rollenliste des Benutzers
        public List<string> Roles { get; set; } = [];
    }
}
