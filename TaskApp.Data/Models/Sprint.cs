using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList.Data.Models;

namespace TaskApp.Data.Models
{
    public class Sprint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public ICollection<Assignment> Tasks { get; set; } = new List<Assignment>();
    }
}
