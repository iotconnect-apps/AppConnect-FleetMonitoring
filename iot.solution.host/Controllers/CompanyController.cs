using host.iot.solution.Filter;
using iot.solution.entity.Structs.Routes;
using iot.solution.service.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using Entity = iot.solution.entity;

namespace host.iot.solution.Controllers
{
    [Route(CompanyRoute.Route.Global)]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _service;

        public CompanyController(ICompanyService companyService)
        {
            _service = companyService;
        }

        [HttpGet]
        [Route(CompanyRoute.Route.GetList, Name = CompanyRoute.Name.GetList)]
        public Entity.BaseResponse<List<Entity.Company>> Get()
        {
            Entity.BaseResponse<List<Entity.Company>> response = new Entity.BaseResponse<List<Entity.Company>>(true);
            try
            {
                response.Data = _service.Get();
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<List<Entity.Company>>(false, ex.Message);
            }
            return response;
        }

        [HttpGet]
        [Route(CompanyRoute.Route.GetById, Name = CompanyRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Company")]
        public Entity.BaseResponse<Entity.Company> Get(string id)
        {
            Entity.BaseResponse<Entity.Company> response = new Entity.BaseResponse<Entity.Company>(true);
            try
            {
                response.Data = _service.Get(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Company>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(CompanyRoute.Route.Manage, Name = CompanyRoute.Name.Manage)]
        public Entity.BaseResponse<Entity.Company> Manage([FromBody]Entity.AddCompanyRequest request)
        {
            Entity.BaseResponse<Entity.Company> response = new Entity.BaseResponse<Entity.Company>(true);
            try
            {
                var status = _service.Manage(request);
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Data;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<Entity.Company>(false, ex.Message);
            }
            return response;
        }

        [HttpPut]
        [Route(CompanyRoute.Route.Delete, Name = CompanyRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Company")]
        public Entity.BaseResponse<bool> Delete(string id)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                var status = _service.Delete(Guid.Parse(id));
                response.IsSuccess = status.Success;
                response.Message = status.Message;
                response.Data = status.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route(CompanyRoute.Route.UpdateStatus, Name = CompanyRoute.Name.UpdateStatus)]
        [EnsureGuidParameter("id", "Company")]
        public Entity.BaseResponse<bool> UpdateStatus(string id, bool status)
        {
            Entity.BaseResponse<bool> response = new Entity.BaseResponse<bool>(true);
            try
            {
                Entity.ActionStatus result = _service.UpdateStatus(Guid.Parse(id), status);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Success;
            }
            catch (Exception ex)
            {
                base.LogException(ex);
                return new Entity.BaseResponse<bool>(false, ex.Message);
            }
            return response;
        }

    }
}