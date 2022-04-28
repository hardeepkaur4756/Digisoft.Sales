using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories.Interface;
using Digisoft.Sales.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Digisoft.Sales.Repositories
{

    public class BillingRepository : IRepository<Billing>
    {
        private readonly SalesEntities _context;
        public BillingRepository(SalesEntities context)
        {
            _context = context;
        }
        public BillingRepository()
        {
            _context = new SalesEntities();
        }
        public IEnumerable<Billing> GetAll()
        {
            return _context.Billings;
        }
        public IEnumerable<BillingViewModel> GetBiddersTotalBillings(DateTime? Date)
        {
            var billingsVm = new List<BillingViewModel>();
            if (Date.HasValue)
            {
                var date = (DateTime)Date;
                var billings = _context.Billings
                    .Where(x => x.BillingDate.Month == date.Month && x.BillingDate.Year == date.Year)
                    .Include(x => x.Job).ToList();
                AutoMapper.Mapper.Map(billings, billingsVm);
                return billingsVm;
            }
            else { 
            var billings= _context.Billings.Include(x => x.Job.Price);
                AutoMapper.Mapper.Map(billings, billingsVm);
                return billingsVm;
            }
        }
        public IEnumerable<Billing> GetAll(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Billings.Where(x => x.BillingDate >= startDate && x.BillingDate <= endDate);
            }
            else
            {
                return _context.Billings;
            }
        }
        public IEnumerable<Billing> GetAll(DateTime? startDate, DateTime? endDate, List<string> clientIds)
        {
            if (clientIds.Count > 0)
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    return _context.Billings.Where(x => (clientIds.Contains(x.Job.ClientId.ToString())) &&
                    (x.BillingDate >= startDate && x.BillingDate <= endDate));
                }
                else
                {
                    return _context.Billings.Where(x => (clientIds.Contains(x.Job.ClientId.ToString())));
                }
            }
            else
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    return _context.Billings.Where(x => x.BillingDate >= startDate && x.BillingDate <= endDate);
                }
                else
                {
                    return _context.Billings;
                }
            }
        }

        public IEnumerable<Billing> GetAllAfterSearch(DataTablesParam param, DateTime? startDate, DateTime? endDate, List<string> clientIds)
        {
            var sSearch = param.sSearch.ToLower();

            if (clientIds.Count > 0)
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    return _context.Billings
                          .Where(x => (clientIds.Contains(x.Job.ClientId.ToString())) &&
                           (x.BillingDate >= startDate && x.BillingDate <= endDate) &&
                            (x.Job.Client.Name.ToLower().Contains(sSearch)
                          || x.Job.Developer.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.CreatedOn.ToString().Contains(sSearch)
                          || x.Hours.ToString().Contains(sSearch)
                          || x.Amount.ToString().Contains(sSearch)
                          || x.BillingDate.ToString().Contains(sSearch))
                          );
                }
                else
                {
                    return _context.Billings
                          .Where(x => (clientIds.Contains(x.Job.ClientId.ToString())) &&
                           (x.Job.Client.Name.ToLower().Contains(sSearch)
                          || x.Job.Developer.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.CreatedOn.ToString().Contains(sSearch)
                          || x.Hours.ToString().Contains(sSearch)
                          || x.Amount.ToString().Contains(sSearch))
                          //|| x.BillingDate.ToString().Contains(sSearch)
                          );
                }
            }
            //(biddersIds.Contains(x.UserId)) &&
            else
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    return _context.Billings
                          .Where(x =>
                          (x.BillingDate >= startDate && x.BillingDate <= endDate) &&
                            (x.Job.Client.Name.ToLower().Contains(sSearch)
                          || x.Job.Developer.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.CreatedOn.ToString().Contains(sSearch)
                          || x.Hours.ToString().Contains(sSearch)
                          || x.Amount.ToString().Contains(sSearch))
                          || x.BillingDate.ToString().Contains(sSearch)
                          );
                }
                else
                {
                    return _context.Billings
                          .Where(x =>
                             x.Job.Client.Name.ToLower().Contains(sSearch)
                          || x.Job.Developer.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.CreatedOn.ToString().Contains(sSearch)
                          || x.Hours.ToString().Contains(sSearch)
                          || x.Amount.ToString().Contains(sSearch)
                          //|| x.BillingDate.ToString().Contains(sSearch)
                          );
                }
            }
        }

        public Billing GetbyId(int id)
        {
            return _context.Billings.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            Billing model = _context.Billings.FirstOrDefault(x => x.Id == id);
            _context.Billings.Remove(model);
            _context.SaveChanges();
        }

        public Billing Update(Billing Billing)
        {
            _context.SaveChanges();
            return Billing;
        }

        public Billing Insert(Billing Billing)
        {
            _context.Billings.Add(Billing);
            _context.SaveChanges();
            return Billing;
        }

        public int GetBillingCount(ControllerTypeEnum.ControllerType checkFor, int id)
        {
            ControllerTypeEnum.ControllerType controllerType = (ControllerTypeEnum.ControllerType)(checkFor);
            var count = default(int);
            switch (controllerType)
            {
                case ControllerTypeEnum.ControllerType.Bidding:
                    count = _context.Billings.Where(x => x.Job.Bidding.Id == id && (x.Amount > 0 || x.Hours > 0)).Count();
                    break;
                case ControllerTypeEnum.ControllerType.Client:
                    count = _context.Billings.Where(x => x.Job.Client.Id == id && (x.Amount > 0 || x.Hours > 0)).Count();
                    break;
                case ControllerTypeEnum.ControllerType.Job:
                    count = _context.Billings.Where(x => x.Job.Id == id && (x.Amount > 0 || x.Hours > 0)).Count();
                    break;
                    //default:
                    //    break;
            }
            return count;
        }
    }
}