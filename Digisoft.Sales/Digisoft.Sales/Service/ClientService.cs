using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories;
using Digisoft.Sales.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Service
{
    public class ClientService : IClientService
    {
        private readonly ClientRepository _clientRepository;

        public ClientService()
        {
            _clientRepository = new ClientRepository();
        }

        public void Delete(int id)
        {
            _clientRepository.Delete(id);
        }

        public IEnumerable<Client> GetAll()
        {
            return _clientRepository.GetAll();
        }
         public IEnumerable<Client> GetAll(DateTime? startDate, DateTime? endDate)
        {
            return _clientRepository.GetAll(startDate, endDate);
        }

        public IEnumerable<Client> GetAllAfterSearch(DataTablesParam param, DateTime? startDate, DateTime? endDate)
        {
            return _clientRepository.GetAllAfterSearch(param, startDate, endDate); 
        }

        /// <summary>
        /// get client by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Client GetByID(int id)
        {
            return _clientRepository.GetbyId(id);
        }

        /// <summary>
        /// insert or update clients
        /// </summary>
        /// <param name="clientVM"></param>
        /// <returns></returns>
        public Client InsertUpdate(ClientViewModel clientVM)
        {
            try
            {    
                if (clientVM != null)
                {
                    // Get  exitting client by Id
                    Client client = _clientRepository.GetbyId(clientVM.Id);

                    if (clientVM.Id > 0)
                    {
                        clientVM.CreatedOn = client.CreatedOn;
                        clientVM.CreatedBy = client.CreatedBy;
                        AutoMapper.Mapper.Map(clientVM, client);
                        _clientRepository.Update(client);
                        return client;
                    }
                    else
                    {  // insert client 
                        clientVM.CreatedOn = DateTime.Now;
                        Client clientEntity = new Client();
                        AutoMapper.Mapper.Map(clientVM, clientEntity);
                        _clientRepository.Insert(clientEntity);
                        return clientEntity;
                    }
                }
                else
                {
                    throw new Exception("clientVM is null.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ClientViewModel GetByIDVM(int id)
        {
            Client client = _clientRepository.GetbyId(id);
            ClientViewModel clientVM = new ClientViewModel();
            AutoMapper.Mapper.Map(client, clientVM);
            return clientVM;
        }

    }
}