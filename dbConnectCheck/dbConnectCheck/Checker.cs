using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace dbConnectCheck
{
    class Checker
    {
        static int Main(string[] args)
        {
            int rc = 0;

            try
            {
                Validate(args);
                XmlDocument config = LoadConfigurationFile(args[0]);
                CheckConnectionStrings(config);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                rc = 1;
            }

            return rc;
        }

        /// <summary>
        /// Find and test all the database connections.
        /// </summary>
        /// <param name="config"></param>
        static void CheckConnectionStrings(XmlDocument config)
        {
            SqlConnection conn;
            string successMessageFmt = "Connection {0} is valid.";
            string failedMessageFmt = "Connection {0} failed.";

            bool hasErrors = false;
            List<string> errorMessages = new List<string>();

            // Find all connection strings which have been added to this file.
            XmlNodeList connectionList = config.SelectNodes("//connectionStrings/add");
            foreach (XmlNode connection in connectionList)
            {
                string name = string.Empty;
                string connectString = string.Empty;

                try
                {
                    ValidateRequiredAttribute(connection, "name");
                    ValidateRequiredAttribute(connection, "connectionString");

                    name = connection.Attributes["name"].Value;
                    connectString = connection.Attributes["connectionString"].Value;

                    using (conn = new SqlConnection(connectString))
                    {
                        try
                        {
                            conn.Open();
                            Console.WriteLine(string.Format(successMessageFmt, name));
                        }
                        catch (Exception)
                        {
                            hasErrors = true;
                            errorMessages.Add(string.Format(failedMessageFmt, name));
                        }
                    }
                }
                catch (ArgumentException)
                {
                    hasErrors = true;
                    errorMessages.Add(string.Format(failedMessageFmt, name));
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                    errorMessages.Add(ex.Message);
                }
            }

            if (hasErrors)
            {
                string bigMessage = string.Empty;
                errorMessages.ForEach(message => {
                    if (!string.IsNullOrEmpty(bigMessage))
                        bigMessage += "\n";
                    bigMessage += message;
                });

                throw new ConnectionStringErrorException(bigMessage);
            }
        }

        /// <summary>
        /// Validate that a required attribute (e.g. "name" or "connectionString") is present in
        /// a connection definition.
        /// </summary>
        /// <param name="connection">XmlNode containing a connection defition (the <add/> element)</param>
        /// <param name="attribute">Attribute to check for.</param>
        static void ValidateRequiredAttribute(XmlNode connection, string attribute)
        {
            string missingAttributeMessageFmt = "Required attribute '{0}' is missing from: {1}.";
            if (connection.Attributes[attribute] == null)
                throw new ConnectionStringErrorException(string.Format(missingAttributeMessageFmt, attribute, connection.OuterXml));
        }


        /// <summary>
        /// Loads the specified configuration file.
        /// 
        /// Throws NotAConfigurationFileException if the file is not a valid configuration file.
        /// </summary>
        /// <param name="filepath">Name and optional path of the configuration file to be loaded.</param>
        /// <returns>An XmlDocument object containing the </returns>
        static XmlDocument LoadConfigurationFile(string filepath)
        {
            string badConfigMessageFmt = "{0} does not appear to be a valid configuration file.";

            XmlDocument config = new XmlDocument();
            try
            {
                config.Load(filepath);
                if(config.DocumentElement.Name != "configuration")
                    throw new NotAConfigurationFileException(string.Format(badConfigMessageFmt, filepath));
            }
            catch (XmlException ex)
            {
                throw new NotAConfigurationFileException(string.Format(badConfigMessageFmt, filepath), ex);
            }
            return config;
        }

        /// <summary>
        /// Validates that the user passed a file name to check.
        /// </summary>
        /// <param name="argList">The program's argument list</param>
        static void Validate(string[] argList)
        {
            #region Error Messages
            String noFileSpecifiedMessage =
@"dbConnectCheck <config-file-name>

    config-file-name is the name (and optional path) of an
                     application configuration file.";
            String fileNotFoundMessageFmt = "Unable to find file {0}.";
            #endregion

            if (argList.Length != 1)
            {
                throw new ValidationException(noFileSpecifiedMessage);
            }

            if(!File.Exists(argList[0]))
            {
                throw new ValidationException(string.Format(fileNotFoundMessageFmt, argList[0]));
            }
        }

        /// <summary>
        /// Displays an exception containing an error.
        /// </summary>
        /// <param name="ex">An exception object</param>
        static void DisplayError(Exception ex)
        {
            ConsoleColor origColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(ex);
            Console.ForegroundColor = origColor;
        }

        static void DisplayError(String error)
        {
            ConsoleColor origColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(error);
            Console.ForegroundColor = origColor;
        }
    }
}
