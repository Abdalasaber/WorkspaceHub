namespace WorkspaceHub.Application.DTOs.Workspace
{
    public class WorkspaceResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; }

        public string? Description { get; set; }

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }

        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int WorkspaceTypeId { get; set; }
        public string WorkspaceType { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

    }
}
