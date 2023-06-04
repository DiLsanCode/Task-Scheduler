using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList.Data.Models;

namespace TaskApp.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? AssignmentId { get; set; }
        public int UserId { get; set; }
        public Assignment? Assignment { get; set; }
        public User User { get; set; } = null!;

    }
}
