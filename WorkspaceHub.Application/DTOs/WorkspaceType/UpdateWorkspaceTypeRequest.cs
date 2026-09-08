namespace WorkspaceHub.Application.DTOs.WorkspaceType
{
    public class UpdateWorkspaceTypeRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public bool IsActive { get; set; }

    }
}
