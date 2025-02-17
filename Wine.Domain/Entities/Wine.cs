using Wine.Domain.Contracts;

namespace Wine.Domain.Entities;

public class Wine : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string Brand { get; set; }
    public string Type { get; set; }
}