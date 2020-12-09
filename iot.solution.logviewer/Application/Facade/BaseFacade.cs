using component.logger.data.log.Model;
using component.logger.data.log.Request;
using component.logger.platform.common.Enums;
using component.services.logger.viewer.Common.Extension;
using iot.solution.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace component.services.logger.viewer.Application.Facade
{
    /// <summary>
    /// BaseFacade
    /// </summary>
    public abstract class BaseFacade
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFacade" /> class.
        /// </summary>
        protected BaseFacade()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFacade" /> class.
        /// </summary>
        /// <param name="invokingUserId">The invoking user identifier.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <param name="version">The version.</param>
        protected BaseFacade(string invokingUserId, string applicationCode, string version) : this()
        {
            Guid invokingUserIdResult;
            if (String.IsNullOrWhiteSpace(invokingUserId)) RaiseException(ResponseStatus.Business_EmptyData, "UserId", true);
            if (!Guid.TryParse(invokingUserId, out invokingUserIdResult)) RaiseException(ResponseStatus.Business_InvalidData, "UserId", true);
            InvokingUserId = invokingUserIdResult;

            if (String.IsNullOrWhiteSpace(applicationCode)) RaiseException(ResponseStatus.Business_EmptyData, "applicationCode", true);

            if (string.IsNullOrEmpty(applicationCode))
                RaiseException(ResponseStatus.Business_InvalidData, "applicationCode", true);

            ApplicationCode = applicationCode;

            if (String.IsNullOrWhiteSpace(version)) RaiseException(ResponseStatus.Business_EmptyData, "Version", true);
            Version = version;
        }

        #region Variables
        /// <summary>
        /// Gets or sets the invoking user identifier.
        /// </summary>
        /// <value>
        /// The invoking user identifier.
        /// </value>
        public Guid InvokingUserId { get; set; }

        /// <summary>
        /// The application code
        /// </summary>
        public string ApplicationCode;

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        protected string Version { get; set; }

        /// <summary>
        /// Gets or sets the okta user identifier.
        /// </summary>
        /// <value>
        /// The okta user identifier.
        /// </value>
        public string OktaUserId { get; set; }

        /// <summary>
        /// The UI message
        /// </summary>
        public static Dictionary<string, string> UiMessage = new Dictionary<string, string>();
        #endregion

        /// <summary>
        /// Raises the exception.
        /// </summary>
        /// <param name="status">The status.</param>
        public void RaiseException(ResponseStatus status)
        {
            throw new GenericCustomException(status.ToDescription()) { ErrorCode = status.ToString() };
        }

        /// <summary>
        /// Raises the exception.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="message">The message.</param>
        /// <param name="isEnum">if set to <c>true</c> [is enum].</param>
        /// </exception>
        public void RaiseException(ResponseStatus status, string message, bool isEnum = false)
        {
            if (isEnum)
                throw new GenericCustomException(status.ToDescription(), message) { ErrorCode = status.ToString() };
            else
                throw new GenericCustomException(message) { ErrorCode = status.ToString() };
        }

        /// <summary>
        /// Validates the specified lr.
        /// </summary>
        /// <param name="lr">The lr.</param>
        public void Validate(ListRequest lr)
        {
            Validate(lr, null);
        }

        /// <summary>
        /// Validates the specified lr.
        /// </summary>
        /// <param name="lr">The lr.</param>
        /// <param name="type">The type.</param>
        public void Validate(ListRequest lr, Type type)
        {
            if (lr.PageSize <= 0)
                throw new InvalidDataException("Invalid Pagesize");

            if (lr.PageNumber <= 0)
                throw new InvalidDataException("Invalid Pageno");

            if (!string.IsNullOrWhiteSpace(lr.OrderBy) && type != null)
            {
                if (string.IsNullOrWhiteSpace(lr.OrderBy))
                    throw new InvalidDataException("Invalid Orderby");
            }
        }

        /// <summary>
        /// Validates the response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        public void ValidateResponse<T>(DataResponse<T> response)
        {
            if ((ResponseStatus)response.Status != ResponseStatus.Success)
            {
                if (response.Extension != null)
                {
                    string messageCode = Convert.ToString(response.Extension["fieldname"]);
                    if (response.Extension.ContainsKey("invalidvalue") && !string.IsNullOrWhiteSpace(Convert.ToString(response.Extension["invalidvalue"])))
                        RaiseException((ResponseStatus)response.Status, GetUiMessage(messageCode) + response.Extension["invalidvalue"]);
                    else if (response.Extension.ContainsKey("ismessagecode") && !string.IsNullOrWhiteSpace(Convert.ToString(response.Extension["fieldname"])))
                        RaiseException((ResponseStatus)response.Status, GetUiMessage(messageCode));
                    else if (string.IsNullOrWhiteSpace(messageCode))
                        RaiseException((ResponseStatus)response.Status);
                    else
                        RaiseException((ResponseStatus)response.Status, GetUiMessage(messageCode));
                }
            }
        }

        /// <summary>
        /// Fires the validation.
        /// </summary>
        /// <param name="messageCode">The message code.</param>
        /// <param name="status">The status.</param>
        public void FireValidation(string messageCode, ResponseStatus status)
        {
            RaiseException(status, GetUiMessage(messageCode));
        }

        /// <summary>
        /// Gets the UI message.
        /// </summary>
        /// <param name="messageCode">The message code.</param>
        /// <returns></returns>
        public string GetUiMessage(string messageCode)
        {
            return messageCode;
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static string GenerateXML(object log)
        {
            return GenerateXML(log, false);
        }

        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="isInnerXml">if set to <c>true</c> [is inner XML].</param>
        /// <returns></returns>
        public static string GenerateXML(object log, bool isInnerXml)
        {
            string Doc = string.Empty;
            XmlSerializerNamespaces emptyXML = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializerXML = new XmlSerializer(log.GetType());
            XmlWriterSettings settingsWriter = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (StringWriter stream = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(stream, settingsWriter))
            {
                serializerXML.Serialize(writer, log, emptyXML);
                Doc = stream.ToString();
                if (isInnerXml)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(Doc);
                    Doc = xmlDocument.DocumentElement.InnerXml;
                }
            }
            return Doc;
        }
    }
}
