﻿using PM.Domain;
using PM.DomainServices.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IStatusServices : IRepository<Status>
    {
    }
}