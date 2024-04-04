using Core.Application.Abstractions.Persistence.Repository.Writing;
using Core.Tests.Attributes;
using Core.Users.Domain;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.ApplicationUserRepository;

public class ApplicationUserWriteRepository
{
    private readonly IBaseWriteRepository<ApplicationUser> _userWriteRepository;

    
    public ApplicationUserWriteRepository()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        var dbContext = new ApplicationDbContext(dbOptions);
     
        _userWriteRepository = new BaseRepository<ApplicationUser>(dbContext);
    }
    
    [Theory, FixtureInlineAutoData]
    public async Task Should_BeValid_When_AddCorrectUsers(ApplicationUser user, ApplicationUser user2)
    {
        // arrange
        user.Roles = new[] { new ApplicationUserApplicationUserRole() { ApplicationUserRoleId = 1 } };
        user2.Roles = new[] { new ApplicationUserApplicationUserRole() { ApplicationUserRoleId = 2 } };
        
        // act
        await _userWriteRepository.AddAsync(user, CancellationToken.None);
        await _userWriteRepository.AddAsync(user2, CancellationToken.None);

        // assert
        var count = await _userWriteRepository.AsAsyncRead().CountAsync(CancellationToken.None);
        Assert.Equal(2, count);
    }
    
}