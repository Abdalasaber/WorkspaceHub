using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkspaceHub.Infrastructure.Extensions;

public static class IndexBuilderExtensions
{
    public static IndexBuilder HasSoftDeleteUniqueFilter(this IndexBuilder indexBuilder)
    {
        return indexBuilder
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}