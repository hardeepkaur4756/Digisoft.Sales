using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories;
using Digisoft.Sales.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Digisoft.Sales.Utility.Enums;

namespace Digisoft.Sales.Service
{
    public class BillingService : IBillingService
    {
        private readonly BillingRepository _billingRepository;

        public BillingService()
        {
            _billingRepository = new BillingRepository();
        }

        public void Delete(int id)
        {
            _billingRepository.Delete(id);
        }

        public IEnumerable<Billing> GetAll()
        {
            return _billingRepository.GetAll();
        }
        public IEnumerable<BillingViewModel> GetBiddersTotalBillings(DateTime? date)
        {
            return _billingRepository.GetBiddersTotalBillings(date);
        }
        public IEnumerable<Billing> GetAll(DateTime? startDate, DateTime? endDate, List<string> clientIds)
        {
            if (clientIds.Count > 0)
            {
                return _billingRepository.GetAll(startDate, endDate,clientIds);
            }
            else
            {
                return _billingRepository.GetAll(startDate, endDate);
            }
        }

        public IEnumerable<Billing> GetAllAfterSearch(DataTablesParam param, DateTime? startDate, DateTime? endDate, List<string> clientIds)
        {
           return _billingRepository.GetAllAfterSearch(param, startDate,endDate, clientIds);
        }
        /// <summary>
        /// get billing by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Billing GetByID(int id)
        {
            return _billingRepository.GetbyId(id);
        }

        /// <summary>
        /// insert or update billings
        /// </summary>
        /// <param name="billingVM"></param>
        /// <returns></returns>
        public Billing InsertUpdate(BillingViewModel billingVM)
        {
            try
            {
                if (billingVM != null)
                {
                    // Get  exitting billing by Id
                    Billing billing = _billingRepository.GetbyId(billingVM.Id);

                    if (billingVM.Id > 0)
                    {
                        billingVM.CreatedOn = billing.CreatedOn;
                        billingVM.UserId = billing.UserId;
                        AutoMapper.Mapper.Map(billingVM, billing);

                        if (billingVM.ProjectTypeId == (int)ProjectTypeEnum.ProjectType.FixPrice)
                            billing.Hours = default;
                        else
                            billing.Amount = default;

                        _billingRepository.Update(billing);
                        return billing;
                    }
                    else
                    {  // insert billing 
                        billingVM.CreatedOn = DateTime.Now;
                        Billing billingEntity = new Billing();
                        AutoMapper.Mapper.Map(billingVM, billingEntity);
                        _billingRepository.Insert(billingEntity);
                        return billingEntity;
                    }
                }
                else
                {
                    throw new Exception("billingVM is null.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public BillingViewModel GetByIDVM(int id)
        {
            Billing billing = _billingRepository.GetbyId(id);
            BillingViewModel billingVM = new BillingViewModel();
            AutoMapper.Mapper.Map(billing, billingVM);
            return billingVM;
        }

        public int GetBillingCount(ControllerTypeEnum.ControllerType checkFor, int id)
        {
            return _billingRepository.GetBillingCount(checkFor, id);
        }
    }
}