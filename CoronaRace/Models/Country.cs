using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaRace.Models
{
  
    public class Country
    {
        public string Name { get; set; }
        public Cases Cases { get; set; }
        public int Distance { get; set; }
        public int DeathDistance { get; set; }
        public string Horse { get; set; }
    }
}
