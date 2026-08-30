namespace SimplePos.Application.Users.Queries.GetOutletAccess;

public record UserOutletAccessResponse(
    Guid OutletId,
    string OutletName,
    string? PhoneNumber,
    string City,
    string State
);
