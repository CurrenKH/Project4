using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMovieManager_CurrenandMatthew
{
    class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Type { get; set; }
        public string ImagePath { get; set; }

        public Member()
        {
            Id = 0;
            Name = "";
            DOB = new DateTime();
            Type = 0;
            ImagePath = "";
        }
    }
}
