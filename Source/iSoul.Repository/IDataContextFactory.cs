
namespace iSoul.Repository
{
    public interface IDataContextFactory
    {
        IDataContext Create(string nameOrConnectionString, string configNameSpace);
    }
}
