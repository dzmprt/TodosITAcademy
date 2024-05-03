using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Todos.Application.Abstractions.ExternalProviders;
using Todos.Application.Abstractions.Persistence.Repository.Read;
using Todos.Application.Abstractions.Persistence.Repository.Writing;
using Todos.Domain;
using Todos.Domain.Enums;
using Todos.ExternalProviders.Exceptions;
using Todos.ExternalProviders.Models;

namespace Todos.ExternalProviders;

public class OwnersProvider : IOwnersProvider
{
    private readonly IBaseWriteRepository<Owner> _owners;
    
    private readonly IBaseReadRepository<OwnerRole> _ownerRoles;

    private readonly IConfiguration _configuration;
    
    private readonly HttpClient _httpClient;
    
    public OwnersProvider(IHttpClientFactory httpClientFactory, 
        IBaseWriteRepository<Owner> owners, 
        IBaseReadRepository<OwnerRole> ownerRoles,
        IConfiguration configuration)
    {
        _owners = owners;
        _ownerRoles = ownerRoles;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<Owner> GetOwnerAsync(Guid ownerId, CancellationToken cancellationToken)
    {
        var result = await _owners.AsAsyncRead().SingleOrDefaultAsync(u => u.OwnerId == ownerId, cancellationToken);
        if (result is not null)
        {
            return result;
        }
        
        var userServiceUrl = _configuration["UserServiceApiUrl"];
        var getUserApiMethodUrl = $"{userServiceUrl}/api/v1/Users/{ownerId.ToString()}";
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, getUserApiMethodUrl);
        var responseMessage = await _httpClient.SendAsync(httpRequest, cancellationToken);
        if (!responseMessage.IsSuccessStatusCode)
        {
            var serviceName = "UserService";
            var requestUrlMessage = $"request url '{getUserApiMethodUrl}'";
            if (responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new ExternalServiceNotAvailable(serviceName, requestUrlMessage);
            }

            throw new ExternalServiceBadResult(serviceName, requestUrlMessage);
        }

        var jsonResult = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var getUserDto = JsonSerializer.Deserialize<GetUserDto>(jsonResult, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var rolesDomains = (await _ownerRoles.AsAsyncRead().ToArrayAsync(cancellationToken)).Where(r => getUserDto!.Roles.Contains(r.RoleId)).ToArray();
        result = new Owner(getUserDto!.ApplicationUserId, rolesDomains);
        result = await _owners.AddAsync(result, cancellationToken);
        return result;
    }
}