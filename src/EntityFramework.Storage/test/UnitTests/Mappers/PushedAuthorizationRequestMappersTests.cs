using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers;

public class PushedAuthorizationRequestMappersTests
{
    [Fact]
    public void CanMapPushedAuthorizationRequest()
    {
        var model = new Models.PushedAuthorizationRequest();
        var mappedEntity = model.ToEntity();
        var mappedModel = mappedEntity.ToModel();

        Assert.NotNull(mappedModel);
        Assert.NotNull(mappedEntity);
    }

    [Fact]
    public void mapping_model_to_entity_maps_all_properties()
    {
        var excludedProperties = new string[]
        {
            "Id",
        };

        MapperTestHelpers
            .AllPropertiesAreMapped<Models.PushedAuthorizationRequest, Entities.PushedAuthorizationRequest>(source => source.ToEntity(), excludedProperties, out var unmappedMembers)
            .Should()
            .BeTrue($"{string.Join(',', unmappedMembers)} should be mapped");
    }

    [Fact]
    public void mapping_entity_to_model_maps_all_properties()
    {
        MapperTestHelpers
            .AllPropertiesAreMapped<Entities.PushedAuthorizationRequest, Models.PushedAuthorizationRequest>(source => source.ToModel(), out var unmappedMembers)
            .Should()
            .BeTrue($"{string.Join(',', unmappedMembers)} should be mapped");
    }
}