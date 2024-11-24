﻿using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    public class RoleInProjectServices(ApplicationDbContext _context) : Repository<RoleInProject>(_context), IRoleInProjectServices
    {
    }
}
