using System.Collections.Generic;

namespace CSFramework
{
    public interface IStorageRegistry
    {
        List<Storage> GetRegisteredStorage();
    }
}