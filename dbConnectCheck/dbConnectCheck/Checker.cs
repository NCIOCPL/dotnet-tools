using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
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
                ConnectionStringSettingsCollection connections = LoadConfigurationFile(args[0]);
                CheckConnectionStrings(connections);
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
        static void CheckConnectionStrings(ConnectionStringSettingsCollection connectionList)
        {
            SqlConnection conn;
            string successMessageFmt = "Connection {0} is valid.";
            string failedMessageFmt = "Connection {0} failed. {1}";

            bool hasErrors = false;
            List<string> errorMessages = new List<string>();

            // Find all connection strings which have been added to this file.
            foreach (ConnectionStringSettings connection in connectionList)
            {
                string name = string.Empty;
                string connectString = string.Empty;

                try
                {
                    name = connection.Name;
                    connectString = connection.ConnectionString;

                    using (conn = new SqlConnection(connectString))
                    {
                        try
                        {
                            conn.Open();
                            Console.WriteLine(string.Format(successMessageFmt, name));
                        }
                        catch (Exception ex)
                        {
                            hasErrors = true;
                            errorMessages.Add(string.Format(failedMessageFmt, name, ex.Message));
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    hasErrors = true;
                    errorMessages.Add(string.Format(failedMessageFmt, name, ex.Message));
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
        /// Loads the specified configuration file.
        /// 
        /// Throws NotAConfigurationFileException if the file is not a valid configuration file.
        /// </summary>
        /// <param name="filepath">Name and optional path of the configuration file to be loaded.</param>
        /// <returns>An XmlDocument object containing the </returns>
        static ConnectionStringSettingsCollection LoadConfigurationFile(string filepath)
        {
            const string APP_CONFIG_KEY = "APP_CONFIG_FILE";

            Object oldConfigFile = AppDomain.CurrentDomain.GetData(APP_CONFIG_KEY);

            ConnectionStringSettingsCollection returnedConnections = new ConnectionStringSettingsCollection();

            try
            {
                /// ConfigurationManager.OpenExeConfiguration(string) wants the filename of
                /// an application to load from (and appends ".config"). Using that with an arbitrary
                /// config file results in an error because it tries to load "app.config.config" and leaving out
                /// the extension results in an error that the exepath isn't correctly formatted.
                /// 
                /// Trying to use WebConfigurationManager.OpenWebConfiguration() likewise fails, because it expects
                /// a *virtual* path (i.e. A URL hosted on the current machine). This precludes checking configuration
                /// files for anything but a web.config, and only allows this application to be run on a web server.
                /// 
                /// ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel) can be used by changing the
                /// name of the application's configuration file.  See http://stackoverflow.com/a/14246260/282194
                /// This is OK, as this application doesn't have a configuration file of its own.

                // Preserve old config file name in case the above assumption changes.
                AppDomain.CurrentDomain.SetData(APP_CONFIG_KEY, filepath);
                Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!conf.HasFile)
                    throw new ConfigurationFileException(string.Format("Cannot load configuration file {0}.", filepath));

                // Even if the connectionStrings section is missing from the file, there may be
                // default connections inherited from the machine.config. These can be identified by
                // the ElementInformation's property's IsPresent flag.
                ConnectionStringsSection sect = conf.ConnectionStrings;

                // Strip out .Net default connection strings.
                foreach (ConnectionStringSettings  connString in sect.ConnectionStrings)
                {
                    if (connString.ElementInformation.IsPresent)
                        returnedConnections.Add(connString);
                }
            }
            finally
            {
                // Restore old config file name.
                AppDomain.CurrentDomain.SetData(APP_CONFIG_KEY, oldConfigFile);
            }

            return returnedConnections;
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
