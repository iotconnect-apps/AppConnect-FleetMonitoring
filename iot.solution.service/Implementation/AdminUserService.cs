using component.helper;
using component.logger;
using iot.solution.common;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
using System.Reflection;

namespace iot.solution.service.Implementation
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly LogHandler.Logger _logger;

        public AdminUserService(IAdminUserRepository adminUserRepository, LogHandler.Logger logManager)
        {
            _logger = logManager;
            _adminUserRepository = adminUserRepository;
        }

        public Entity.ActionStatus AdminLogin(Entity.LoginRequest request)
        {
            Entity.ActionStatus result = new ActionStatus(true);
            try
            {
                Model.AdminUser adminUser = _adminUserRepository.AdminLogin(request);
                if (adminUser == null)
                {
                    result.Success = false;
                    result.Message = "No User Found!";
                }
                else if (adminUser == null || !adminUser.Password.Equals(request.Password))
                {
                    result.Success = false;
                    result.Message = "Invalid credentials.";
                }
                else if (adminUser != null && (!adminUser.IsActive || adminUser.IsDeleted))
                {
                    result.Success = false;
                    result.Message = "Your account has been deactivated.";
                }
                else
                {
                    result.Success = true;
                    result.Data = adminUser;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


        public Entity.AdminUserResponse Get(Guid id)
        {
            try
            {
                return _adminUserRepository.FindBy(x => x.Guid.Equals(id)).Select(p => Mapper.Configuration.Mapper.Map<Entity.AdminUserResponse>(p)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }


        public SearchResult<List<UserResponse>> GetAdminUserList(SearchRequest searchRequest)
        {
            SearchResult<List<UserResponse>> userList;
            try
            {
                userList = _adminUserRepository.List(searchRequest);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                return null;
            }

            return userList;
        }

        public Entity.ActionStatus Manage(Entity.AddAdminUserRequest request)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);
            try
            {
                if (request.Id == null || request.Id == Guid.Empty)
                {
                    var checkExisting = _adminUserRepository.FindBy(x => x.Email.Equals(request.Email) && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (checkExisting == null)
                    {

                        var adminUser = Mapper.Configuration.Mapper.Map<Entity.AddAdminUserRequest, Model.AdminUser>(request);



                        adminUser.CreatedBy = SolutionConfiguration.CurrentUserId;
                        adminUser.CreatedDate = DateTime.Now;
                        adminUser.IsActive = true;
                        adminUser.IsDeleted = false;
                        adminUser.Guid = Guid.NewGuid();
                        adminUser.CompanyGuid = SolutionConfiguration.CompanyId;

                        actionStatus = _adminUserRepository.Insert(adminUser);

                        actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                        if (!actionStatus.Success)
                        {
                            _logger.ErrorLog(new Exception($"Admin User is not added in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                            actionStatus.Success = false;
                            actionStatus.Message = "Something Went Wrong!";
                        }
                    }
                    else
                    {
                        // _logger.Error($"AdminUser is not updated in solution database, Error: {actionStatus.Message}");
                        actionStatus.Success = false;
                        actionStatus.Message = "Email Id Already Registered.";
                    }


                }
                else
                {
                    var oldAdminUser = _adminUserRepository.FindBy(x => x.Guid.Equals(request.Id)).FirstOrDefault();
                    if (oldAdminUser == null)
                    {
                        throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminUser");
                    }
                    else
                    {
                        var checkExisting = _adminUserRepository.FindBy(x => x.Email.Equals(request.Email) && x.IsActive && !x.IsDeleted && x.Guid != oldAdminUser.Guid).FirstOrDefault();
                        if (checkExisting == null)
                        {
                            var existingPassword = oldAdminUser.Password;
                            var dbAdminUser = Mapper.Configuration.Mapper.Map(request, oldAdminUser);

                            dbAdminUser.CreatedBy = oldAdminUser.CreatedBy;
                            dbAdminUser.CreatedDate = oldAdminUser.CreatedDate;
                            dbAdminUser.UpdatedDate = DateTime.Now;
                            dbAdminUser.UpdatedBy = SolutionConfiguration.CurrentUserId;
                            dbAdminUser.CompanyGuid = SolutionConfiguration.CompanyId;
                            if(string.IsNullOrEmpty(dbAdminUser.Password))
                                dbAdminUser.Password = existingPassword;
                            actionStatus = _adminUserRepository.Update(dbAdminUser);

                            actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                            if (!actionStatus.Success)
                            {
                                _logger.ErrorLog(new Exception($"AdminUser is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                                actionStatus.Success = false;
                                actionStatus.Message = "Something Went Wrong!";
                            }
                        }
                        else
                        {
                            actionStatus.Success = false;
                            actionStatus.Message = "Email Id Already Registered With Another User.";
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;

        }


        public Entity.ActionStatus Delete(Guid id)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);

            try
            {

                var dbAdminUser = _adminUserRepository.FindBy(x => x.Guid.Equals(id)).FirstOrDefault();

                if (dbAdminUser == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound } : AdminUser");
                }
                else
                {
                    dbAdminUser.IsDeleted = true;
                    dbAdminUser.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    dbAdminUser.UpdatedDate = DateTime.Now;
                    actionStatus = _adminUserRepository.Update(dbAdminUser);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);

                    if (!actionStatus.Success)
                    {
                        _logger.ErrorLog(new Exception($"AdminUser is not deleted in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;
        }
        /// <summary>
        /// Put fasle for InActive and Tru
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionStatus UpdateStatus(Guid id, bool status)
        {
            Entity.ActionStatus actionStatus = new ActionStatus(true);
            try
            {
                var existingUser = _adminUserRepository.FindBy(x => x.Guid.Equals(id) && !x.IsDeleted).FirstOrDefault();

                if (existingUser == null)
                {
                    throw new NotFoundCustomException($"{CommonException.Name.NoRecordsFound} : AdminUser");
                }
                else
                {
                    existingUser.IsActive = status;
                    existingUser.UpdatedBy = SolutionConfiguration.CurrentUserId;
                    existingUser.UpdatedDate = DateTime.Now;
                    actionStatus = _adminUserRepository.Update(existingUser);
                    actionStatus.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.UserResponse>(actionStatus.Data);
                    if (!actionStatus.Success)
                    {
                        _logger.ErrorLog(new Exception($"AdminUser is not updated in solution database, Error: {actionStatus.Message}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                        actionStatus.Success = false;
                        actionStatus.Message = "Something Went Wrong!";
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }

            return actionStatus;
        }

        public Entity.ActionStatus ChangePassword(ChangePasswordRequest request)
        {
            var result = new Entity.ActionStatus();
            try
            {
                var adminUser = _adminUserRepository.FindBy(x => x.Email.Equals(request.Email)).FirstOrDefault();

                if (adminUser != null)
                {
                    if (adminUser.Password.Equals(request.OldPassword))
                    {
                        adminUser.Password = request.NewPassword;
                        adminUser.UpdatedBy = component.helper.SolutionConfiguration.CurrentUserId;
                        adminUser.UpdatedDate = DateTime.Now;

                        result = _adminUserRepository.Update(adminUser);

                        result.Data = Mapper.Configuration.Mapper.Map<Model.AdminUser, Entity.AdminUserResponse>(result.Data);
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Old Password Is Incorrect.";
                        result.Data = null;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "User Not Found.";
                    result.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
    }
}
