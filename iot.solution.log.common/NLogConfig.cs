using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace component.logger.platform.common
{
    /// <summary>
    /// NLogConfig
    /// </summary>
    public static class NLogConfig
    {
        /// <summary>
        /// Sets the log target.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="internalLogging">if set to <c>true</c> [internal logging].</param>
        public static void SetLogTarget(string connectionString, bool internalLogging = false)
        {
            try
            {
                string tableName = "ErrorLog";

                string nLogQuery = $"INSERT INTO {tableName} ([ApplicationCode],[ComponentStatus],[ErrorCode],[Exception],[LogFile],[Identity],[LogDate],[Logger],[Message],[MessageData],[Method],[Severity],[StackTrace]) " +
                    $" VALUES (@ApplicationCode, @ComponentStatus, @ErrorCode, @Exception, @LogFile, @Identity, @LogDate, @Logger, @Message, @MessageData, @Method, @Severity, @StackTrace)";

                DatabaseTarget dbLog = new DatabaseTarget(Constants.NLOG_DB_CONFIG)
                {
                    CommandText = nLogQuery,
                    CommandType = System.Data.CommandType.Text,
                    ConnectionString = connectionString
                };
                //dbLog.Parameters.Add(new DatabaseParameterInfo("@LogId", "${gdc:LogId}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@ApplicationCode", "${gdc:ApplicationCode}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@ComponentStatus", "${gdc:ComponentStatus}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@ErrorCode", "${gdc:ErrorCode}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Exception", "${gdc:Exception}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@LogFile", "${gdc:LogFile}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Identity", "${gdc:Identity}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@LogDate", "${gdc:LogDate}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Logger", "${gdc:Logger}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Message", "${gdc:Message}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@MessageData", "${gdc:MessageData}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Method", "${gdc:Method}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@Severity", "${gdc:Severity}"));
                dbLog.Parameters.Add(new DatabaseParameterInfo("@StackTrace", "${gdc:StackTrace}"));

                LoggingConfiguration config = LogManager.Configuration;
                if (config == null)
                    config = new LoggingConfiguration();
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, LogLevel.Fatal, dbLog));

                if (internalLogging)
                {
                    InternalLogger.LogFile = "nlog.txt";
                    InternalLogger.LogWriter = new StringWriter();
                    InternalLogger.LogLevel = LogLevel.Trace;
                }

                LogManager.Configuration = config;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
