using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Services.Default.KeyManagement;

internal class MockSigningKeyStore : ISigningKeyStore
{
    public List<SerializedKey> Keys { get; set; } = new List<SerializedKey>();
    public bool LoadKeysAsyncWasCalled { get; set; }
    public bool DeleteWasCalled { get; set; }

    public Task DeleteKeyAsync(string id)
    {
        DeleteWasCalled = true;
        if (Keys != null)
        {
            Keys.Remove(Keys.FirstOrDefault(x => x.Id == id));
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<SerializedKey>> LoadKeysAsync()
    {
        LoadKeysAsyncWasCalled = true;
        return Task.FromResult<IEnumerable<SerializedKey>>(Keys);
    }

    public Task StoreKeyAsync(SerializedKey key)
    {
        Keys ??= new List<SerializedKey>();

        Keys.Add(key);
        return Task.CompletedTask;
    }
}