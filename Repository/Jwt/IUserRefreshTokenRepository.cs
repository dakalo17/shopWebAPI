using shopWebAPI.Models.Jwt;

namespace shopWebAPI.Repository.Jwt
{
	public interface IUserRefreshTokenRepository
	{
		void SaveRefreshToken(UserRefreshToken obj);
		bool RefreshTokenIsValid(UserRefreshToken obj);
	}
}
