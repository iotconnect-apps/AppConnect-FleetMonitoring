using System;
using System.Collections.Generic;
using System.Text;
using Model = iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class MasterWidgetRepository : GenericRepository<Model.MasterWidget>, IMasterWidgetRepository
    {
        private readonly LogHandler.Logger _logger;

        public MasterWidgetRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger) : base(unitOfWork, logger)
        {
            _logger = logger;
            _uow = unitOfWork;
        }
    }
}
