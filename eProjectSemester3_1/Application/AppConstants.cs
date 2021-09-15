using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application
{
    public class AppConstants
    {
        public const int SaltSize = 24;
        public const string AdminRoleName = "Admin";
        public const string ShoppingRoleName = "Shopping";

        public const string AdminUserName = "admin";


        // View Bag / Temp Data Constants
        public const string MessageViewBagName = "Message";
        public const string GlobalClass = "GlobalClass";
        public const string CurrentAction = "CurrentAction";
        public const string CurrentController = "CurrentController";

        // Paging
        public const int AdminPageSize = 20;
        public const int ShopPageSize = 20;
        public const int NewsPageSize = 20;
    }
}