using Core.Interfaces;

namespace Core.Entities;

public class BaseEntity
{
    public BaseEntity()
    {
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
    }
    public Guid Id { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
}
