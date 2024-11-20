using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Seat
{
    public class SeatResponseDto
    {
        public int ID { get; set; }                
        public string SeatNumber { get; set; }     
        public string SeatType { get; set; }      
        public bool isAvailable { get; set; }           
        public decimal Price { get; set; }        
        public string RoomID { get; set; }        
        public string ShowtimeID { get; set; }    
    }
}
