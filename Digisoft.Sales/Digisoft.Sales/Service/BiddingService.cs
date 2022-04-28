using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories;
using Digisoft.Sales.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Service
{
    public class BiddingService : IBiddingService
    {
        private readonly BiddingRepository _biddingRepository;
        private readonly JobRepository _jobRepository;

        public BiddingService()
        {
            _biddingRepository = new BiddingRepository();
            _jobRepository = new JobRepository();
        }

        public void Delete(int id)
        {
            _biddingRepository.Delete(id);
        }

        public IEnumerable<Bidding> GetAll()
        {
            return _biddingRepository.GetAll();
        }
        public IEnumerable<Bidding> GetAll(List<string> biddersIds)
        {
            return _biddingRepository.GetAll(biddersIds);
        }
        public IEnumerable<Bidding> GetAllBetweenDates(DateTime startDate, DateTime endDate, List<string> biddersIds)
        {
            return _biddingRepository.GetAllBetweenDates(startDate,endDate,biddersIds);
        }

        public IEnumerable<Bidding> GetAllAfterSearch(DataTablesParam param, List<string> biddersIds)
        {
            return _biddingRepository.GetAllAfterSearch(param,biddersIds);
        }
        public IEnumerable<Bidding> GetAllBetweenDatesAfterSearch(DataTablesParam param,DateTime startDate,DateTime endDate,List<string> biddersIds)
        {
            return _biddingRepository.GetAllBetweenDatesAfterSearch(param, startDate, endDate, biddersIds);
        } 
        public List<BidderVM> GetBiddersData(DateTime? startDate,DateTime? endDate, List<UserDetail> bidders)
        {
            var toReturn = new List<BidderVM>();

            //Get Biddings on AppliedOn
            var biddings =_biddingRepository.GetAllBetweenDates(startDate, endDate);
            //Get jobs on hired on date 
            var jobs =_jobRepository.GetAll(startDate, endDate);

            foreach(var item in bidders)
            {
                var  bidder = new BidderVM();
                var biddingByUser = biddings.Where(y => y.UserId == item.UserId).ToList();
                bidder.BidderName = item.FirstName;

                if (biddingByUser.Any())
                {
                    bidder.Jobs = biddingByUser.Count();
                    bidder.Replies = biddingByUser.Where(a => a.GetReply == true).Count();
                    bidder.ConnectsUsed = biddingByUser.Sum(x => x.ConnectsUsed);
                }
                bidder.Hires =jobs.Any() ? jobs.Where(y => y.UserId == item.UserId).Count():default;
                toReturn.Add(bidder);
            }
            return toReturn;
        }

        /// <summary>
        /// get bidding by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bidding GetByID(int id)
        {
            return _biddingRepository.GetbyId(id);
        }

        /// <summary>
        /// get bidding viewmodel information by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AddEditBiddingViewModel GetByIDVM(int id)
        {
            Bidding bidding = _biddingRepository.GetbyId(id);
            var biddingrVM = new AddEditBiddingViewModel();
            if (bidding != null)
            {
                biddingrVM = new AddEditBiddingViewModel
                {
                    Id = bidding.Id,
                    Title = bidding.Title,
                    JobUrl = bidding.JobUrl,
                    ProposalUrl = bidding.ProposalUrl,
                    UserId = bidding.UserId,
                    CreatedOn = DateTime.Now,
                    PlatformId = bidding.PlatformId,
                    AppliedUnderId = bidding.AppliedUnderId,
                    DeveloperId = bidding.DeveloperId,
                    ProjectTypeId = bidding.ProjectTypeId,
                    TechnologyId = bidding.TechnologyId,
                    Price = bidding.Price,
                    Notes = bidding.Notes,
                    ConnectsUsed = bidding.ConnectsUsed,
                    AppliedOn = bidding.AppliedOn,
                    AppliedDate= bidding.AppliedOn.ToString("dd/MM/yyyy"),
                    GetReply = bidding.GetReply,
                    TeamLeadId = bidding.TeamLeadId,
                    UserName = bidding.AspNetUser?.UserName,
                    PlatformName = bidding.Platform?.Name,
                    AppliedUnderName = bidding.AppliedUnder?.Name,
                    DeveloperName = bidding.AppliedUnder.Name,
                    ProjectTypeName = bidding.ProjectType.Name,
                    TechnologyName = bidding.Technology?.Name
                };
            }
            return biddingrVM;
        }

        /// <summary>
        /// insert or update users
        /// </summary>
        /// <param name="biddingVM"></param>
        /// <returns></returns>
        public Bidding InsertUpdate(AddEditBiddingViewModel biddingVM)
        {
            try
            {
                if (biddingVM != null)
                {
                    // Get  exitting bid by Id
                    Bidding bidding = _biddingRepository.GetbyId(biddingVM.Id);
 
                    if (bidding != null)
                    {
                        bidding.Title = biddingVM.Title;
                        bidding.JobUrl = biddingVM.JobUrl;
                        bidding.ProposalUrl = biddingVM.ProposalUrl;
                        //bidding.UserId = biddingVM.UserId;
                        bidding.PlatformId = biddingVM.PlatformId;
                        bidding.AppliedUnderId = biddingVM.AppliedUnderId;
                        bidding.DeveloperId = biddingVM.DeveloperId;
                        bidding.TechnologyId = biddingVM.TechnologyId;
                        bidding.ProjectTypeId = biddingVM.ProjectTypeId;
                        bidding.Price = biddingVM.Price;
                        bidding.Notes = biddingVM.Notes;
                        bidding.ConnectsUsed = biddingVM.ConnectsUsed;
                        bidding.AppliedOn = biddingVM.AppliedOn;
                        //bidding.GetReply = biddingVM.GetReply;
                        //bidding.TeamLeadId = biddingVM.GetReply == false ? biddingVM.TeamLeadId = null : biddingVM.TeamLeadId; 
                        _biddingRepository.Update(bidding);
                        return bidding;
                    }
                    else
                    {  // insert user 
                        Bidding biddingEntity = new Bidding
                        {
                            Title = biddingVM.Title,
                            JobUrl = biddingVM.JobUrl,
                            ProposalUrl = biddingVM.ProposalUrl,
                            UserId = biddingVM.UserId,
                            PlatformId = biddingVM.PlatformId,
                            AppliedUnderId = biddingVM.AppliedUnderId,
                            DeveloperId = biddingVM.DeveloperId,
                            TechnologyId = biddingVM.TechnologyId,
                            ProjectTypeId = biddingVM.ProjectTypeId,
                            Price = biddingVM.Price,
                            Notes = biddingVM.Notes,
                            ConnectsUsed = biddingVM.ConnectsUsed,
                            AppliedOn = biddingVM.AppliedOn,
                            CreatedOn = DateTime.Now,
                            GetReply = false,
                            //TeamLeadId= biddingVM.TeamLeadId
                        };
                        _biddingRepository.Insert(biddingEntity);
                        return biddingEntity;
                    }
                }
                else
                {
                    throw new Exception("biddingVM is null.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Bidding InsertUpdateTeamLead(AddEditBiddingViewModel biddingVM)
        {
            try
            {
                if (biddingVM != null)
                {
                    // Get  exitting bid by Id
                    Bidding bidding = _biddingRepository.GetbyId(biddingVM.Id);

                    if (bidding != null)
                    {
                        //if teamlead is there then GetReply true
                        bidding.GetReply = true;
                        bidding.TeamLeadId = biddingVM.TeamLeadId.HasValue ? biddingVM.TeamLeadId : default;
                        _biddingRepository.Update(bidding);
                        return bidding;
                    }
                    else
                    {  // insert user 
                        Bidding biddingEntity = new Bidding();
                            biddingEntity.GetReply = true;
                            biddingEntity.TeamLeadId = biddingVM.TeamLeadId.HasValue ? biddingVM.TeamLeadId : default;
                            _biddingRepository.Insert(biddingEntity);
                        return biddingEntity;
                    }
                }
                else
                {
                    throw new Exception("Sorry an error occured!.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Not in use 
        public IEnumerable<BiddingViewModel> GetTotalContact(DateTime date)
        {
            var biddings = _biddingRepository.GetTotalContact(date).ToList();
            IEnumerable<BiddingViewModel> biddingContactsVm = biddings.GroupBy(g => g.UserId)
                           .Select(x => new BiddingViewModel
                           {
                               UserId = x.Key,
                               UserName=x.Max(a=>a.AspNetUser.UserName),
                               TotalContactOfEach = x.Sum(i => i.ConnectsUsed)
                           });
            return biddingContactsVm;
        }

        public IEnumerable<Bidding> GetTotalBiddingsOfMonth(DateTime date)
        {
            var biddings = _biddingRepository.GetTotalBiddingsOfMonth(date);
            return biddings;
        }
        public IEnumerable<Bidding> GetBiddersTotalBiddingsOfMonth(DateTime date)
        {
            var biddings = _biddingRepository.GetBiddersTotalBiddingsOfMonth(date);
            return biddings;
        }
        public IEnumerable<Bidding> GetUserTotalBiddingsOfMonth(DateTime date, string userId)
        {
            var biddings = _biddingRepository.GetUserTotalBiddingsOfMonth(date,userId);
            return biddings;
        } 
        public IEnumerable<Bidding> GetUserTotalBiddingsOfYear(DateTime date, string userId)
        {
            var biddings = _biddingRepository.GetUserTotalBiddingsOfYear(date,userId);
            return biddings;
        }

    }
}