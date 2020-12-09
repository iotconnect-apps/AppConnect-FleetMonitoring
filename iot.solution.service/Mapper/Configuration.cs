using Model = iot.solution.model.Models;
using Entity = iot.solution.entity;
using IOTUserProvider = IoTConnect.UserProvider;
using AutoMapper;
using System;
using IOT = IoTConnect.Model;
using iot.solution.service.Mapper.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace iot.solution.service.Mapper
{
    public class Configuration
    {
        public static IMapper Mapper { get; private set; }

        public static void Initialize()
        {
            var config = new MapperConfiguration(mc =>
            {
                mc.CreateMap<Model.User, Entity.User>().ReverseMap();
                mc.CreateMap<Model.User, Entity.AddUserRequest>()
                .ForMember(au => au.EntityGuid, o => o.MapFrom(u => u.EntityGuid)).ReverseMap();
                mc.CreateMap<Model.Role, Entity.Role>().ReverseMap();
                mc.CreateMap<Model.Company, Entity.Company>().ReverseMap();
                mc.CreateMap<Model.Entity, Entity.Entity>().ReverseMap();
                mc.CreateMap<Model.Fleet, Entity.Fleet>().ReverseMap();
                mc.CreateMap<Model.EntityWithCounts, Entity.EntityWithCounts>().ReverseMap();
               
                mc.CreateMap<Model.DeviceMaintenance, Entity.DeviceMaintenance>().ReverseMap();
                mc.CreateMap<Model.Entity, Entity.AddEntityRequest>().ReverseMap();
                mc.CreateMap<Entity.EntityModel, Entity.Entity>().ReverseMap();

                mc.CreateMap<Entity.FleetModel, Entity.Fleet>().ReverseMap();
                mc.CreateMap<Model.FleetModel, Entity.Fleet>().ReverseMap();

                mc.CreateMap<Model.DeviceType, Entity.DeviceType>().ReverseMap();

                mc.CreateMap<Model.Entity, Entity.EntityDetail>().ReverseMap();
                mc.CreateMap<Model.Fleet, Entity.FleetDetail>().ReverseMap();
                mc.CreateMap<Model.Device, Entity.Device>().ReverseMap();
                mc.CreateMap<Model.DeviceDetail, Entity.Device>().ReverseMap();
             
                mc.CreateMap<Model.DeviceModel, Entity.Device>().ReverseMap();
                mc.CreateMap<Entity.DeviceModel, Entity.Device>().ReverseMap();

                mc.CreateMap<Model.User, Entity.UserResponse>().ReverseMap();

                mc.CreateMap<Model.Driver, Entity.Driver>().ReverseMap();
                mc.CreateMap<Entity.DriverModel, Entity.Driver>().ReverseMap();

                mc.CreateMap<Model.Trip, Entity.Trip>().ReverseMap();
                mc.CreateMap<Entity.TripModel, Entity.Trip>().ReverseMap();
                mc.CreateMap<Model.Trip, Entity.TripDetail>().ReverseMap();
                mc.CreateMap<Entity.Trip, Entity.TripDetail>().ReverseMap();
                mc.CreateMap<Model.TripModel, Entity.Trip>().ReverseMap();

                mc.CreateMap<Model.KitTypeAttribute, Entity.KitTypeAttribute>().ReverseMap();
                mc.CreateMap<Model.KitTypeCommand, Entity.KitTypeCommand>().ReverseMap();
                mc.CreateMap<Model.HardwareKit, Entity.HardwareKit>().ReverseMap();
                mc.CreateMap<List<Entity.HardwareKit>, Entity.HardwareKitDTO>().ReverseMap();
                mc.CreateMap<Model.KitType, Entity.KitType>().ReverseMap();
                mc.CreateMap<IOT.AllRuleResult, Entity.AllRuleResponse>().ReverseMap();
                mc.CreateMap<IOT.SingleRuleResult, Entity.SingleRuleResponse>().ReverseMap();
                mc.CreateMap<Entity.Rule, IOT.AddRuleModel>().ReverseMap();
                mc.CreateMap<Entity.Rule, IOT.UpdateRuleModel>().ReverseMap();
                mc.CreateMap<Entity.HardwareKitDTO, Model.HardwareKit>().ReverseMap();
                mc.CreateMap<Model.MasterWidget, Entity.MasterWidget>().ReverseMap();
                mc.CreateMap<Model.UserDasboardWidget, Entity.UserDasboardWidget>().ReverseMap();
                #region " IOT Connect Mapping"

                mc.CreateMap<Entity.Entity, IOT.AddEntityModel>().ConvertUsing(new EntityToAddEntityModelMapping());
                mc.CreateMap<Entity.Entity, IOT.UpdateEntityModel>().ConvertUsing(new EntityToUpdateEntityModelMapping());
                mc.CreateMap<Entity.Device, IOT.AddDeviceModel>().ConvertUsing(new DeviceToAddDeviceModelMapping());
                mc.CreateMap<Entity.Device, IOT.UpdateDeviceModel>().ConvertUsing(new DeviceToUpdateDeviceModelMapping());

                mc.CreateMap<Entity.AddUserRequest, IOT.AddUserModel>().ConvertUsing(new AddUserRequestToAddUserModelMapping());
                mc.CreateMap<Entity.AddUserRequest, IOT.UpdateUserModel>().ConvertUsing(new AddUserRequestToUpdateUserModelMapping());
                mc.CreateMap<Entity.ChangePasswordRequest, IOT.ChangePasswordModel>().ConvertUsing(new ChangePasswordRequestToChangePasswordModel());
                mc.CreateMap<Entity.DeviceCounterResult, IOT.DeviceCounterResult>().ReverseMap();
                mc.CreateMap<Entity.DeviceTelemetryDataResult, IOT.DeviceTelemetryData>().ReverseMap();
                mc.CreateMap<Entity.DeviceConnectionStatusResult, IOT.DeviceConnectionStatus>().ReverseMap();
                
                mc.CreateMap<Entity.DeviceCounterByEntityResult, IOT.DeviceCounterByEntityResult>().ReverseMap();


                #endregion

                #region "AdminUser Mapping"

                mc.CreateMap<Model.AdminUser, Entity.AddAdminUserRequest>().ReverseMap();
                mc.CreateMap<Model.AdminUser, Entity.UserResponse>().ReverseMap();
                mc.CreateMap<Model.AdminUser, Entity.AdminUserResponse>().ReverseMap();

                #endregion
            });

            Mapper = config.CreateMapper();
        }

    }


}