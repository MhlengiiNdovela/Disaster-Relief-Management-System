using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Service.Data;

namespace GiftOfTheGivers.Tests;

public class SqliteSchemaTests
{
    [Fact]
    public void Free_database_schema_can_be_created()
    {
        var path = Path.Combine(Path.GetTempPath(), $"gift-test-{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<GiftDbContext>()
            .UseSqlite($"Data Source={path}")
            .Options;

        try
        {
            using var db = new GiftDbContext(options);
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
            Assert.Equal(0, db.Users.Count());
            Assert.Equal(0, db.Donations.Count());
            Assert.Equal(0, db.Volunteers.Count());
            Assert.Equal(0, db.ReliefProjects.Count());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
