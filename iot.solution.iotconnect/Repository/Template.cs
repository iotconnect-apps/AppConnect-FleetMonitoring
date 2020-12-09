using Flurl.Http;
using IoTConnect.Common;
using IoTConnect.Common.Constant;
using IoTConnect.Common.Interface;
using IoTConnect.Common.Repository;
using IoTConnect.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTConnect.TemplateProvider
{
    /// <summary>
    /// Device Template.
    /// </summary>
    public class Template : ITemplate
    {
        #region Private properties
        private string _token = string.Empty;
        private IIoTConnectAPIDiscovery _ioTConnectAPIDiscovery;
        private string _envCode = string.Empty;
        private string _solutionKey = string.Empty;
        #endregion

        #region Ctor
        public Template(string token, string environmentCode, string solutionKey)
        {
            _token = token;
            _envCode = environmentCode;
            _solutionKey = solutionKey;
            _ioTConnectAPIDiscovery = new IoTConnectAPIDiscovery();
            FlurlHttp.Configure(settings => settings.OnError = HandleFlurlErrorAsync);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Handles the flurl error asynchronous.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        private void HandleFlurlErrorAsync(HttpCall call)
        {
            call.ExceptionHandled = true;
            IoTConnectException ioTConnectErrorResponse = JsonConvert.DeserializeObject<IoTConnectException>(call.Response.Content.ReadAsStringAsync().Result);
            throw ioTConnectErrorResponse;
        }
        #endregion

        #region Template Crud
        /// <summary>
        /// Get Device Template including Attributes, Commands and Settings.
        /// </summary>
        /// <param name="TemplateGuid">Template Unique Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<SingleTemplateResult>> Single(string TemplateGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(TemplateGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "TemplateGuid is required",
                        Param = "TemplateGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<SingleTemplateResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.TemplateLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, TemplateGuid);
                var deviceTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                .GetJsonAsync<DataResponse<List<SingleTemplateResult>>>();

                return new DataResponse<SingleTemplateResult>(null)
                {
                    data = deviceTemplate.data.FirstOrDefault(),
                    message = deviceTemplate.message,
                    status = deviceTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<SingleTemplateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "Single()");
                throw ex;
            }
        }

        /// <summary>
        /// Add a new Device Template.
        /// To manage devices and their associated properties, one needs a pre-defined structure. A device template provides that structure by allowing you to create attributes, settings, properties, rules, commands and much more – all to help you to monitor and manage your device management system efficiently.
        /// By using this, you can also create gateway supported device templates.To do that, enter tag parameter value in the request body.You can manage gateways in the same way as any other device of your device management system.
        /// </summary>
        /// <param name="request">Add template Request.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddTemplateResult>> Add(AddTemplateModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddTemplateResult>(null)
                    {
                        errorMessages = errorList,
                        status = false
                    };
                }
                request.CustomETPlaceHolders = null;
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AddTemplate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addTempalte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddTemplateResult>>>();

                return new DataResponse<AddTemplateResult>(null)
                {
                    data = addTempalte.data.FirstOrDefault(),
                    message = addTempalte.message,
                    status = addTempalte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddTemplateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "Add()");
                throw ex;
            }
        }

        /// <summary>
        /// Update Device Template.
        /// To update a device template, use this. You can only update name, description and firmware hardware version of the device template.
        /// </summary>
        /// <param name="TemplateGuid">Template Unique Identifier.</param>
        /// <param name="request">Update Template Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateTemplateResult>> Update(string TemplateGuid, UpdateTemplateModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateTemplateResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.UpdateTemplate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, TemplateGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateTemplateResult>>>();

                return new DataResponse<UpdateTemplateResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateTemplateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "Update()");
                throw ex;
            }
        }

        /// <summary>
        /// Get the list of available device templates of your company. Though filters are optional, you can add them as given in the parameters.To apply the filters, you need to pass these filters in query string.
        /// </summary>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTemplateResult>>> All(PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.Templates);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllTemplateResult>>>();

                return new DataResponse<List<AllTemplateResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTemplateResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "All()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Template.
        /// To do that, you need to pass a device template guid that you want to delete.
        /// </summary>
        /// <param name="templateGuid">Template Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteTemplateResult>> Delete(string templateGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(templateGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "TemplateGuid is required",
                        Param = "TemplateGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteTemplateResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteTemplateResult deleteTemplate = new DeleteTemplateResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.DeleteTemplate);
                string formattedUrl = String.Format(accessTokenUrl, Constants.userVersion, templateGuid);
                var templatedelte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteTemplate).ReceiveJson<DataResponse<List<DeleteTemplateResult>>>();

                return new DataResponse<DeleteTemplateResult>(null)
                {
                    data = templatedelte.data.FirstOrDefault(),
                    message = templatedelte.message,
                    status = templatedelte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteTemplateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "Delete()");
                throw ex;
            }
        }

        /// <summary>
        /// Get the list of valid datatypes for attributes.
        /// </summary>        
        public async Task<DataResponse<List<AllDataTypeResult>>> DataType()
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.DataTypes);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                                              .GetJsonAsync<DataResponse<List<AllDataTypeResult>>>();

                return result;
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllDataTypeResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "DataType()");
                throw ex;
            }

        }

        /// <summary>
        /// To create a device template with attributes, use this. By using provided JSON format in request format, you can add a device template quickly.
        /// </summary>
        /// <param name="request">Add Quick Template Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddQuickTemplateResult>> Quick(AddQuickTemplateModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddQuickTemplateResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.QuickAdd);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addTempalte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddQuickTemplateResult>>>();

                return new DataResponse<AddQuickTemplateResult>(null)
                {
                    data = addTempalte.data.FirstOrDefault(),
                    message = addTempalte.message,
                    status = addTempalte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddQuickTemplateResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "Quick()");
                throw ex;
            }
        }

        /// <summary>
        /// Get the list of all available tags in template for given device template guid using this method.
        /// </summary>
        ///  <param name="deviceTemplateGuidOrCode">Device template Guid Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTagLookupResult>>> TagLookUp(string deviceTemplateGuidOrCode)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceTemplateGuidOrCode))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device TemplateGuid is required",
                        Param = "TemplateGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllTagLookupResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.TagLookup);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceTemplateGuidOrCode);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })                                         
                                         .GetJsonAsync<BaseResponse<List<AllTagLookupResult>>>();

                return new DataResponse<List<AllTagLookupResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true                    
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTagLookupResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "TagLookUp()");
                throw ex;
            }
        }
        #endregion

        #region Attribute Crud
        /// <summary>
        ///This Method allows you to create device template attributes.Attributes will be defined in your device template.One template can have one or more attributes. Your IoT devices will send the data for these template attributes.
        ///Here, localName and datatype are mandatory for any attribute, while a tag is mandatory for gateway supported templates only. The datatype of your attribute is predefined.If you select object datatype, you will be able to add child attributes for that attribute. Data validation allows you to set data range or specific data for attribute values.
        ///For Edge Enabled device, template must contain at least one attribute with datatype 'Number' to send data to Hub.
        /// </summary>
        /// <param name="request">Add template Request.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddAttributeResult>> AddAttribute(AddAttributeModel request)
        {
            try
            {

                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddAttributeResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AddAttribute);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addAttr = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddAttributeResult>>>();

                return new DataResponse<AddAttributeResult>(null)
                {
                    data = addAttr.data.FirstOrDefault(),
                    message = addAttr.message,
                    status = addAttr.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddAttributeResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AddAttribute()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method allows you to update the details of a device template attribute. For that, you need to send attributeGuid in method and attribute update details in model.        
        /// </summary>
        /// <param name="AttributeGuid">Attribute Guid.</param>
        /// <param name="request">Update Attribute Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateAttributeResult>> UpdateAttribute(string AttributeGuid, UpdateAttributeModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(AttributeGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "AttributeGuid is required",
                        Param = "AttributeGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<UpdateAttributeResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.UpdateAttribute);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, AttributeGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateAttributeResult>>>();

                return new DataResponse<UpdateAttributeResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateAttributeResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "UpdateAttribute()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Template Attribute.
        /// This Method allows you to delete a device template attribute. For that, you need to send attributeGuid in request url.
        /// </summary>
        /// <param name="attributeGuid">Attribute Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteAttributeResult>> DeleteAttribute(string attributeGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(attributeGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "AttributeGuid is required",
                        Param = "AttributeGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteAttributeResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteAttributeResult deleteTemplateAttribute = new DeleteAttributeResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.DeleteAttribute);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, attributeGuid);
                var templatedelte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteTemplateAttribute).ReceiveJson<DataResponse<List<DeleteAttributeResult>>>();

                return new DataResponse<DeleteAttributeResult>(null)
                {
                    data = templatedelte.data.FirstOrDefault(),
                    message = templatedelte.message,
                    status = templatedelte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteAttributeResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "DeleteAttribute()");
                throw ex;
            }
        }

        /// <summary>
        /// Get the list of available device templates of your company. Though filters are optional, you can add them as given in the parameters.To apply the filters, you need to pass these filters in query string.
        /// </summary>
        /// <param name="templateGuid">Template identifier.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AttributeResult>>> AllAttribute(string templateGuid, PagingModel pagingModel, string tag)
        {
            try
            {
                if (string.IsNullOrEmpty(templateGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Template Guid is required",
                        Param = "templateguid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AttributeResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AllAttribute);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, templateGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy,

                                         })
                                         .GetJsonAsync<BaseResponse<List<AttributeResult>>>();

                return new DataResponse<List<AttributeResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AttributeResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AllAttribute()");
                throw ex;
            }
        }


        #endregion

        #region TwinProperty Crud
        /// <summary>
        ///Device twins are used to synchronize state information between a device and an IoT hub.Device twin is a JSON document, associated with a specific device, and stored by IoT Hub in the cloud where you can query them.Device twin contains desired properties, reported properties, and tags.The desired property is set by a back-end application and read by a device.The reported property is set by a device and read by a back-end application.The tag is set by a back-end application and is never sent to a device.Tags are used to organize devices.
        ///You can add new twin properties using this Method.These properties will be defined in your device template and will be part of your device template.A single template can have one or more twin properties.
        ///Here name, localName, defaultValue and datatype are mandatory for any twin property.Data validation allows you to set data range or specific data for twin property values.
        /// </summary>
        /// <param name="request">Add Twin Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddTwinResult>> AddTwin(AddTwinModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddTwinResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AddTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addAttr = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddTwinResult>>>();

                return new DataResponse<AddTwinResult>(null)
                {
                    data = addAttr.data.FirstOrDefault(),
                    message = addAttr.message,
                    status = addAttr.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddTwinResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AddTwin()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method allows you to update a twin property. For that, you need to send settingGuid in request url along with the twin property update details in the request body.
        /// </summary>
        /// <param name="settingGuid">settingGuid Guid.</param>
        /// <param name="request">Update Twin Property Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateTwinResult>> UpdateTwin(string settingGuid, UpdateTwinPropertyModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateTwinResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.UpdateTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, settingGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateTwinResult>>>();

                return new DataResponse<UpdateTwinResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateTwinResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "UpdateTwin()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Template Attribute.
        /// you can delete a twin property using this Method. For that, you need to send settingGuid in request url.
        /// </summary>
        /// <param name="seetingGuid">Seeting Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteTwinResult>> DeleteTwin(string settingGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(settingGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "SettingGuid is required",
                        Param = "SettingGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteTwinResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteTwinResult deleteTemplateAttribute = new DeleteTwinResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.DeleteTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, settingGuid);
                var templatedelte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteTemplateAttribute).ReceiveJson<DataResponse<List<DeleteTwinResult>>>();

                return new DataResponse<DeleteTwinResult>(null)
                {
                    data = templatedelte.data.FirstOrDefault(),
                    message = templatedelte.message,
                    status = templatedelte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteTwinResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "DeleteTwin()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method gives you the list of available twin properties in your device template. For that, you need to send deviceTemplateGuid.
        ///You can also add the filters as given in the parameters.Though the filters are optional, to apply them, you need to send these filter parameters.
        /// </summary>
        /// <param name="templateGuid">Template Identifier.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTwinResult>>> AllTemplateTwin(string templateGuid, PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(templateGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Template Guid is required",
                        Param = "Templateguid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllTwinResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AllTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, templateGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy,
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllTwinResult>>>();

                return new DataResponse<List<AllTwinResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTwinResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AllTemplateTwin()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method provides you the list of available twin properties of a device. For that, you need to send deviceGuid.
        ///You can also add the filters as given in the parameters.Though the filters are optional, to apply them, you need to send these filter parameters.
        /// </summary>
        /// <param name="deviceGuid">Device Guid.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllTwinResult>>> AllDeviceTwin(string deviceGuid, PagingModel pagingModel)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "Device Guid is required",
                        Param = "deviceguid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<List<AllTwinResult>>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AllDeviceTwin);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, deviceGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy,
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllTwinResult>>>();

                return new DataResponse<List<AllTwinResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllTwinResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AllDeviceTwin()");
                throw ex;
            }
        }
        #endregion

        #region Command Crud
        /// <summary>
        ///Commands are used to manage a device remotely.You can add multiple commands to your device template. You can use commands to run a command on the device remotely, instantly, from IoTConnect.
        ///This Method allows you to create device template commands. These commands will be defined in your device template and will be a part of your device template. A template can have one or more commands.
        ///Here, name and command are mandatory for any template command.
        /// </summary>
        /// <param name="request">Add Command Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddCommandResult>> AddCommand(AddCommandModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddCommandResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AddCommand);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addAttr = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddCommandResult>>>();

                return new DataResponse<AddCommandResult>(null)
                {
                    data = addAttr.data.FirstOrDefault(),
                    message = addAttr.message,
                    status = addAttr.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddCommandResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AddCommand()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method lets you update a command. For that, you need to send commandGuid and command details.
        /// </summary>
        /// <param name="CommandGuid">Command Guid.</param>
        /// <param name="request">Update Command Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<UpdateCommandResult>> UpdateCommand(string CommandGuid, UpdateCommandModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<UpdateCommandResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.UpdateCommand);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, CommandGuid);
                var updateTemplate = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .PutJsonAsync(request).ReceiveJson<DataResponse<List<UpdateCommandResult>>>();

                return new DataResponse<UpdateCommandResult>(null)
                {
                    data = updateTemplate.data.FirstOrDefault(),
                    message = updateTemplate.message,
                    status = updateTemplate.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<UpdateCommandResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "UpdateCommand()");
                throw ex;
            }
        }

        /// <summary>
        /// Delete Command.
        /// This Method allows you to delete a device template attribute. For that, you need to send attributeGuid.
        /// </summary>
        /// <param name="commandGuid">Command Identifier.</param>
        /// <returns></returns>
        public async Task<DataResponse<DeleteCommandResult>> DeleteCommand(string commandGuid)
        {
            try
            {
                if (string.IsNullOrEmpty(commandGuid))
                {
                    List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                    ErrorItemModel errorItemModel = new ErrorItemModel()
                    {
                        Message = "CommandGuid is required",
                        Param = "CommandGuid"
                    };
                    errorItemModels.Add(errorItemModel);
                    return new DataResponse<DeleteCommandResult>(null)
                    {
                        errorMessages = errorItemModels,
                        status = false
                    };
                }
                DeleteCommandResult deleteTemplateAttribute = new DeleteCommandResult();
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.DeleteCommand);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, commandGuid);
                var templatedelte = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                 .SendJsonAsync(HttpMethod.Delete, deleteTemplateAttribute).ReceiveJson<DataResponse<List<DeleteCommandResult>>>();

                return new DataResponse<DeleteCommandResult>(null)
                {
                    data = templatedelte.data.FirstOrDefault(),
                    message = templatedelte.message,
                    status = templatedelte.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<DeleteCommandResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "DeleteCommand()");
                throw ex;
            }
        }

        /// <summary>
        /// This Method gives you the list of available commands in your device template. For that, you need to send deviceTemplateGuid.
        ///You can also add the filters as given in the parameters.Though the filters are optional, to apply them, you need to send these filter parameters in a query string.
        /// </summary>
        ///  <param name="templateGuid">Template Identifier.</param>
        /// <param name="request">Paging Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<List<AllCommandResult>>> AllTemplateCommand(string templateGuid, PagingModel pagingModel)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.AllTemplateCommand);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, templateGuid);
                var result = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                         .SetQueryParams(new
                                         {
                                             pageSize = pagingModel.PageSize,
                                             pageNumber = pagingModel.PageNo,
                                             searchtext = pagingModel.SearchText,
                                             sortBy = pagingModel.SortBy,
                                         })
                                         .GetJsonAsync<BaseResponse<List<AllCommandResult>>>();

                return new DataResponse<List<AllCommandResult>>(null)
                {
                    data = result.Data,
                    message = result.Message,
                    status = true
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<List<AllCommandResult>>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };
                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "AllTemplateCommand()");
                throw ex;
            }
        }

        /// <summary>
        ///Run a command on any device using this Method. For that, you need to send deviceGuid along with commandGuid and parameterValue.This command will be executed on a real device.Using this, you will be able to send data to devices from your UI.
        /// </summary>
        ///  <param name="devicetemplateGuid">Device Template Guid.</param>
        /// <param name="request">Command Execute Model.</param>
        /// <returns></returns>
        public async Task<DataResponse<AddCommandResult>> CommandExecute(string devicetemplateGuid, CommandExecuteModel request)
        {
            try
            {
                var errorList = Helper.ValidateObject(request);
                if (errorList?.Count > 0)
                {
                    return new DataResponse<AddCommandResult>(null)
                    {
                        errorMessages = errorList,
                        message = "Data Error",
                        status = false
                    };
                }
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.ExcuteCommand);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion, devicetemplateGuid);
                var addAttr = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddCommandResult>>>();

                return new DataResponse<AddCommandResult>(null)
                {
                    data = addAttr.data.FirstOrDefault(),
                    message = addAttr.message,
                    status = addAttr.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddCommandResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "CommandExecute()");
                throw ex;
            }
        }

        /// <summary>
        /// Execute Command.
        /// </summary>
        /// <param name="request">Command Execute Model</param>
        /// <returns></returns>
        public async Task<DataResponse<AddCommandResult>> CommandsExecute(CommandsExecuteModel request)
        {
            try
            {
                var portalApi = await _ioTConnectAPIDiscovery.GetPortalUrl(_envCode, _solutionKey, IoTConnectBaseURLType.DeviceBaseUrl);
                string accessTokenUrl = string.Concat(portalApi, TemplateApi.ExcuteCommands);
                string formattedUrl = String.Format(accessTokenUrl, Constants.deviceVersion);
                var addAttr = await formattedUrl.WithHeaders(new { Content_type = Constants.contentType, Authorization = Constants.bearerTokenType + _token })
                                                  .PostJsonAsync(request).ReceiveJson<DataResponse<List<AddCommandResult>>>();

                return new DataResponse<AddCommandResult>(null)
                {
                    data = addAttr.data.FirstOrDefault(),
                    message = addAttr.message,
                    status = addAttr.status
                };
            }
            catch (IoTConnectException ex)
            {
                List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
                errorItemModels.AddRange(ex.error);
                return new DataResponse<AddCommandResult>(null)
                {
                    errorMessages = errorItemModels,
                    message = ex.message,
                    status = false
                };                
            }
            catch (Exception ex)
            {
                await _ioTConnectAPIDiscovery.LoggedException(_envCode, ex, "Template", "CommandsExecute()");
                throw ex;
            }
        }
        #endregion
    }
}
