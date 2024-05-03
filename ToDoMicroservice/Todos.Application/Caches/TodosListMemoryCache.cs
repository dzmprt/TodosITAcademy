using Todos.Application.BaseRealizations;
using Todos.Application.DTOs;

namespace Todos.Application.Caches;

public class TodosListMemoryCache : BaseCache<BaseListDto<GetTodoDto>>;