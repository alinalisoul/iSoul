using System;
using System.Collections.Generic;

namespace iSoul.Model
{
    public class ViewModel : IViewModel
    {
        #region IViewModel 成员

        public Guid Identity { get; set; }

        public int DisplayIdentity { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public bool IsDeleted { get; set; }

        #endregion

        internal IModelService ModelService { get; set; }

        protected ICollection<TModel> Load<TModel>() where TModel : IViewModel
        {
            if (this.ModelService != null)
            {
                return this.ModelService.GetAll<TModel>();
            }
            return new List<TModel>();
        }

        protected TModel Load<TModel>(Guid id) where TModel : IViewModel
        {
            if (this.ModelService != null)
            {
                return this.ModelService.Get<TModel>(id);
            }
            return default(TModel);
        }

        protected IEnumerable<TMany> LoadMany<TOne, TMany>(Guid id)
            where TOne : IViewModel
            where TMany : IViewModel
        {
            if (this.ModelService != null)
            {
                return this.ModelService.GetMany<TOne, TMany>(id);
            }
            return new List<TMany>();
        }

        protected void AddMany<TTarget, TModel>(TModel many)
            where TTarget : IViewModel
            where TModel : IViewModel
        {
            this.ModelService.Add<TTarget, TModel>(this.Identity, many);
        }

        protected void RemoveMany<TTarget, TModel>(TModel model)
            where TTarget : IViewModel
            where TModel : IViewModel
        {
            this.ModelService.Remove<TTarget, TModel>(this.Identity, model);
        }
    }
}
