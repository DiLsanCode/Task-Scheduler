using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskList.Data.Models;

namespace TaskApp.Business.dto
{
    public class dtoComment
    {
        public string Text { get; set; }
        public int? TaskId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
