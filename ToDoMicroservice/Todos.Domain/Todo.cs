namespace Todos.Domain;

public class Todo
{
    public int TodoId { get; private set;  }
    
    public string Name { get; private set; }
    
    public bool IsDone { get; private set; }
    
    public DateTime CreatedDate { get; private set;  }
    
    public DateTime? UpdatedDate { get; private set; }
    
    public Guid OwnerId { get; private set; }
    public Owner Owner { get; private set; }

    private Todo(){}
    
    public Todo(string name, Guid ownerId, DateTime createdDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is empty", nameof(name));
        }

        if (name.Length > 200)
        {
            throw new ArgumentException("Name length more then 50", nameof(name));
        }
        Name = name;
        OwnerId = ownerId;
        CreatedDate = createdDate;
    }
    

    public void UpdateName(string name, DateTime updatedDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is empty", nameof(name));
        }

        if (name.Length > 50)
        {
            throw new ArgumentException("Name length more then 50", nameof(name));
        }

        Name = name;
        SetUpdateDate(updatedDate);
    }

    public void UpdateIsDone(bool isDone, DateTime updatedDate)
    {
        IsDone = isDone;
        SetUpdateDate(updatedDate);
    }

    private void SetUpdateDate(DateTime updatedDate)
    {
        if (updatedDate < UpdatedDate)
        {
            throw new ArgumentException("Incorrect update date", nameof(updatedDate));
        }
        UpdatedDate = updatedDate;
    }
}