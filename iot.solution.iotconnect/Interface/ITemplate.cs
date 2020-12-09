using IoTConnect.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTConnect.TemplateProvider
{
   internal interface ITemplate
    {
        Task<DataResponse<SingleTemplateResult>> Single(string deviceTemplateGuid);
        Task<DataResponse<AddTemplateResult>> Add(AddTemplateModel request);
        Task<DataResponse<AddQuickTemplateResult>> Quick(AddQuickTemplateModel request);
        Task<DataResponse<UpdateTemplateResult>> Update(string templateGuid, UpdateTemplateModel request);
        Task<DataResponse<List<AllTemplateResult>>> All(PagingModel pagingModel);
        Task<DataResponse<DeleteTemplateResult>> Delete(string templateGuid);
        Task<DataResponse<List<AllDataTypeResult>>> DataType();
        Task<DataResponse<AddAttributeResult>> AddAttribute(AddAttributeModel request);
        Task<DataResponse<UpdateAttributeResult>> UpdateAttribute(string AttributeGuid, UpdateAttributeModel request);
        Task<DataResponse<DeleteAttributeResult>> DeleteAttribute(string attributeGuid);
        Task<DataResponse<List<AttributeResult>>> AllAttribute(string templateGuid, PagingModel pagingModel, string tag);
        Task<DataResponse<AddTwinResult>> AddTwin(AddTwinModel request);
        Task<DataResponse<UpdateTwinResult>> UpdateTwin(string settingGuid, UpdateTwinPropertyModel request);
        Task<DataResponse<DeleteTwinResult>> DeleteTwin(string seetingGuid);
        Task<DataResponse<List<AllTwinResult>>> AllTemplateTwin(string templateGuid, PagingModel pagingModel);
        Task<DataResponse<List<AllTwinResult>>> AllDeviceTwin(string deviceGuid, PagingModel pagingModel);
        Task<DataResponse<AddCommandResult>> AddCommand(AddCommandModel request);
        Task<DataResponse<UpdateCommandResult>> UpdateCommand(string CommandGuid, UpdateCommandModel request);
        Task<DataResponse<DeleteCommandResult>> DeleteCommand(string commandGuid);
        Task<DataResponse<List<AllCommandResult>>> AllTemplateCommand(string templateGuid, PagingModel pagingModel);
        Task<DataResponse<AddCommandResult>> CommandExecute(string devicetemplateGuid, CommandExecuteModel request);

    }
}
