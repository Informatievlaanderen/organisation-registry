namespace OrganisationRegistry.SqlServer
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public static class DatabaseFacadeExtensions
    {
        public static async Task DeleteAllRows(this DatabaseFacade source, string schema, params string[] tableNames)
        {
            await source.ExecuteSqlRawAsync(
                string.Concat(
                    tableNames.Select(tableName => $"DELETE FROM [{schema}].[{tableName}];")));
        }
    }
}
