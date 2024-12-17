using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Constant.Permission
{
    public static class PermissionKey
    {

        //User
        public const string CreateUser = "create_user";
        public const string UpdateUser = "update_user";
        public const string DeleteUser = "delete_user";
        public const string ViewUser = "view_users";
        public const string ViewAllUser = "view_all_users";

        //comment
        public const string AddComment = "AddComment";
        public const string DeleteComment = "DeleteComment";

        //food
        public const string AddFood = "AddFood";
        public const string UpdateFood = "UpdateFood";
        public const string DeleteFood = "DeleteFood";
        public const string ViewFood = "ViewFood";
        public const string GetAllFoods = "GetAllFoods";

        //movie
        public const string AddMovie = "AddMovie";
        public const string UpdateMovie = "UpdateMovie";
        public const string DeleteMovie = "DeleteMovie";
        public const string ViewMovies = "ViewMovies";

        //payment
        public const string CreatePayment = "CreatePayment";
        public const string ViewPayment = "ViewPayment";

        //revenue
        public const string ViewRevenue = "ViewRevenue";

        //Role
        public const string GetAllRoles = "GetAllRoles";
        public const string GetRoleById = "GetRoleById";
        public const string CreateRole = "CreateRole";
        public const string UpdateRole = "UpdateRole";
        public const string DeleteRole = "DeleteRole";
        public const string AddRoleToUser = "AddRoleToUser";
        //public const string GetUserRoles = "get_user_roles";

        //room
        public const string AddRoom = "AddRoom";
        public const string UpdateRoom = "UpdateRoom";
        public const string DeleteRoom = "DeleteRoom";
        public const string ViewRoomsByTheater = "ViewRoomsByTheater";

        //seat
        public const string AddSeat = "AddSeat";
        public const string UpdateSeat = "UpdateSeat";
        public const string DeleteSeat = "DeleteSeat";
        public const string ViewSeatsByRoom = "ViewSeatsByRoom";
        public const string LinkDoubleSeat = "LinkDoubleSeat";

        //seat-price
        public const string AddSeatPrice = "AddSeatPrice";
        public const string UpdateSeatPrice = "UpdateSeatPrice";
        public const string DeleteSeatPrice = "DeleteSeatPrice";
        public const string ViewSeatPricesByRoom = "ViewSeatPricesByRoom";
        public const string ViewSeatPriceById = "ViewSeatPriceById";

        //showtime
        public const string CreateShowtime = "CreateShowtime";
        public const string ViewAllShowtimes = "ViewAllShowtimes";
        public const string ViewShowtimeById = "ViewShowtimeById";
        public const string UpdateShowtime = "UpdateShowtime";
        public const string DeleteShowtime = "DeleteShowtime";

        //theater-chain
        public const string CreateTheaterChain = "CreateTheaterChain";
        public const string ViewAllTheaterChains = "ViewAllTheaterChains";
        public const string UpdateTheaterChain = "UpdateTheaterChain";
        public const string DeleteTheaterChain = "DeleteTheaterChain";

        //theater
        public const string CreateTheater = "CreateTheater";
        public const string ViewTheaters = "ViewTheaters";
        public const string DeleteTheater = "DeleteTheater";
        public const string UpdateTheater = "UpdateTheater";

        //ticket
        public const string BookTicketPermission = "BookTicket";
        public const string ViewTicketDetails = "ViewTicketDetails";
        public const string DeleteTicketPermission = "DeleteTicket";
        public const string ViewAllTickets = "ViewAllTickets";
        public const string ViewUserTickets = "ViewUserTickets";
    }
}