namespace HSEBank.FinancialAccounting.Interfaces.Repositories;
public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T GetById(Guid id);
    void Add(T entity);
    void Update(T entity);
    bool Delete(Guid id);
}