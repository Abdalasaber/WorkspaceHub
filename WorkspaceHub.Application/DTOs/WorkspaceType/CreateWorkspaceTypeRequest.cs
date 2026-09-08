namespace WorkspaceHub.Application.DTOs.WorkspaceType
{
    public class CreateWorkspaceTypeRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Icon { get; set; }

    }
}
