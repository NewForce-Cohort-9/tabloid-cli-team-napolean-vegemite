using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabloidCLI.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TextContext { get; set; }
        public DateTime CreationDateTime { get; set; }
        public Post Post { get; set; }


    }
}
