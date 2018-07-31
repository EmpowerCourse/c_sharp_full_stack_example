namespace Armoire.Common
{
    public enum TypeOfUserRole
    {
        [Descriptor(AppConstants.DESCRIPTOR_Long, "User can maintain all application reference data")]
        Administrator = 1,
        [Descriptor(AppConstants.DESCRIPTOR_Long, "User can view but not access or modify application reference data")]
        Viewer = 2
    }
}