using SnapTraceV2.Args;
using SnapTraceV2.Models.Http;
using SnapTraceV2.Models.Requests;
using SnapTraceV2.Services;
using System.Security.Claims;

namespace SnapTraceV2.Options;

internal class UserOptions
{
    private static readonly string[] NameClaims = [ClaimTypes.Name, ClaimTypes.GivenName, ClaimTypes.Surname, ClaimTypes.Email, "name", "username", "email", "emailaddress"];
    private static readonly string[] EmailAddressClaims = [ClaimTypes.Email, "email", "emailaddress"];
    private static readonly string[] AvatarClaims = ["avatar", "picture", "image"];

    internal HandlersContainer Handlers { get; }

    public UserOptions() => Handlers = new HandlersContainer();

    internal class HandlersContainer
    {
        public Func<HttpRequest, UserRequest> CreateUserPayload { get; set; }
        public Func<FlushLogArgs, IEnumerable<string>> GenerateSearchKeywords { get; set; }

        public HandlersContainer()
        {
            CreateUserPayload = (args) =>
            {
                return new UserRequest
                {
                    Name = args.Properties.Claims.FirstOrDefault(p => NameClaims.Contains(p.Key.ToLowerInvariant())).Value,
                    EmailAddress = args.Properties.Claims.FirstOrDefault(p => EmailAddressClaims.Contains(p.Key.ToLowerInvariant())).Value,
                    Avatar = args.Properties.Claims.FirstOrDefault(p => AvatarClaims.Contains(p.Key.ToLowerInvariant())).Value
                };
            };

            GenerateSearchKeywords = (args) =>
            {
                var service = new GenerateSearchKeywordsService();
                return service.GenerateKeywords(args);
            };
        }
    }
}
