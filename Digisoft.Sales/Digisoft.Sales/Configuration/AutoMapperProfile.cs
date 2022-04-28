using AutoMapper;
using Digisoft.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Digisoft.Sales.Configuration
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        #region Constructor
        public AutoMapperProfile()
        {
            CreateMap<Client, ClientViewModel>()
                 .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.AspNetUser.UserName));
            CreateMap<ClientViewModel, Client>();

            CreateMap<Job, JobViewModel>()
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AspNetUser.UserName))
                 .ForMember(dest => dest.HiredDate, opt => opt.MapFrom(src => src.HiredOn.ToString("dd/MM/yyyy")))
                 .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                 .ForMember(dest => dest.BiddingTitle, opt => opt.MapFrom(src => src.Bidding.Title))
                 .ForMember(dest => dest.AppliedUnderName, opt => opt.MapFrom(src => src.AppliedUnder.Name))
                 .ForMember(dest => dest.DeveloperName, opt => opt.MapFrom(src => src.Developer.Name))
                 .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.Platform.Name))
                 .ForMember(dest => dest.ProjectTypeName, opt => opt.MapFrom(src => src.ProjectType.Name))
                 .ForMember(dest => dest.TeamLeadName, opt => opt.MapFrom(src => src.TeamLead.Name));
            CreateMap<JobViewModel, Job>();

            CreateMap<Billing, BillingViewModel>()
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => string.Format("{0}-{1}", src.Job.Client.Name, src.Job.Developer.Name)))
                .ForMember(dest => dest.ProjectTypeId, opt => opt.MapFrom(src => src.Job.ProjectTypeId))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.AspNetUser.UserDetails.Max(s => (string.Format("{0} {1}", s.FirstName, s.LastName)))))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => Convert.ToDecimal(src.Job.Price * src.Hours)))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.AspNetUser.UserName))
                .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Job));

            CreateMap<BillingViewModel, Billing>();

        }
        #endregion

        public static void Run()
        {
            AutoMapper.Mapper.Initialize(a => { a.AddProfile<AutoMapperProfile>(); });

        }


    }
}