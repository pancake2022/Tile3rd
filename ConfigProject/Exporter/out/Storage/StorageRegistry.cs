using System.Collections.Generic;

namespace CSFramework
{
    public class StorageRegistry : IStorageRegistry
    {
        public List<Storage> GetRegisteredStorage ()
        {
            return new List<Storage>
            {
                new CommonStorage(), // 公用存档
                new LevelStorage(), // 关卡存档
            };
        }
    }
}