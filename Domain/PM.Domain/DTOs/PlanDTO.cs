using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.DTOs
{
    [Table(name:"Plan")]
    public class PlanDTO
    {
        [Key]
        public string Id { get; set; }
        [Column(name:"Plan Name")]
        public string PlanName { get; set; } = string.Empty;
        [Column(name: "Create At")]
        public DateTime CreateAt { get; set; }
        [Column(name: "Start At")]
        public DateTime StartAt { get; set; }
        [Column(name:"End At")]
        public DateTime EndAt { get; set; }
    }
}
