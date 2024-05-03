using Todos.Application.Abstractions.Mappings;
using Todos.Domain;

namespace Todos.Application.DTOs;

public class GetTodoDto : IMapFrom<Todo>
{
    public int TodoId { get; set; }

    public string Name { get; set; } = default!;

    public bool IsDone { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public Guid OwnerId { get; set; }
}