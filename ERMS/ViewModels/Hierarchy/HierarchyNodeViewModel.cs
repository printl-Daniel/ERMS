// ViewModels/Hierarchy/HierarchyNodeViewModel.cs
namespace ERMS.ViewModels.Hierarchy
{
    public class HierarchyNodeViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PositionTitle { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicturePath { get; set; }
        public int SubordinateCount { get; set; }
        public int Level { get; set; }
        public List<HierarchyNodeViewModel> Subordinates { get; set; } = new List<HierarchyNodeViewModel>();

        // Display helpers
        public bool HasSubordinates => SubordinateCount > 0;
        public bool HasPhoneNumber => !string.IsNullOrEmpty(PhoneNumber);
        public string DisplayProfilePicture => ProfilePicturePath ?? "/images/default-avatar.png";
    }
}