

using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.Interface
{
    internal interface IFirmware
    {
        Task<DataResponse<List<AllFirmwareResult>>> All(PagingModel pagingModel);
        Task<DataResponse<List<FirmwareDetailsResult>>> FirmwareDetails(string firmwareGuid);
        Task<DataResponse<List<FirmwareLookupResult>>> LookUp();
        Task<DataResponse<AddFirmwareResult>> Add(AddFirmwareModel createRequest);
        Task<DataResponse<AddFirmwareResult>> FirmwareValidate(FirmwareValidateModel request);
        Task<DataResponse<AddFirmwareResult>> Update(UpdateFirmwareModel updateRequest);
        Task<DataResponse<List<FirmwareUpgradeLookupResult>>> FirmwareLookUpById(string firmwareGuid);
        Task<DataResponse<List<FirmwareUpgradeLookupResult>>> FirmwareUpgradeLookUpById(string firmwareGuid);
        Task<DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>> FirmwareUpgradeByFirmwareUpgradeGuid(string firmwareupgradeGuid);
        Task<DataResponse<List<FirmwareUpgradeByUpgradeGuidResult>>> AllFirmwareUpgrade(string firmwareGuid, string type, PagingModel pagingModel);
        Task<DataResponse<AddFirmwareResult>> AddFirmwareUpgrade(AddFirmwareUpgradeModel createRequest);
        Task<DataResponse<AddFirmwareResult>> UpdateFirmwareUpgrade(UpdateFirmwareUpgradeModel updateRequest);
        Task<DataResponse<AddFirmwareResult>> PublishFirmwareUpgrade(string firmwareUpgradeGuid);
        Task<DataResponse<DeleteAttributeResult>> DeleteFirmwareUpgrade(string firmwareUpgradeGuid, bool isDeleteSingleFirmware);
        Task<DataResponse<List<RecentOTAResult>>> SingleOTA(string deviceGuid, PagingModel pagingModel);
        Task<DataResponse<List<RecentOTAResult>>> RecentOTA(AllRecentOTAModel allRecentOTAModel);
        Task<DataResponse<List<AllRecentActivityResult>>> RecentActivity(RecentActivityModel allRecentActivityModel);
        Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> OTAUpdateByDeviceGuid(string deviceGuid);
        Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> AllDeviceByOTAUpdate(AllDeviceOtaUpdateModel allDeviceOtaUpdateModel);
        Task<DataResponse<List<AllDeviceByOtaUpdateResult>>> AllOTAUpdateByGuidAndStatus(AllDeviceOtaUpdateModel allDeviceOtaUpdateModel);
        Task<DataResponse<List<AllOTAUpdateResult>>> AllOTAUpdate(PagingModel pagingModel);
        Task<DataResponse<SendOTAUpdateResult>> SendOTAUpdate(SendOTAUpdateModel request);
        Task<DataResponse<UpdateOTAStatusResult>> UpdateOTAStatus(UpdateOTAStatusModel request);
        Task<DataResponse<List<OTAUpdateHistoryResult>>> OTAUpdateHistory(OTAUpdateHistoryModel createRequest);        
    }
}
