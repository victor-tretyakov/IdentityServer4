// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers;

public class ClientMappersTests
{
    [Fact]
    public void Can_Map()
    {
        var model = new Models.Client();
        var mappedEntity = model.ToEntity();
        var mappedModel = mappedEntity.ToModel();

        Assert.NotNull(mappedModel);
        Assert.NotNull(mappedEntity);
    }

    [Fact]
    public void Properties_Map()
    {
        var model = new Models.Client()
        {
            Properties =
            {
                {"foo1", "bar1"},
                {"foo2", "bar2"},
            }
        };


        var mappedEntity = model.ToEntity();

        mappedEntity.Properties.Count.Should().Be(2);
        var foo1 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo1");
        foo1.Should().NotBeNull();
        foo1.Value.Should().Be("bar1");
        var foo2 = mappedEntity.Properties.FirstOrDefault(x => x.Key == "foo2");
        foo2.Should().NotBeNull();
        foo2.Value.Should().Be("bar2");



        var mappedModel = mappedEntity.ToModel();

        mappedModel.Properties.Count.Should().Be(2);
        mappedModel.Properties.ContainsKey("foo1").Should().BeTrue();
        mappedModel.Properties.ContainsKey("foo2").Should().BeTrue();
        mappedModel.Properties["foo1"].Should().Be("bar1");
        mappedModel.Properties["foo2"].Should().Be("bar2");
    }

    [Fact]
    public void duplicates_properties_in_db_map()
    {
        var entity = new Entities.Client
        {
            Properties = new List<Entities.ClientProperty>()
            {
                new() {Key = "foo1", Value = "bar1"},
                new() {Key = "foo1", Value = "bar2"},
            }
        };

        Action modelAction = () => entity.ToModel();
        modelAction.Should().Throw<Exception>();
    }

    [Fact]
    public void missing_values_should_use_defaults()
    {
        var entity = new Entities.Client
        {
            ClientSecrets = new List<Entities.ClientSecret>
            {
                new() {
                }
            }
        };

        var def = new Client
        {
            ClientSecrets = { new Secret("foo") }
        };

        var model = entity.ToModel();
        model.ProtocolType.Should().Be(def.ProtocolType);
        model.ClientSecrets.First().Type.Should().Be(def.ClientSecrets.First().Type);
    }


    [Fact]
    public void mapping_model_to_entity_maps_all_properties()
    {
        var notMapped = new string[]
        {
            "Id",
            "Updated",
            "LastAccessed",
            "NonEditable",
        };

        var notAutoInitialized = new string[]
        {
            "AllowedGrantTypes",
        };

        MapperTestHelpers
            .AllPropertiesAreMapped<Client, Entities.Client>(
                notAutoInitialized,
                source => {
                    source.AllowedIdentityTokenSigningAlgorithms.Add("RS256"); // We have to add values, otherwise the converter will produce null
                    source.AllowedGrantTypes = new List<string>
                    {
                        GrantType.AuthorizationCode // We need to set real values for the grant types, because they are validated
                    };
                },
                source => source.ToEntity(),
                notMapped,
                out var unmappedMembers)
            .Should()
            .BeTrue($"{string.Join(',', unmappedMembers)} should be mapped");
    }

    [Fact]
    public void mapping_entity_to_model_maps_all_properties()
    {
        MapperTestHelpers
            .AllPropertiesAreMapped<Entities.Client, Client>(
                source => source.ToModel(),
                out var unmappedMembers)
            .Should()
            .BeTrue($"{string.Join(',', unmappedMembers)} should be mapped");
    }

    private enum TestEnum
    {
        Foo, Bar
    }

    private class ExtendedClientEntity : Entities.Client
    {
        public int Number { get; set; }
        public bool Flag { get; set; }
        public TestEnum Enumeration { get; set; }
        public IEnumerable<string> Enumerable { get; set; }
        public List<string> List { get; set; }
        public Dictionary<string, string> Dictionary { get; set; }

        public ExtendedClientEntity(Entities.Client client)
        {
            var extendedType = typeof(ExtendedClientEntity);
            var baseType = typeof(Entities.Client);

            foreach (var baseProperty in baseType.GetProperties())
            {
                var derivedProperty = extendedType.GetProperty(baseProperty.Name);
                if (derivedProperty != null && derivedProperty.CanWrite && baseProperty.CanRead)
                {
                    var value = baseProperty.GetValue(client);
                    derivedProperty.SetValue(this, value);
                }
            }
        }
    }

    private class ExtendedClientModel : Client
    {
        public ExtendedClientEntity ToExtendedEntity()
        {
            return new ExtendedClientEntity(this.ToEntity());
        }
    }

    private static int CountForgottenProperties<TBase, TDerived>() where TDerived : TBase
    {
        var baseProperties = typeof(TBase).GetProperties();
        var derivedProperties = typeof(TDerived).GetProperties();

        return derivedProperties
            .Count(derivedProperty => !baseProperties.Any(baseProp => baseProp.Name == derivedProperty.Name));
    }

    [Fact]
    public void forgetting_to_map_properties_is_checked_by_tests()
    {
        var notMapped = new string[]
        {
            "Id",
            "Updated",
            "LastAccessed",
            "NonEditable"
        };

        var notAutoInitialized = new string[]
        {
            "AllowedGrantTypes",
        };

        MapperTestHelpers
            .AllPropertiesAreMapped<ExtendedClientModel, ExtendedClientEntity>(
                notAutoInitialized,
                source => {
                    source.AllowedIdentityTokenSigningAlgorithms.Add("RS256"); // We have to add values, otherwise the converter will produce null
                    source.AllowedGrantTypes = new List<string>
                    {
                        GrantType.AuthorizationCode // We need to set real values for the grant types, because they are validated
                    };
                },
                source => source.ToExtendedEntity(),
                notMapped,
                out var unmappedMembers)
            .Should()
            .BeFalse();
        unmappedMembers.Count.Should().Be(CountForgottenProperties<Entities.Client, ExtendedClientEntity>());
    }
}