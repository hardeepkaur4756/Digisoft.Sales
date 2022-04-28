using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Repositories
{
    public class ClientRepository : IRepository<Client>
    {
        private readonly SalesEntities _context;
        public ClientRepository(SalesEntities context)
        {
            _context = context;
        }
        public ClientRepository()
        {
            _context = new SalesEntities();
        }
        public IEnumerable<Client> GetAll()
        {
            return _context.Clients.Include(x=>x.AspNetUser);
        }
        public IEnumerable<Client> GetAll(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Clients.Where(x => 
                (x.CreatedOn.Day >= ((DateTime)startDate).Day 
                && x.CreatedOn.Month >= ((DateTime)startDate).Month
                && x.CreatedOn.Year >= ((DateTime)startDate).Year)
                &&
                (x.CreatedOn.Day <= ((DateTime)endDate).Day
                && x.CreatedOn.Month <= ((DateTime)endDate).Month
                && x.CreatedOn.Year <= ((DateTime)endDate).Year)
                );
                //return _context.Clients.Where(x => x.CreatedOn >= startDate && x.CreatedOn <= endDate);
            }
            else
            {
                return _context.Clients;
            }
        }

        public IEnumerable<Client> GetAllAfterSearch(DataTablesParam param,DateTime? startDate, DateTime? endDate)
        {
            var sSearch = param.sSearch.ToLower();
            if (startDate.HasValue && endDate.HasValue)
            {
                return _context.Clients
                      .Where(x =>
                      (x.CreatedOn >= startDate && x.CreatedOn <= endDate) &&
                        (x.Name.ToLower().Contains(sSearch)
                      || x.Skype.ToLower().Contains(sSearch)
                      || x.EmailAdress.ToLower().Contains(sSearch)
                      || x.AspNetUser.UserName.ToLower().Contains(sSearch))
                      || x.CreatedOn.ToString().Contains(sSearch)
                      );
            }
            else
            {
                return _context.Clients.Where(x => 
                         x.Name.ToLower().Contains(sSearch)
                         || x.Skype.ToLower().Contains(sSearch)
                         || x.EmailAdress.ToLower().Contains(sSearch)
                         || x.AspNetUser.UserName.ToLower().Contains(sSearch)
                         //|| x.CreatedOn.ToString().Contains(sSearch)
                         );
            }
        }

        public Client GetbyId(int id)
        {
            return _context.Clients.FirstOrDefault(x => x.Id == id);
        }
        public void Delete(int id)
        {
            Client model = _context.Clients.FirstOrDefault(x => x.Id == id);
            _context.Clients.Remove(model);
            _context.SaveChanges();
        }

        public Client Update(Client client)
        {
            _context.SaveChanges();
            return client;
        }

        public Client Insert(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
            return client;
        }
    }
}