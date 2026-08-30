using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Users;
using Xunit;

namespace Tests.Domain.Users;

public class UserOutletAccessTests
{
    private User CreateTestUser(Guid companyId, Guid? outletId = null)
    {
        var email = EmailAddress.Create("user@example.com").Data!;
        return User.Create(outletId, companyId, "testuser", email, "1234567890", "Manager").Data!;
    }

    [Fact]
    public void Create_ShouldSucceed_WhenValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var outletId = Guid.NewGuid();

        // Act
        Result<UserOutletAccess> result = UserOutletAccess.Create(userId, outletId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(outletId, result.Data.OutletId);
    }

    [Fact]
    public void Create_ShouldFail_WhenUserIdIsEmpty()
    {
        // Act
        Result<UserOutletAccess> result = UserOutletAccess.Create(Guid.Empty, Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserOutletAccessError.UserIdEmpty, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenOutletIdIsEmpty()
    {
        // Act
        Result<UserOutletAccess> result = UserOutletAccess.Create(Guid.NewGuid(), Guid.Empty);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserOutletAccessError.OutletIdEmpty, result.Error);
    }

    [Fact]
    public void AssignOutletAccess_ShouldSucceed_WhenValid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var user = CreateTestUser(companyId);
        var outletId = Guid.NewGuid();

        // Act
        Result result = user.AssignOutletAccess(outletId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(user.UserOutletAccesses);
        Assert.Equal(outletId, user.UserOutletAccesses.First().OutletId);
    }

    [Fact]
    public void AssignOutletAccess_ShouldFail_WhenOutletIdIsEmpty()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());

        // Act
        Result result = user.AssignOutletAccess(Guid.Empty);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserOutletAccessError.OutletIdEmpty, result.Error);
        Assert.Empty(user.UserOutletAccesses);
    }

    [Fact]
    public void AssignOutletAccess_ShouldFail_WhenAlreadyAssigned()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());
        var outletId = Guid.NewGuid();
        user.AssignOutletAccess(outletId);

        // Act
        Result result = user.AssignOutletAccess(outletId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserOutletAccessError.OutletAlreadyAssigned, result.Error);
    }

    [Fact]
    public void AssignOutletAccess_ShouldFail_WhenUserIsSoftDeleted()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());
        user.SoftDelete();

        // Act
        Result result = user.AssignOutletAccess(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserError.SoftDeleted, result.Error);
    }

    [Fact]
    public void RevokeOutletAccess_ShouldSucceed_WhenAssigned()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());
        var outletId = Guid.NewGuid();
        user.AssignOutletAccess(outletId);

        // Act
        Result result = user.RevokeOutletAccess(outletId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(user.UserOutletAccesses);
    }

    [Fact]
    public void RevokeOutletAccess_ShouldFail_WhenNotAssigned()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());

        // Act
        Result result = user.RevokeOutletAccess(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserOutletAccessError.OutletNotAssigned, result.Error);
    }

    [Fact]
    public void ClearOutletAccesses_ShouldRemoveAllAssignments()
    {
        // Arrange
        var user = CreateTestUser(Guid.NewGuid());
        user.AssignOutletAccess(Guid.NewGuid());
        user.AssignOutletAccess(Guid.NewGuid());
        Assert.Equal(2, user.UserOutletAccesses.Count);

        // Act
        Result result = user.ClearOutletAccesses();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(user.UserOutletAccesses);
    }

    [Fact]
    public void CanAccessOutlet_CompanyAdmin_ShouldAccessAllOutletsInSameCompany()
    {
        // Arrange: OutletId is null and no UserOutletAccess entries -> Full Admin
        var companyId = Guid.NewGuid();
        var user = CreateTestUser(companyId, outletId: null);
        var outlet1 = Guid.NewGuid();
        var outlet2 = Guid.NewGuid();

        // Act & Assert
        Assert.True(user.CanAccessOutlet(outlet1, companyId));
        Assert.True(user.CanAccessOutlet(outlet2, companyId));
    }

    [Fact]
    public void CanAccessOutlet_CompanyAdmin_ShouldDenyAccessToDifferentCompany()
    {
        // Arrange
        var companyId1 = Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        var user = CreateTestUser(companyId1, outletId: null);
        var outletInCompany2 = Guid.NewGuid();

        // Act & Assert
        Assert.False(user.CanAccessOutlet(outletInCompany2, companyId2));
    }

    [Fact]
    public void CanAccessOutlet_RegionalManager_ShouldOnlyAccessAssignedOutlets()
    {
        // Arrange: OutletId is null, but specific UserOutletAccess records assigned
        var companyId = Guid.NewGuid();
        var user = CreateTestUser(companyId, outletId: null);
        var branchNorth = Guid.NewGuid();
        var branchSouth = Guid.NewGuid();
        var branchEast = Guid.NewGuid();

        user.AssignOutletAccess(branchNorth);
        user.AssignOutletAccess(branchSouth);

        // Act & Assert
        Assert.True(user.CanAccessOutlet(branchNorth, companyId));
        Assert.True(user.CanAccessOutlet(branchSouth, companyId));
        Assert.False(user.CanAccessOutlet(branchEast, companyId)); // Not assigned
    }

    [Fact]
    public void CanAccessOutlet_BranchStaff_ShouldOnlyAccessAssignedOutlet()
    {
        // Arrange: OutletId has a specific value
        var companyId = Guid.NewGuid();
        var assignedOutlet = Guid.NewGuid();
        var otherOutlet = Guid.NewGuid();
        var user = CreateTestUser(companyId, outletId: assignedOutlet);

        // Act & Assert
        Assert.True(user.CanAccessOutlet(assignedOutlet, companyId));
        Assert.False(user.CanAccessOutlet(otherOutlet, companyId));
    }

    [Fact]
    public void CanAccessOutlet_ShouldDeny_WhenUserIsSoftDeletedOrInactive()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var user = CreateTestUser(companyId, outletId: null);
        var outlet = Guid.NewGuid();
        user.SoftDelete();

        // Act & Assert
        Assert.False(user.CanAccessOutlet(outlet, companyId));
    }
}
