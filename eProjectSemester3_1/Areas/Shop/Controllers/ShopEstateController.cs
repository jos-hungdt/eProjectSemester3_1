using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using eProjectSemester3_1.Areas.Shop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace eProjectSemester3_1.Areas.Shop.Controllers
{
    [Authorize(Roles = AppConstants.ShoppingRoleName)]
    public class ShopEstateController : BaseShopController
    {
        public readonly EstateService _estateService;
        public readonly EstateStyleService _estateStyleService;
        public readonly EstateTypeService _estateTypeService;
        public readonly PostEstateTypeService _postEstateTypeService;
        public Application.Context.AppDbContext _context = new Application.Context.AppDbContext();
        public ShopEstateController(PostEstateTypeService postEstateTypeService,EstateTypeService estateTypeService,EstateStyleService estateStyleService,EstateService estateService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
         
            _estateService = estateService;
            _estateStyleService = estateStyleService;
            _estateTypeService = estateTypeService;
            _postEstateTypeService = postEstateTypeService;
        }
        // GET: Shop/ShopEstate
        public ActionResult Index(int? p)
        {
            int count = _estateService.EstateCount(LoginUser);
            var model = new ShopEstateViewModel();
            var Paging = CalcPaging(AppConstants.AdminPageSize, p, count);
            if (IsRole(AppConstants.AdminRoleName))
            {
                 model = new ShopEstateViewModel
                {
                    Paging = Paging,
                     ListEstate = _estateService.GetList(Paging.Page, AppConstants.ShopPageSize)

                 };
            }
            else
            {
                model = new ShopEstateViewModel
                {
                    Paging = Paging,
                    ListEstate = _estateService.GetList(LoginUser, Paging.Page, AppConstants.ShopPageSize)
                   // ListEstate = _estateService.GetList(LoginUser, Paging.Page, AppConstants.ShopPageSize);

                };
            }
            return View(model);
        }
        #region Delete
        public ActionResult Delete(int? id,int? p)
        {
            if (id == null) return RedirectToAction("Index",new { p = p });
            var estate = _estateService.Get((int)id);
            if ((estate == null))
            {
                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                {
                    Message = "estate not found",
                    MessageType = GenericMessages.warning
                };
                return RedirectToAction("Index", new { p = p });
            }
            if ((estate.User.Id == this.LoginUser.Id) || (IsRole(AppConstants.AdminRoleName) == true) )
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        _estateService.Remove(estate);
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Estate.StartsWith);

                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Delete estate successfully",
                            MessageType = GenericMessages.success
                        };

                        //return RedirectToAction("Index", new { p = p });
                    }catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Delete estate Error",
                            MessageType = GenericMessages.warning
                        };
                    }
                
                }

            }
            return RedirectToAction("Index",new { p = p });
        }
        #endregion

        #region Private
        private ShopEstateEditViewModel AdditionalEstateViewModel(ShopEstateEditViewModel viewModel)
        {
            viewModel.AllEstateStyle = _estateStyleService.GetBaseSelectListEstateStyle(_estateStyleService.GetAll());
            viewModel.AllEstateType = _estateTypeService.GetBaseSelectListEstateType(_estateTypeService.GetAll());
            viewModel.AllPostEstateType = _postEstateTypeService.GetBaseSelectListPostEstateType(_postEstateTypeService.GetAll());
            
            return viewModel;
        }
        #endregion

        #region Create Estate
        [Authorize]
        public ActionResult Create()
        {

            var viewModel = new ShopEstateEditViewModel
            {

            };

            viewModel = AdditionalEstateViewModel(viewModel);

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShopEstateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var estate = new Estate
                        {
                            realEstateTitle = viewModel.realEstateTitle,
                            City = viewModel.City,
                            Street = viewModel.Street,
                            NoOfBedrooms = viewModel.NoOfBedrooms,
                            NoOfBathrooms = viewModel.NoOfBathrooms,
                            GardenArea = viewModel.GardenArea,
                            Orientation = viewModel.Orientation,
                            ExtraFacilitiesAvailable = viewModel.ExtraFacilitiesAvailable,
                            ModesOfTransport = viewModel.ModesOfTransport,
                            WithFurniture = viewModel.WithFurniture,
                            ModeOfPayment = viewModel.ModeOfPayment,
                            Deposit = viewModel.Deposit,
                            Negotiable = viewModel.Negotiable,
                            Description = viewModel.Description,
                            realEstateImage = viewModel.realEstateImage,
                            realEstateStatus = viewModel.realEstateStatus,
                            EstateStatus = viewModel.EstateStatus,
                            Price = viewModel.Price,
                            Area = viewModel.Area,
                            realEstateImage2 = viewModel.realEstateImage2,
                            realEstateImage3 = viewModel.realEstateImage3,
                            realEstateImage4 = viewModel.realEstateImage4,
                            realEstateImage5 = viewModel.realEstateImage5,
                            Teaser = viewModel.Teaser,
                            PostType = _postEstateTypeService.Get(viewModel.PostEstateType),
                            EstateStyle = _estateStyleService.Get(viewModel.EstateStyle),
                            EstateType = _estateTypeService.Get(viewModel.EstateType),
                            User = LoginUser
                        };

                        _estateService.Add(estate);

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Estate.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Create estate success",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Create estate Error");
                    }
                }
            }

            viewModel = AdditionalEstateViewModel(viewModel);
            return View(viewModel);
        }

        #endregion

        #region Edit Estate
        public ActionResult Edit(int id)
        {
            var estate = _estateService.Get(id);
            if (estate == null) return RedirectToAction("Index");

            var viewModel = new ShopEstateEditViewModel
            {
                realEstateTitle = estate.realEstateTitle,
                City = estate.City,
                Street = estate.Street,
                NoOfBedrooms = estate.NoOfBedrooms,
                NoOfBathrooms = estate.NoOfBathrooms,
                GardenArea = estate.GardenArea,
                Orientation = estate.Orientation,
                ExtraFacilitiesAvailable = estate.ExtraFacilitiesAvailable,
                ModesOfTransport = estate.ModesOfTransport,
                WithFurniture = estate.WithFurniture,
                ModeOfPayment = estate.ModeOfPayment,
                Deposit = estate.Deposit,
                Negotiable = estate.Negotiable,
                Description = estate.Description,
                realEstateImage = estate.realEstateImage,
                realEstateStatus = estate.realEstateStatus,
                EstateStatus = estate.EstateStatus,
                Price = estate.Price,
                Area = estate.Area,
                realEstateImage2 = estate.realEstateImage2,
                realEstateImage3 = estate.realEstateImage3,
                realEstateImage4 = estate.realEstateImage4,
                realEstateImage5 = estate.realEstateImage5,
                Teaser = estate.Teaser,
                PostEstateType = estate.PostType.postTypeID,
                EstateStyle = estate.EstateStyle.realEstateStyleID,
                EstateType = estate.EstateType.realStateTypeID,
            };

            viewModel = AdditionalEstateViewModel(viewModel);
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShopEstateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {

                    try
                    {
                        var estate = _estateService.Get(viewModel.Id, false);
                        if (estate == null) return HttpNotFound();// RedirectToAction("Index");
                        if ((estate.User.Id == this.LoginUser.Id) || (IsRole(AppConstants.AdminRoleName) == true))
                        {
                            estate.realEstateTitle = viewModel.realEstateTitle;
                            estate.City = viewModel.City;
                            estate.Street = viewModel.Street;
                            estate.NoOfBedrooms = viewModel.NoOfBedrooms;
                            estate.NoOfBathrooms = viewModel.NoOfBathrooms;
                            estate.GardenArea = viewModel.GardenArea;
                            estate.Orientation = viewModel.Orientation;
                            estate.ExtraFacilitiesAvailable = viewModel.ExtraFacilitiesAvailable;
                            estate.ModesOfTransport = viewModel.ModesOfTransport;
                            estate.WithFurniture = viewModel.WithFurniture;
                            estate.ModeOfPayment = viewModel.ModeOfPayment;
                            estate.Deposit = viewModel.Deposit;
                            estate.Negotiable = viewModel.Negotiable;
                            estate.Description = viewModel.Description;
                            estate.realEstateImage = viewModel.realEstateImage;
                            estate.realEstateStatus = viewModel.realEstateStatus;
                            estate.EstateStatus = viewModel.EstateStatus;
                            estate.Price = viewModel.Price;
                            estate.Area = viewModel.Area;
                            estate.realEstateImage2 = viewModel.realEstateImage2;
                            estate.realEstateImage3 = viewModel.realEstateImage3;
                            estate.realEstateImage4 = viewModel.realEstateImage4;
                            estate.realEstateImage5 = viewModel.realEstateImage5;
                            estate.Teaser = viewModel.Teaser;
                            estate.PostType = _postEstateTypeService.Get(viewModel.PostEstateType);
                            estate.EstateStyle = _estateStyleService.Get(viewModel.EstateStyle);
                            estate.EstateType = _estateTypeService.Get(viewModel.EstateType);
                            estate.User = LoginUser;
                        }
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Estate.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Update estate successfully",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Edit estate Error");
                    }
                }
            }

            viewModel = AdditionalEstateViewModel(viewModel);
            return View(viewModel);
        }
        #endregion

    }
}