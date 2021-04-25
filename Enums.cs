using System;

namespace DotNetNuke.Modules.Links.Enums
{
    public enum ModuleContentTypes
    {
        Links = 1,
        Menu = 2,
        Folder = 3,
        [Obsolete("Friends mode has been removed in v8.0.0. This enum value will be deleted as soon as we have a 5 value in the enum.")]
        Friends = 4,
    }

    public enum LinkView
    {
        Horizontal = 1,
        Vertical = 2
    }

    public enum DisplayAttribute
    {
        Username = 1,
        DisplayName = 2,
        FirstName = 3,
        LastName = 4,
        FullName = 5
    }

    public enum DisplayOrder
    {
        ASC = 1,
        DESC = 2
    }
}
