using System;
using System.Collections.Generic;

namespace iSoul.Model
{
    public interface IRelationshipProvider<TModelA, TModelB> : IModelProvider
        where TModelA : IViewModel
        where TModelB : IViewModel
    {
        List<KeyValuePair<Guid, Guid>> Relationships { get; }

        void AddB(Guid id, TModelB model);

        void AddA(Guid id, TModelA model);

        void RemoveB(Guid id, TModelB model);

        void RemoveA(Guid id, TModelA model);
    }
}
