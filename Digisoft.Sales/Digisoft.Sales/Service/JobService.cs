using Digisoft.Sales.Models;
using Digisoft.Sales.Repositories;
using Digisoft.Sales.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Service
{
    public class JobService : IJobService
    {
        private readonly JobRepository _jobRepository;

        public JobService()
        {
            _jobRepository = new JobRepository();
        }

        public void Delete(int id)
        {
            _jobRepository.Delete(id);
        }

        public IEnumerable<Job> GetAll()
        {
            return _jobRepository.GetAll();
        }
        public IEnumerable<Job> GetAll(DateTime? startDate, DateTime? endDate)
        {
            return _jobRepository.GetAll(startDate,endDate);
        }
        public IEnumerable<Job> GetAllAfterSearch(DataTablesParam param, DateTime? startDate, DateTime? endDate)
        {
            return _jobRepository.GetAllAfterSearch(param,startDate,endDate);
        }

        /// <summary>
        /// get job by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Job GetByID(int id)
        {
            return _jobRepository.GetbyId(id);
        }

        /// <summary>
        /// insert or update jobs
        /// </summary>
        /// <param name="jobVM"></param>
        /// <returns></returns>
        public Job InsertUpdate(JobViewModel jobVM)
        {
            try
            {
                if (jobVM != null)
                {
                    // Get  exitting job by Id
                    Job job = _jobRepository.GetbyId(jobVM.Id);

                    if (jobVM.Id > 0)
                    {
                        jobVM.CreatedOn = job.CreatedOn;
                        jobVM.UserId = job.UserId;
                        AutoMapper.Mapper.Map(jobVM, job);
                        _jobRepository.Update(job);
                        return job;
                    }
                    else
                    {  // insert job 
                        jobVM.CreatedOn = DateTime.Now;
                        Job jobEntity = new Job();
                        AutoMapper.Mapper.Map(jobVM, jobEntity);
                        _jobRepository.Insert(jobEntity);
                        return jobEntity;
                    }
                }
                else
                {
                    throw new Exception("jobVM is null.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public JobViewModel GetByIDVM(int id)
        {
            Job job = _jobRepository.GetbyId(id);
            JobViewModel jobVM = new JobViewModel();
            AutoMapper.Mapper.Map(job, jobVM);
            return jobVM;
        }

        public IEnumerable<Job> GetTotalJobsOfMonth(DateTime date)
        {
            var jobs = _jobRepository.GetTotalJobsOfMonth(date);
            return jobs;
        }
        public IEnumerable<Job> GetBiddersTotalJobsOfMonth(DateTime date)
        {
            var jobs = _jobRepository.GetBiddersTotalJobsOfMonth(date);
            return jobs;
        }
        public IEnumerable<Job> GetUserTotalJobsOfMonth(DateTime date,string userId)
        {
            var jobs = _jobRepository.GetUserTotalJobsOfMonth(date, userId);
            return jobs;
        }
        public IEnumerable<Job> GetUserTotalJobsOfYear(DateTime date, string userId)
        {
            var jobs = _jobRepository.GetUserTotalJobsOfYear(date, userId);
            return jobs;
        }
        public IEnumerable<Job> GetBidJobs(int id)
        {
            return _jobRepository.GetBidJobs(id);
        }
    }
}