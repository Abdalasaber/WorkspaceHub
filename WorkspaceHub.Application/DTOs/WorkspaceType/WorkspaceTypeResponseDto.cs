namespace WorkspaceHub.Application.DTOs.WorkspaceType
{
    public class WorkspaceTypeResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public bool IsActive { get; set; }

        public string TenantName { get; set; }
        public int TenantId { get; set; }

    }
}
