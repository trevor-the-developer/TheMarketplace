using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Api.Endpoints.Authentication.Token
{
    public record TokenRevokeRequest(string AccessToken, string RefreshToken);
}