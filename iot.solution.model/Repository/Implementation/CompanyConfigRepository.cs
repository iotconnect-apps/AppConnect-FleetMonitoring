using component.logger;
using iot.solution.model.Models;
using iot.solution.model.Repository.Interface;
using LogHandler = component.services.loghandler;

namespace iot.solution.model.Repository.Implementation
{
    public class CompanyConfigRepository : GenericRepository<CompanyConfig> , ICompanyConfigRepository
    {
        private readonly LogHandler.Logger logger;
        public CompanyConfigRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }
    }
}
