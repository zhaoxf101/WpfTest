using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Record
    { 
        public int Id { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value;  }
        }
        

        public Record Clone()
        {
            return (Record)MemberwiseClone();
        }
    }

}
