﻿using Digisoft.Sales.Helper;
using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories;
using Digisoft.Sales.Service;
using Digisoft.Sales.Utility.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        #region properties

        private SalesEntities _context;
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private readonly ClientService _clientService;
        private readonly BillingService _billingService;

        #endregion

        public ClientController()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            _context = new SalesEntities();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _clientService = new ClientService();
            _billingService = new BillingService();
        }

        // GET: Client
        public ActionResult Index()
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //var clients = _clientService.GetAll().ToList();
            //var clientsVm = _clientService.GetAll().Select(x => new ClientViewModel
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    EmailAdress = x.EmailAdress,
            //    Skype = x.Skype,
            //    CreatedByName = x.AspNetUser.UserName,
            //    IsCurrentUser = user.Id==x.CreatedBy ? true : false
            //}).ToList();
            ////AutoMapper.Mapper.Map(clients, clientsVm);
            return View();
        }

        [HttpGet]
        public JsonResult GetList(DataTablesParam param, string sortDir, string sortCol, DateTime? startDate, DateTime? endDate)
        {
            var clientsVm = new List<ClientViewModel>();
            int pageNo = 1;
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (param.iDisplayStart >= param.iDisplayLength)
                pageNo = (param.iDisplayStart / param.iDisplayLength) + 1;

            int totalCount = 0;

            if (param.sSearch != null)
            {
                sortCol = sortCol == "CreatedByName" ? "CreatedBy" : sortCol;
                clientsVm = _clientService.GetAllAfterSearch(param,startDate,endDate)
                .OrderBy(x => x.Id).OrderBy(sortCol + " " + sortDir).Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                .Select(x => new ClientViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    EmailAdress = x.EmailAdress,
                    Skype = x.Skype,
                    //CreatedByName = x.AspNetUser.UserName,
                    CreatedByName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                    CreatedOn =x.CreatedOn,
                    IsCurrentUser = user.Id == x.CreatedBy ? true : false
                })
                .ToList();
                totalCount = clientsVm.Count();
            }
            else
            {
                sortCol = sortCol == "CreatedByName" ? "CreatedBy" : sortCol;
                totalCount = _clientService.GetAll(startDate, endDate).Count();
                clientsVm = _clientService.GetAll(startDate, endDate).OrderBy(x => x.Id).OrderBy(sortCol + " " + sortDir)
                    .Skip((pageNo - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                     .Select(x => new ClientViewModel
                     {
                         Id = x.Id,
                         Name = x.Name,
                         EmailAdress = x.EmailAdress,
                         Skype = x.Skype,
                         //CreatedByName = x.AspNetUser.UserName,
                         CreatedByName = x.AspNetUser?.UserDetails?.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName))),
                         CreatedOn = x.CreatedOn,
                         IsCurrentUser = user.Id == x.CreatedBy ? true : false
                     })
                .ToList();
            }

            //Comment due to performance issue
            //AutoMapper.Mapper.Map(jobs, clientsVm);

            return Json(new
            {
                aaData = clientsVm,
                sEcho = param.sEcho,
                iTotalDisplayRecords = totalCount,
                iTotalRecords = totalCount
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddClient(int id,string viewType)
        {
            ClientViewModel vm = id > 0 ? _clientService.GetByIDVM(id) : new ClientViewModel();

            if (viewType == "Display")
            {
                vm.ViewType = "Display";
            }
            
            return Json(new { Success = true, Html = this.RenderPartialViewToString("_AddEditClient", vm) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Insert update Client
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertUpdate(ClientViewModel vm)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (vm.Id < 1)
            {
                vm.CreatedBy = user.Id;
            }
            
            _clientService.InsertUpdate(vm);
            if (vm.Id > 0)
            {
                return Json(new { Message = "Client updated successfully!", Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "Client inserted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// delete client
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id)
        {
            if (User.IsInRole("Admin"))
            {
                _clientService.Delete(Id);
                return Json(new { Message = "Client deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Check billing first
                var billingCount = _billingService.GetBillingCount(ControllerTypeEnum.ControllerType.Client, Id);
                if (billingCount <= 0)
                {
                    _clientService.Delete(Id);
                    return Json(new { Message = "Client deleted successfully!", Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Sorry! You can't delete this record!", Success = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}