using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using eProjectSemester3_1.Application.Entities;

namespace eProjectSemester3_1.ViewModels
{
    public class EstateViewModel
    {
        [Key]
        public int RealEstateID { get; set; }

        [Required]
        public string realEstateTitle { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(500)]
        public string Street { get; set; }

        public int? NoOfBedrooms { get; set; }

        public int? NoOfBathrooms { get; set; }

        [StringLength(100)]
        public string GardenArea { get; set; }

        [StringLength(100)]
        public string Orientation { get; set; }

        public bool? ExtraFacilitiesAvailable { get; set; }

        [StringLength(100)]
        public string ModesOfTransport { get; set; }

        public int? WithFurniture { get; set; }

        [StringLength(100)]
        public string ModeOfPayment { get; set; }

        [StringLength(100)]
        public string Deposit { get; set; }

        public bool? Negotiable { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [StringLength(200)]
        public string realEstateImage { get; set; }

        public int? realEstateStatus { get; set; }

        public byte? EstateStatus { get; set; }

        [Column(TypeName = "money")]
        public decimal? Price { get; set; }

        public decimal? Area { get; set; }

        [StringLength(200)]
        public string realEstateImage2 { get; set; }

        [StringLength(200)]
        public string realEstateImage3 { get; set; }

        [StringLength(200)]
        public string realEstateImage4 { get; set; }

        [StringLength(200)]
        public string realEstateImage5 { get; set; }

        [StringLength(100)]
        public string Teaser { get; set; }

        public virtual PostEstateType PostType { get; set; }

        public virtual EstateStyle EstateStyle { get; set; }

        public virtual EstateType EstateType { get; set; }

        public virtual MembershipUser User { get; set; }
    }
}