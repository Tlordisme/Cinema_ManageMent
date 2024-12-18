﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Seat
{
    public class CMSeatPrice
    {
        public int Id { get; set; }

        public string SeatType { get; set; }
        public string RoomID { get; set; } 

        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 

        public decimal Price { get; set; } 
    }
}
