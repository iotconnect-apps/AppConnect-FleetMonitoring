using component.helper;
using component.helper.Interface;
using component.logger;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using Model = iot.solution.model.Models;
using Response = iot.solution.entity.Response;

namespace iot.solution.service.Implementation
{
    public class SubscriberService : ISubscriberService
    {
        private readonly LogHandler.Logger _logger;
        private readonly ISubscriberHelper _subscriberHelper;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailHelper _emailHelper;

        public SubscriberService(LogHandler.Logger logger, IHardwareKitRepository hardwareKitRepository, IKitTypeRepository kitTypeRepository, ICompanyRepository companyRepository,IUserRepository userRepository, IEmailHelper emailHelper)
        {
            _logger = logger;
            _subscriberHelper = new SubscriberHelper(logger);
            _hardwareKitRepository = hardwareKitRepository;
            _companyRepository = companyRepository;
            _userRepository = userRepository;
            _emailHelper = emailHelper;
        }
        public Response.CountryResponse GetCountryLookUp()
        {
            Response.CountryResponse response = new Response.CountryResponse();
            try
            {
                response = _subscriberHelper.GetCountryData();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return response;
        }
        public Response.StateResponse GetStateLookUp(string countryID)
        {
            Response.StateResponse response = new Response.StateResponse();
            try
            {
                response = _subscriberHelper.GetStateData(countryID);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return response;
        }
        public Response.TimezoneResponse GetTimezoneLookUp()
        {
            Response.TimezoneResponse response = new Response.TimezoneResponse();
            try
            {
                response = _subscriberHelper.GetTimezoneData();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return response;
        }
        public Response.SubscriptionPlanResponse GetSubscriptionPlans(string solutionID)
        {
            Response.SubscriptionPlanResponse response = new Response.SubscriptionPlanResponse();
            try
            {
                response = _subscriberHelper.GetSubscriptionPlans(solutionID);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return response;
        }
        public Entity.ActionStatus ValidateCompany(Entity.ValidateCompanyRequest requestData)
        {
            Entity.ActionStatus response = new Entity.ActionStatus(true);
            try
            {
                Entity.BaseResponse<bool> validateResult = new Entity.BaseResponse<bool>(false);
                validateResult = _subscriberHelper.ValidateCompany(requestData);

                response.Success = validateResult.IsSuccess;
                response.Message = validateResult.Message;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public Entity.ActionStatus SaveCompany(Entity.SaveCompanyRequest requestData)
        {
            Entity.ActionStatus response = new Entity.ActionStatus(true);
            try
            {
                requestData.User.PhoneCountryCode = requestData.User.PhoneCountryCode.Replace("+", "");
                Entity.SaveCompanyResponse saveResult = _subscriberHelper.CreateCompany(requestData);
                Entity.SubsciberCompanyDetails subscriptionDetail = GetSubscriberDetails(requestData.SolutionCode, requestData.User.Email);
                if (saveResult != null && saveResult.PaymentTransactionId != null)
                {
                    response.Data = saveResult;
                   
                    Model.Company dbCompany = new Model.Company()
                    {
                        Guid = Guid.Parse(saveResult.IoTConnectCompanyGuid),
                        Name = requestData.User.CompanyName,
                        ContactNo = requestData.User.PhoneCountryCode + "-" + requestData.User.Phone,
                        Address = requestData.User.Address,
                        CountryGuid = Guid.Parse(requestData.User.CountryId),
                        TimezoneGuid = Guid.Parse(requestData.User.TimezoneId),
                        StateGuid = Guid.Parse(requestData.User.StateId),
                        City = requestData.User.CityName,
                        PostalCode = requestData.User.PostalCode
                    };
                    Entity.ActionStatus upStatus = _companyRepository.UpdateDetails(dbCompany);

                    Model.User objUser = new Model.User()
                    {
                        CompanyGuid = Guid.Parse(saveResult.IoTConnectCompanyGuid),
                        Email = requestData.User.Email,
                        ContactNo = requestData.User.PhoneCountryCode + "-" + requestData.User.Phone,
                        FirstName = requestData.User.FirstName,
                        LastName = requestData.User.LastName,
                        TimeZoneGuid = Guid.Parse(requestData.User.TimezoneId)
                    };

                    var dbUser = _userRepository.FindBy(u => u.Email.Equals(objUser.Email) && u.CompanyGuid.Equals(objUser.CompanyGuid)).FirstOrDefault();
                    if (dbUser != null)
                    {
                        dbUser.ContactNo = requestData.User.PhoneCountryCode + "-" + requestData.User.Phone;
                        dbUser.FirstName = objUser.FirstName;
                        dbUser.LastName = objUser.LastName;
                        dbUser.TimeZoneGuid = objUser.TimeZoneGuid;
                        dbUser.SubscriptionEndDate = Convert.ToDateTime(subscriptionDetail.renewalDate);
                        _userRepository.Update(dbUser);
                    }
                    string userName = objUser.FirstName + " " + objUser.LastName;
                    _emailHelper.SendCompanyRegistrationEmail(userName, requestData.User.CompanyName,requestData.User.Email, requestData.User.Password);
                    response.Success = true;
                    response.Message = "";
                }
                else
                {
                    if (saveResult != null)
                    {
                        response.Data = saveResult;
                    }
                    response.Success = false;
                    response.Message = "Something Went Wrong!";
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex);
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        private string titleCase(string inputString)
        {
            string outputString = "";
            outputString = Convert.ToString(char.ToUpper(inputString[0]) + inputString.Substring(1));
            return outputString;
        }
        public Entity.SearchResult<List<Entity.SubscriberData>> SubscriberList(string solutionID, Entity.SearchRequest request)
        {
            try
            {
                bool isTitleCase = component.helper.SolutionConfiguration.isTitleCase;

                if (isTitleCase && !string.IsNullOrEmpty(request.OrderBy))
                {
                    request.OrderBy = titleCase(request.OrderBy);
                }

                var result = _subscriberHelper.SubscriberList(solutionID, request);
                return new Entity.SearchResult<List<Entity.SubscriberData>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.SubscriberData>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return new Entity.SearchResult<List<Entity.SubscriberData>>();
            }
        }
        public Entity.SubsciberCompanyDetails GetSubscriberDetails(string solutionCode, string userEmail)
        {
            Entity.SubsciberCompanyDetails result = new Entity.SubsciberCompanyDetails();
            try
            {
                result = _subscriberHelper.GetSubscriberDetails(solutionCode, userEmail);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public Entity.SearchResult<List<Entity.HardwareKitResponse>> GetSubscriberKitDetails(string companyId, Entity.SearchRequest request, bool isAssigned)
        {
            Entity.SearchResult<List<Entity.HardwareKitResponse>> result = new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            try
            {
                result = _hardwareKitRepository.List(request, isAssigned, companyId);

                return new Entity.SearchResult<List<Entity.HardwareKitResponse>>()
                {
                    Items = result.Items.Select(p => Mapper.Configuration.Mapper.Map<Entity.HardwareKitResponse>(p)).ToList(),
                    Count = result.Count
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public Entity.LastSyncResponse GetLastSyncDetails()
        {
            Entity.LastSyncResponse result = new Entity.LastSyncResponse();
            try
            {
                result = _subscriberHelper.GetLastSyncDetails();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}
