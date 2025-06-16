using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPlusSQL.Infrastructure.Configuration
{
    public class DatabaseSettings
    {
        public string Provider { get; set; }
        public int CommandTimeout { get; set; }
    }
}
