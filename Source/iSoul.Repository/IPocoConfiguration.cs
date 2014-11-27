using System;

namespace iSoul.Repository
{
    public interface IPocoConfiguration
    {
        Type PocoType { get; }

        object Configuration { get; }
    }
}
