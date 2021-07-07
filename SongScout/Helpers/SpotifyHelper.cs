using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongScout.Helpers
{
    class SpotifyHelper
    {
        public string OauthToken { get; set; }

        public async Task GetToken()
        {
            #region Secret
            string CLIENT_ID = "11305c19e8df4f659ae8d72bbbd6b48c";
            string CLIENT_SECRET = "ca577280e81749ada1fac09de061f655";
            #endregion

            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(CLIENT_ID, CLIENT_SECRET);
            var response = await new OAuthClient(config).RequestToken(request);

            OauthToken = response.AccessToken;
        }
    }
}
