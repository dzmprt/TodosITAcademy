namespace Todos.Application.Caches;

internal interface ICleanTodosCacheService
{
    void ClearAllCaches();
    void ClearListCaches();
}