using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class Contact : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
    }
}