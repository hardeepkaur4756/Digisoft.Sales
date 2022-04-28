using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Repositories
{
    public class JobRepository : IRepository<Job>
    {
        private readonly SalesEntities _context;
        public JobRepository(SalesEntities context)
        {
            _context = context;
        }
        public JobRepository()
        {
            _context = new SalesEntities();
        }
        public IEnumerable<Job> GetAll()
        {
            return _context.Jobs;
        }
        public IEnumerable<Job> GetAll(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Jobs.Where(x => x.HiredOn >= startDate && x.HiredOn <= endDate);
            }
            else
            {
                return _context.Jobs;
            }
        }
        public IEnumerable<Job> GetAllAfterSearch(DataTablesParam param, DateTime? startDate, DateTime? endDate)
        {
            var sSearch = param.sSearch.ToLower();

            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Jobs.Where(x =>
                       (x.HiredOn >= startDate && x.HiredOn <= endDate) &&
                       (x.AppliedUnder.Name.ToLower().Contains(sSearch)
                      || x.Bidding.Title.ToLower().Contains(sSearch)
                      || x.Client.Name.ToLower().Contains(sSearch)
                      || x.ProjectType.Name.ToLower().Contains(sSearch)
                      || x.Platform.Name.ToLower().Contains(sSearch)
                      || x.Price.ToString().Contains(sSearch)
                      || x.Developer.Name.ToLower().Contains(sSearch)
                      || x.HiredOn.ToString().Contains(sSearch)));
            }
            else
            {
                return _context.Jobs.Where(x =>
                      x.AppliedUnder.Name.ToLower().Contains(sSearch)
                     || x.Bidding.Title.ToLower().Contains(sSearch)
                     || x.Client.Name.ToLower().Contains(sSearch)
                     || x.ProjectType.Name.ToLower().Contains(sSearch)
                     || x.Platform.Name.ToLower().Contains(sSearch)
                     || x.Price.ToString().Contains(sSearch)
                     || x.Developer.Name.ToLower().Contains(sSearch)
                     || x.HiredOn.ToString().Contains(sSearch));
            }

        }

        public Job GetbyId(int id)
        {
            return _context.Jobs.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            Job model = _context.Jobs.FirstOrDefault(x => x.Id == id);
            _context.Jobs.Remove(model);
            _context.SaveChanges();
        }

        public Job Update(Job Job)
        {
            _context.SaveChanges();
            return Job;
        }

        public Job Insert(Job Job)
        {
            _context.Jobs.Add(Job);
            _context.SaveChanges();
            return Job;
        }
        public IEnumerable<Job> GetTotalJobsOfMonth(DateTime date)
        {
            return _context.Jobs.Where(x => x.HiredOn.Month == date.Month && x.HiredOn.Year == date.Year);
        }
        public IEnumerable<Job> GetBiddersTotalJobsOfMonth(DateTime date)
        {
            return _context.Jobs.Where(x => x.HiredOn.Month == date.Month && x.HiredOn.Year == date.Year && x.AspNetUser.AspNetRoles.Any(a => a.Name == "Bidder"));
        }
        public IEnumerable<Job> GetUserTotalJobsOfMonth(DateTime date, string userId)
        {
            return _context.Jobs.Where(x => x.UserId == userId && x.HiredOn.Month == date.Month && x.HiredOn.Year == date.Year);
        }
        public IEnumerable<Job> GetUserTotalJobsOfYear(DateTime date, string userId)
        {
            return _context.Jobs.Where(x => x.UserId == userId && x.HiredOn.Year == date.Year);
        }
        public IEnumerable<Job> GetBidJobs(int id)
        {
            return _context.Jobs.Where(x => x.BiddingId == id);
        }
    }
}