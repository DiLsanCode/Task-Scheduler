using System;
using System.Collections.Generic;
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
        public int? TaskId { get; set; }
        public int UserId { get; set; }
        public virtual Assignment Task { get; set; }
        public virtual User User { get; set; }

    }
}
