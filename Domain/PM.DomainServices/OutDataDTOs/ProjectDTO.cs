using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.OutDataDTOs
{
    public class ProjectDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string OwnerImage { get; set; }
    }
}
