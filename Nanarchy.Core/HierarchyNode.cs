namespace Nanarchy.Core
{
    public class HierarchyNode
    {
        public int Id { get; set; }
        public int LeftId { get; set; }
        public int RightId { get; set; }
        public int TargetId { get; set; }
    }
}