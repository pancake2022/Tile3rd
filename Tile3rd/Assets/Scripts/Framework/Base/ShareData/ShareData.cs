using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace CSFramework
{
    public abstract class ShareData<TClass> : ListenableData<TClass> where TClass : ListenableData<TClass>
    {
        
    }
}