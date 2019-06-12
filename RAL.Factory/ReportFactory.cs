using AutoMapper;
using ExtendedXmlSerializer.Configuration;
using RAL.ConfigStorageTypes;
using RAL.Reports;
using RAL.ReportsScheduler;
using RAL.RulesEngine;
using RAL.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace RAL.Factory
{
    public class ReportFactory
    {
        private List<Action> listOfLoadOperations = new List<Action>();

        private List<Action> listOfActions = new List<Action>();

        IMapper mapper = new Mapper(
            new MapperConfiguration(cfg =>
                cfg.CreateMap<MachineConfigDSC, MachineInfo>(MemberList.Source)
                .ForSourceMember(x => x.PayloadConverterType, opt => opt.DoNotValidate())
                .ForSourceMember(x => x.MachineType, opt => opt.DoNotValidate())

            )
            );

        private ILogger _logger;

        private IRepositoryForReports repoToInject;

        private ILogger loggerToInject;

        private IList<IReportDaily> listOfReports = new List<IReportDaily>();

        public ReportFactory(ILogger logger = null)
        {
            _logger = logger;
        }
        
        public ReportFactory AddFromStorageType(EmailReportsDSC emailReports)
        {
            listOfLoadOperations.Add(() =>
            {

                foreach (var reportEmailDSC in emailReports.listOfReportConfigs)
                {

                    switch (reportEmailDSC.TypeOfEmailReport)
                    {
                        case Type reportType when reportType == typeof(DepartmentShiftReportForCurrentDay):


                            var report = new DepartmentShiftReportForCurrentDay(reportEmailDSC.EmailAddress, reportEmailDSC.Department, repoToInject, emailReports.SMTPServerHostName, emailReports.SMTPServerPort, emailReports.Credentials, loggerToInject);

                            listOfReports.Add(report);
                            break;
                        default:
                            throw new InvalidCastException();
                    }             
                }

                _logger.Here(nameof(ReportFactory), "").Debug("{stackLightCount} were Loaded", listOfReports.Count());
            });
            return this;
        }

        public ReportFactory LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Reports Config file not found", filePath);
            }

            EmailReportsDSC emailReportDSC;

            var ser = new ConfigurationContainer().Create();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                emailReportDSC = (EmailReportsDSC)ser.Deserialize(reader);

                AddFromStorageType(emailReportDSC);
            }

            return this;
        }

        public ReportFactory WithRepository(IRepositoryForReports repository)
        {

            repoToInject = repository;

            return this;
        }

        public ReportFactory WithLogger(ILogger logger)
        {
            loggerToInject = logger;
            return this;
        }

        public IList<IReportDaily> Build()
        {
            if(repoToInject is null)
            {
                throw new InvalidOperationException($"Repository Writer is Required, please call {nameof(WithRepository)} before {nameof(Build)}.");
            }

            foreach (var loadAction in listOfLoadOperations)
            {
                loadAction.Invoke();
            }

            return listOfReports;
        }
    }
}
