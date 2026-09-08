using System;

namespace GSEvent.Models;

public class Category
{
    public Guid CategoryId {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}

    public Category()
    {
        CategoryId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
