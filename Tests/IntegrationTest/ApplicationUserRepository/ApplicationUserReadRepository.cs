using Core.Application.Abstractions.Persistence.Repository.Read;
using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Tests.Attributes;
using Core.Users.Domain;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.ApplicationUserRepository;

public class ApplicationUserReadRepository
{
    private readonly IBaseReadRepository<ApplicationUser> _userReadRepository;

    private readonly IBaseWriteRepository<ApplicationUser> _userWriteRepository;

    public ApplicationUserReadRepository()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        var dbContext = new ApplicationDbContext(dbOptions);
        _userWriteRepository = new BaseRepository<ApplicationUser>(dbContext);
        _userReadRepository = new BaseRepository<ApplicationUser>(dbContext);
    }

    [Theory]
    [FixtureInlineAutoData("485df797-7978-4542-9381-dcb9709a233c")]
    [FixtureInlineAutoData("77e11069-507a-487f-94e4-6b9e9e93df8e")]
    public async Task GetUserById(string userId, ApplicationUser user)
    {
        // arrange
        var id = Guid.Parse(userId);
        user.ApplicationUserId = id;
        user.Roles = new[] { new ApplicationUserApplicationUserRole() { ApplicationUserRoleId = 1 } };
        await _userWriteRepository.AddAsync(user, CancellationToken.None);

        // act
        var userFromDb = await _userReadRepository.AsAsyncRead()
            .SingleAsync(u => u.ApplicationUserId == id, CancellationToken.None);

        // assert
        Assert.Equal(userFromDb.ApplicationUserId, id);
    }
}