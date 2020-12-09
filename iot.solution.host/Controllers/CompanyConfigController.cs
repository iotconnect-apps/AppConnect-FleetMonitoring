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
    [Route(CompanyConfigRoute.Route.Global)]
    public class CompanyConfigController : BaseController
    {
        private readonly ICompanyConfigService _companyConfigService;
        public CompanyConfigController(ICompanyConfigService companyConfigService)
        {
            _companyConfigService = companyConfigService;
        }

        [ProducesResponseType(typeof(List<Entity.Company>), (int)HttpStatusCode.OK)]
        [HttpGet]
        [Route(CompanyConfigRoute.Route.GetList, Name = CompanyConfigRoute.Name.GetList)]
        public IActionResult Get()
        {
            return Ok(_companyConfigService.Get());
        }

        [ProducesResponseType(typeof(Entity.Company), (int)HttpStatusCode.OK)]
        [HttpGet]
        [Route(CompanyConfigRoute.Route.GetById, Name = CompanyConfigRoute.Name.GetById)]
        [EnsureGuidParameter("id", "Config")]
        public IActionResult Get(string id)
        {
            return Ok(_companyConfigService.Get(Guid.Parse(id)));
        }

        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        [HttpPost]
        [Route(CompanyConfigRoute.Route.Manage, Name = CompanyConfigRoute.Name.Manage)]
        public IActionResult Manage([FromBody]Entity.CompanyConfig company)
        {
            return Ok(_companyConfigService.Manage(company));
        }

        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpPut]
        [Route(CompanyConfigRoute.Route.Delete, Name = CompanyConfigRoute.Name.Delete)]
        [EnsureGuidParameter("id", "Config")]
        public IActionResult Delete(string id)
        {
            return Ok(_companyConfigService.Delete(Guid.Parse(id)));
        }
    }
}