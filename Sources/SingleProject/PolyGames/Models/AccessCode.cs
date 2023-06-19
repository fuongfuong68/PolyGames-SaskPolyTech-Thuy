using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolyGames.Models
{
    public class AccessCode
    {
        public string Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsExpired { get; set; }
    }
}