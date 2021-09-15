using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class BankLoan : Entity
    {
        [Key]
        public int bankLoanId { get; set; }

        public int? bankID { get; set; }

        [StringLength(100)]
        public string bankLoanName { get; set; }

        public double? Minimum { get; set; }

        public double? Maximum { get; set; }

        [StringLength(500)]
        public string loanDescription { get; set; }

        public int? durationOfLoan { get; set; }

        public int? bankLoanStatus { get; set; }

        public virtual Banks Banks { get; set; }
    }
}