namespace WorkspaceHub.Application.DTOs.Workspace
{
    public class CreateWorkspaceRequest
    {
        public string Name { get; set; } = null!;

        public string Code { get; set; }

        public string? Description { get; set; }

        public int Capacity { get; set; }

        public int BranchId { get; set; }

        public int WorkspaceTypeId { get; set; }

    }
}
