using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPlusSQL.Domain.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
    }
}
