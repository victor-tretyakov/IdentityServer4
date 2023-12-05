// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

#pragma warning disable 1591

namespace IdentityServer4.Stores.Serialization
{
    public class CustomContractResolver: DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var jsonTypeInfo = base.GetTypeInfo(type, options);

            return jsonTypeInfo;
        }
    }
}