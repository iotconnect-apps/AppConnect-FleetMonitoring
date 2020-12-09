using System;
using System.Collections.Generic;
using System.Text;
using Model = iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using component.logger;
using LogHandler = component.services.loghandler;


namespace iot.solution.model.Repository.Implementation
{
    public class UserDashboardWidgetRepository : GenericRepository<Model.UserDasboardWidget>,IUserDashboardWidgetRepository
    {
        private readonly LogHandler.Logger _logger;
        public UserDashboardWidgetRepository(IUnitOfWork unitOfWork, LogHandler.Logger logger) : base(unitOfWork, logger)
        {
            _logger = logger;
            _uow = unitOfWork;
        }
    }
}
