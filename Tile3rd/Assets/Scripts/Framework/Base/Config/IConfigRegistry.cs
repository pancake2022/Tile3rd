using System;
using System.Collections.Generic;

namespace CSFramework
{
    public interface IConfigRegistry
    {
        List<KeyValuePair<string, Type>> GetRegisteredConfig();
    }
}