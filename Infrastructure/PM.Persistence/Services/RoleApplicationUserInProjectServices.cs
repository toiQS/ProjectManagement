using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    public class RoleApplicationUserInProjectServices
        (ApplicationDbContext _context) : Repository<RoleApplicationUserInProject>(_context), IRoleApplicationUserInProjectServices
    {
    }
}
