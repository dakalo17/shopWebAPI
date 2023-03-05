using shopWebAPI.Models.Jwt;

namespace shopWebAPI.Repository.Jwt
{
	public class UserRefreshTokenRepository : IUserRefreshTokenRepository
	{
		public UserRefreshTokenRepository() { 
		}

		private Dictionary<string,string> _userTokens = new Dictionary<string, string>();

		public bool RefreshTokenIsValid(UserRefreshToken obj)
		{
			_userTokens.TryGetValue(obj.Username, out var token );
			
			//compare the token from the client with the1 in the server
			//if == then it is a valid token
			return obj.RefreshToken.Equals(token);
		}

		public void SaveRefreshToken(UserRefreshToken obj)
		{
			//insert,update key to relavent user
			if (_userTokens.ContainsKey(obj.Username))
			{
				_userTokens[obj.Username] = obj.RefreshToken;
			}
			else
			{
				_userTokens.Add(obj.Username, obj.RefreshToken);
			}
		}
	}
}
