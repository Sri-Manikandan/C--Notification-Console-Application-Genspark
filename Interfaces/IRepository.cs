namespace Sns.Interfaces{
    internal interface IRepository<T, K> where T : class{

        T Add(T entity);

        List<T> GetAll();
        T? Get(K key);

        T? Update(K key, T entity);
        
        T? Delete(K key);
    }
}
