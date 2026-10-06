namespace SimplePos.Application.Auths.Common;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);
