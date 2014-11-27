using iSoul.Repository;
using Microsoft.Practices.Unity;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iSoul.Model.Core
{
    class ModelService : IModelService
    {
        private IUnityContainer container;

        public ModelService(IUnityContainer container)
        {
            this.container = container;
        }

        #region IModelService 成员

        public TModel Get<TModel>(Guid id) where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            TModel model;
            var key = this.GenKey(typeof(TModel), id);
            using (RedisClient client = this.GetClient())
            {
                model = client.Get<TModel>(key);
            }
            var m = model as ViewModel;
            m.ModelService = this;
            return model;
        }

        public TModel Get<TModel>(int id) where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            TModel model;
            var key = this.GenIdentityKey(typeof(TModel), id);
            using (RedisClient client = this.GetClient())
            {
                var modelKey = client.Get<string>(key);
                model = client.Get<TModel>(modelKey);
            }
            var m = model as ViewModel;
            m.ModelService = this;
            return model;
        }

        public ICollection<TModel> GetAll<TModel>() where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            var key = this.GenKey(typeof(TModel), Guid.Empty);
            using (RedisClient client = this.GetClient())
            {
                var keys = client.SearchKeys(key);
                var models = client.GetAll<TModel>(keys).Values;
                foreach (var model in models)
                {
                    var m = model as ViewModel;
                    m.ModelService = this;
                }
                return models;
            }
        }

        public ICollection<TModelB> GetMany<TModelA, TModelB>(Guid id)
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            this.CheckRelationshipCache<TModelA, TModelB>();
            List<TModelB> models = new List<TModelB>();
            var modelAType = typeof(TModelA);
            var modelBType = typeof(TModelB);
            var keyA = this.GenManyKey(modelAType, modelBType);
            var keyB = this.GenManyKey(modelBType, modelAType);
            using (RedisClient client = this.GetClient())
            {
                if (client.ContainsKey(keyA))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyA);
                    var modelBIdentities = relationship.Where(r => r.Key.Equals(id)).Select(r => r.Value);
                    foreach (var item in modelBIdentities)
                    {
                        var model = this.Get<TModelB>(item);
                        models.Add(model);
                    }
                }
                else if (client.ContainsKey(keyB))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyB);
                    var modelBIdentities = relationship.Where(r => r.Value.Equals(id)).Select(r => r.Key);
                    foreach (var item in modelBIdentities)
                    {
                        var model = this.Get<TModelB>(item);
                        models.Add(model);
                    }
                }
            }
            return models;
        }
        public void Create<TModel>(TModel model) where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            var provider = GetProvider<TModel>();
            var currentModel = provider.Create(model);
            this.CacheModel<TModel>(currentModel);
        }

        public void Edit<TModel>(TModel model) where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            var provider = GetProvider<TModel>();
            var currentModel = provider.Edit(model);
            this.CacheModel<TModel>(currentModel);
        }

        public void Delete<TModel>(Guid id) where TModel : IViewModel
        {
            this.CheckCache<TModel>();

            var modelType = typeof(TModel);
            var provider = GetProvider<TModel>();
            var model = provider.Delete(id);
            this.RemoveCache<TModel>(model);
        }

        public void Add<TTarget, TModel>(Guid id, TModel model)
            where TTarget : IViewModel
            where TModel : IViewModel
        {
            this.CheckRelationshipCache<TTarget, TModel>();
            var providerAB = this.GetRelationshipProvider<TTarget, TModel>();
            if (providerAB != null)
            {
                providerAB.AddB(id, model);
            }
            var providerBA = this.GetRelationshipProvider<TModel, TTarget>();
            if (providerBA != null)
            {
                providerBA.AddA(id, model);
            }
            this.AddRelationshipCache<TTarget, TModel>(id, model.Identity);
        }

        public void Remove<TTarget, TModel>(Guid id, TModel model)
            where TTarget : IViewModel
            where TModel : IViewModel
        {
            this.CheckRelationshipCache<TTarget, TModel>();
            var providerAB = this.GetRelationshipProvider<TTarget, TModel>();
            if (providerAB != null)
            {
                providerAB.RemoveB(id, model);
            }
            var providerBA = this.GetRelationshipProvider<TModel, TTarget>();
            if (providerBA != null)
            {
                providerBA.RemoveA(id, model);
            }
            this.RemoveRelationshipCache<TTarget, TModel>(id, model.Identity);
        }

        #endregion

        #region Private Months

        private string GenKey(Type type, Guid id)
        {
            if (id == Guid.Empty)
            {
                return String.Format("{0}:*", type.FullName);
            }
            else
            {
                return String.Format("{0}:{1}", type.FullName, id.ToString());
            }
        }

        private string GenIdentityKey(Type type, int id)
        {
            return String.Format("ModekKey-{0}:{1}", type.FullName, id.ToString());
        }

        private string GenManyKey(Type one, Type many)
        {
            return String.Format("ManyToMany:{0}-{1}", one.FullName, many.FullName);
        }

        private void CheckCache<TModel>() where TModel : IViewModel
        {
            var modelType = typeof(TModel);
            using (var client = this.GetClient())
            {
                var cacheKey = String.Format("ModelCache:{0}", modelType.FullName);
                if (!client.ContainsKey(cacheKey))
                {
                    var provider = GetProvider<TModel>();
                    foreach (var model in provider.Models)
                    {
                        this.CacheModel<TModel>(model);
                    }
                    client.Set<bool>(cacheKey, true);
                }
            }
        }

        private void CheckRelationshipCache<TModelA, TModelB>()
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            this.CheckCache<TModelA>();
            this.CheckCache<TModelB>();
            var modelAType = typeof(TModelA);
            var modelBType = typeof(TModelB);
            using (var client = this.GetClient())
            {
                var cacheKeyA = String.Format("ModelRelationshipCache:{0}-{1}", modelAType.FullName, modelBType.FullName);
                var cacheKeyB = String.Format("ModelRelationshipCache:{0}-{1}", modelBType.FullName, modelAType.FullName);
                if (!client.ContainsKey(cacheKeyA) && !client.ContainsKey(cacheKeyB))
                {
                    var providerAB = this.GetRelationshipProvider<TModelA, TModelB>();
                    if (providerAB != null)
                    {
                        this.CacheRelationship<TModelA, TModelB>(client, providerAB.Relationships);
                        client.Set<bool>(cacheKeyA, true);
                        client.Set<bool>(cacheKeyB, true);
                    }
                    var providerBA = this.GetRelationshipProvider<TModelB, TModelA>();
                    if (providerBA != null)
                    {
                        this.CacheRelationship<TModelA, TModelB>(client, providerBA.Relationships);
                        client.Set<bool>(cacheKeyA, true);
                        client.Set<bool>(cacheKeyB, true);
                    }
                }
            }
        }

        private void CacheRelationship<TModelA, TModelB>(RedisClient client, List<KeyValuePair<Guid, Guid>> list)
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            var modelAType = typeof(TModelA);
            var modelBType = typeof(TModelB);
            var key = this.GenManyKey(modelAType, modelBType);
            client.Set<List<KeyValuePair<Guid, Guid>>>(key, list);
        }

        private void AddRelationshipCache<TModelA, TModelB>(Guid aid, Guid bid)
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            this.CheckRelationshipCache<TModelA, TModelB>();
            var modelAType = typeof(TModelA);
            var modelBType = typeof(TModelB);
            var keyA = this.GenManyKey(modelAType, modelBType);
            var keyB = this.GenManyKey(modelBType, modelAType);
            using (RedisClient client = this.GetClient())
            {
                if (client.ContainsKey(keyA))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyA);
                    if (!relationship.Any(r => r.Key.Equals(aid) && r.Value.Equals(bid)))
                    {
                        relationship.Add(new KeyValuePair<Guid, Guid>(aid, bid));
                        client.Set<List<KeyValuePair<Guid, Guid>>>(keyA, relationship);
                    }
                }
                else if (client.ContainsKey(keyB))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyB);
                    if (!relationship.Any(r => r.Key.Equals(bid) && r.Value.Equals(aid)))
                    {
                        relationship.Add(new KeyValuePair<Guid, Guid>(bid, aid));
                        client.Set<List<KeyValuePair<Guid, Guid>>>(keyB, relationship);
                    }
                }
            }
        }

        private void RemoveRelationshipCache<TModelA, TModelB>(Guid aid, Guid bid)
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            this.CheckRelationshipCache<TModelA, TModelB>();
            var modelAType = typeof(TModelA);
            var modelBType = typeof(TModelB);
            var keyA = this.GenManyKey(modelAType, modelBType);
            var keyB = this.GenManyKey(modelBType, modelAType);
            using (RedisClient client = this.GetClient())
            {
                if (client.ContainsKey(keyA))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyA);
                    var keyvalue = relationship.SingleOrDefault(r => r.Key.Equals(aid) && r.Value.Equals(bid));
                    relationship.Remove(keyvalue);
                    client.Set<List<KeyValuePair<Guid, Guid>>>(keyA, relationship);
                }
                else if (client.ContainsKey(keyB))
                {
                    var relationship = client.Get<List<KeyValuePair<Guid, Guid>>>(keyB);
                    var keyvalue = relationship.Single(r => r.Key.Equals(bid) && r.Value.Equals(aid));
                    relationship.Remove(keyvalue);
                    client.Set<List<KeyValuePair<Guid, Guid>>>(keyB, relationship);
                }
            }
        }

        private void CacheModel<TModel>(TModel model) where TModel : IViewModel
        {
            using (var client = this.GetClient())
            {
                var modelType = typeof(TModel);
                var key = this.GenKey(modelType, model.Identity);
                client.Set<TModel>(key, model);

                var modekKey = this.GenIdentityKey(modelType, model.DisplayIdentity);
                client.Set<string>(modekKey, key);
            }
        }

        private void RemoveCache<TModel>(TModel model)
            where TModel : IViewModel
        {
            using (var client = GetClient())
            {
                var modelType = typeof(TModel);
                var key = this.GenKey(modelType, model.Identity);
                var modelKey = this.GenIdentityKey(modelType, model.DisplayIdentity);
                client.Remove(modelKey);
                client.Remove(key);
            }
        }

        private RedisClient GetClient()
        {
            return new RedisClient("127.0.0.1", 6379);
        }

        private IModelProvider<TModel> GetProvider<TModel>() where TModel : IViewModel
        {
            var provider = this.container.ResolveAll<IModelProvider>().SingleOrDefault(p => typeof(IModelProvider<TModel>).IsAssignableFrom(p.GetType()) && p.CanProvider(typeof(TModel)));
            if (provider != null)
            {
                var pv = provider as ModelProvider;
                pv.Init(this.container.Resolve<IDataContextFactory>());
                return provider as IModelProvider<TModel>;
            }
            return null;
        }

        private IRelationshipProvider<TModelA, TModelB> GetRelationshipProvider<TModelA, TModelB>()
            where TModelA : IViewModel
            where TModelB : IViewModel
        {
            var provider = this.container.ResolveAll<IModelProvider>().SingleOrDefault(p => (typeof(IRelationshipProvider<TModelA, TModelB>).IsAssignableFrom(p.GetType())) && p.CanProvider(typeof(TModelA)) && p.CanProvider(typeof(TModelB)));
            if (provider != null)
            {
                var pv = provider as ModelProvider;
                pv.Init(this.container.Resolve<IDataContextFactory>());
                return provider as IRelationshipProvider<TModelA, TModelB>;
            }
            return null;
        }

        #endregion
    }
}
