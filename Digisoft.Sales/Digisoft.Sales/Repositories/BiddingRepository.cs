using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Digisoft.Sales.Repositories
{
    public class BiddingRepository : IRepository<Bidding>
    {
        private readonly SalesEntities _context;
        public BiddingRepository(SalesEntities context)
        {
            _context = context;
        }
        public BiddingRepository()
        {
            _context = new SalesEntities();
        }
        public IEnumerable<Bidding> GetAll()
        {
            return _context.Biddings;
        }
        public IEnumerable<Bidding> GetAll(List<string> biddersIds)
        {
            if (biddersIds.Count > 0)
            {
                return _context.Biddings.Where(x => (biddersIds.Contains(x.UserId)));
            }
            else
            {
                return _context.Biddings;
            }
        }

        public IEnumerable<Bidding> GetAllBetweenDates(DateTime startDate, DateTime endDate, List<string> biddersIds)
        {
            if (biddersIds.Count > 0)
            {
                return _context.Biddings.Where(x => (biddersIds.Contains(x.UserId)) &&
                                          (x.AppliedOn >= startDate && x.AppliedOn <= endDate)).ToList();
            }
            else
            {
                return _context.Biddings.Where(x => x.AppliedOn >= startDate && x.AppliedOn <= endDate);
            }
        }

        public IEnumerable<Bidding> GetAllBetweenDates(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Biddings.Where(x => x.AppliedOn >= startDate && x.AppliedOn <= endDate);
            }
            else
            {
                return _context.Biddings;
            }
        }

        public IEnumerable<Bidding> GetAllAfterSearch(DataTablesParam param, List<string> biddersIds)
        {
            var sSearch = param.sSearch.ToLower();

            if (biddersIds.Count > 0)
            {
                var billings = _context.Biddings
                                          .Where(x =>
                                          (biddersIds.Contains(x.UserId)) &&
                                          (x.Title.ToLower().Contains(sSearch)
                          || x.Technology.Name.ToLower().Contains(sSearch)
                          || x.AppliedUnder.Name.ToLower().Contains(sSearch)
                          || x.Platform.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.TeamLead.Name.ToLower().Contains(sSearch)
                          || x.ConnectsUsed.ToString().Contains(sSearch)
                          || x.GetReply.ToString().Contains(sSearch)
                          || x.AppliedOn.ToString().Contains(sSearch))
                          );
                return billings;
            }
            else
            {
                var billings = _context.Biddings
                          .Where(x =>
                             x.Title.ToLower().Contains(sSearch)
                          || x.Technology.Name.ToLower().Contains(sSearch)
                          || x.AppliedUnder.Name.ToLower().Contains(sSearch)
                          || x.Platform.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.TeamLead.Name.ToLower().Contains(sSearch)
                          || x.ConnectsUsed.ToString().Contains(sSearch)
                          || x.GetReply.ToString().Contains(sSearch)
                          || x.AppliedOn.ToString().Contains(sSearch)
                          );
                return billings;
            }
        }
        public IEnumerable<Bidding> GetAllBetweenDatesAfterSearch(DataTablesParam param, DateTime startDate, DateTime endDate, List<string> biddersIds)
        {
            var sSearch = param.sSearch.ToLower();

            if (biddersIds.Count > 0)
            {
                var billings = _context.Biddings
                                          .Where(x =>
                                          (biddersIds.Contains(x.UserId)) &&
                                           (x.AppliedOn >= startDate && x.AppliedOn <= endDate) &&
                                             (x.Title.ToLower().Contains(sSearch)
                                          || x.Technology.Name.ToLower().Contains(sSearch)
                                          || x.AppliedUnder.Name.ToLower().Contains(sSearch)
                                          || x.Platform.Name.ToLower().Contains(sSearch)
                                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                                          || x.TeamLead.Name.ToLower().Contains(sSearch)
                                          || x.ConnectsUsed.ToString().Contains(sSearch)
                                          || x.GetReply.ToString().Contains(sSearch)
                                          || x.AppliedOn.ToString().Contains(sSearch))
                                          ).ToList();
                return billings;
            }
            else
            {
                var billings = _context.Biddings
                          .Where(x =>
                           (x.AppliedOn >= startDate && x.AppliedOn <= endDate) &&
                             (x.Title.ToLower().Contains(sSearch)
                          || x.Technology.Name.ToLower().Contains(sSearch)
                          || x.AppliedUnder.Name.ToLower().Contains(sSearch)
                          || x.Platform.Name.ToLower().Contains(sSearch)
                          || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                          || x.TeamLead.Name.ToLower().Contains(sSearch)
                          || x.ConnectsUsed.ToString().Contains(sSearch)
                          || x.GetReply.ToString().Contains(sSearch)
                          || x.AppliedOn.ToString().Contains(sSearch))
                          );
                return billings;
            }

        }

        public Bidding GetbyId(int id)
        {
            return _context.Biddings.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            Bidding model = _context.Biddings.FirstOrDefault(x => x.Id == id);
            _context.Biddings.Remove(model);
            _context.SaveChanges();
        }

        public Bidding Update(Bidding bid)
        {
            _context.SaveChanges();
            return bid;
        }

        public Bidding Insert(Bidding bid)
        {
            _context.Biddings.Add(bid);
            _context.SaveChanges();
            return bid;
        }
        public IEnumerable<Bidding> GetTotalContact(DateTime date)
        {
            return _context.Biddings.Where(x => x.AppliedOn.Month == date.Month && x.AppliedOn.Year == date.Year && x.AspNetUser.AspNetRoles.Any(a => a.Name == "Bidder"));
        }

        public IEnumerable<Bidding> GetTotalBiddingsOfMonth(DateTime date)
        {
            return _context.Biddings.Where(x => x.AppliedOn.Month == date.Month && x.AppliedOn.Year == date.Year);
        }
        public IEnumerable<Bidding> GetBiddersTotalBiddingsOfMonth(DateTime date)
        {
            return _context.Biddings.Where(x => x.AppliedOn.Month == date.Month && x.AppliedOn.Year == date.Year && x.AspNetUser.AspNetRoles.Any(a => a.Name == "Bidder"));
        }
        public IEnumerable<Bidding> GetUserTotalBiddingsOfMonth(DateTime date, string userId)
        {
            return _context.Biddings.Where(x => x.UserId == userId && x.AppliedOn.Month == date.Month && x.AppliedOn.Year == date.Year);
        }
        public IEnumerable<Bidding> GetUserTotalBiddingsOfYear(DateTime date, string userId)
        {
            return _context.Biddings.Where(x => x.UserId == userId && x.AppliedOn.Year == date.Year);
        }

    }
}