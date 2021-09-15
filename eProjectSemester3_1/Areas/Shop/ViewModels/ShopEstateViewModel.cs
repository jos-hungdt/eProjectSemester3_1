using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Shop.ViewModels
{
    public class ShopEstateViewModel
    {
        public List<Estate> ListEstate { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class ShopEstateEditViewModel
    {
        public int Id { get; set; }
        
        public string realEstateTitle { get; set; }
        
        public string City { get; set; }
        
        public string Street { get; set; }

        public int? NoOfBedrooms { get; set; }

        public int? NoOfBathrooms { get; set; }
        
        public string GardenArea { get; set; }
        
        public string Orientation { get; set; }

        public bool? ExtraFacilitiesAvailable { get; set; }
        
        public string ModesOfTransport { get; set; }

        public int? WithFurniture { get; set; }
        
        public string ModeOfPayment { get; set; }
        
        public string Deposit { get; set; }

        public bool? Negotiable { get; set; }
        
        public string Description { get; set; }
        
        public string realEstateImage { get; set; }

        public int? realEstateStatus { get; set; }

        public byte? EstateStatus { get; set; }
        
        public decimal? Price { get; set; }

        public decimal? Area { get; set; }
        
        public string realEstateImage2 { get; set; }
        
        public string realEstateImage3 { get; set; }
        
        public string realEstateImage4 { get; set; }
        
        public string realEstateImage5 { get; set; }
        
        public string Teaser { get; set; }

        public int PostEstateType { get; set; }
        public List<SelectListItem> AllPostEstateType { get; set; }

        public int EstateStyle { get; set; }
        public List<SelectListItem> AllEstateStyle { get; set; }

        public int EstateType { get; set; }
        public List<SelectListItem> AllEstateType { get; set; }

    }
}
