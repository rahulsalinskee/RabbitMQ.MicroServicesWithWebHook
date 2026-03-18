namespace Order.API.Repository.FilterServices.Services
{
    public interface IFilterService<T>
    {
        public IQueryable<T?> ApplyFilterOn(IQueryable<T?> queryOn, string? columnName, string? filterKeyWord);
    }
}