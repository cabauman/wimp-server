using Microsoft.EntityFrameworkCore;

namespace WIMP_Server.Data;

public class WimpDbContextDev : WimpDbContext
{
    public WimpDbContextDev(DbContextOptions options) : base(options)
    {
    }
}
