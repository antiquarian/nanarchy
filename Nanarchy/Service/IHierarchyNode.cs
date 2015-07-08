namespace Nanarchy.Service
{
    public interface IHierarchyNode
    {
        int Id { get; set; }
        int LeftId { get; set; }
        int RightId { get; set; } 
        int TargetId { get; set; }
    }
}