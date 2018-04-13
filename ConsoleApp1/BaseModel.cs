using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketingPlatform.Client.Model
{
    public class BaseModel<T>
    {
        public bool IsSuccess
        {
            get
            {
                return Code == 0;
            }
        }

        public int Code { get; set; }

        public string Msg { get; set; }

        public T Result { get; set; }
    }
}
