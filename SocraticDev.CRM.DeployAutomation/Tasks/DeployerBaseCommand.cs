using Microsoft.Xrm.Sdk;
using SocraticDev.CRM.DeployAutomation.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace SocraticDev.CRM.DeployAutomation.Tasks
{
    public abstract class DeployerBaseCommand<T> : ICommand
    {
        protected T Settings
        {
            get;
            private set;
        }

       
        protected IOrganizationService Service
        {
            get;
            private set;
        }

        protected DeployerBaseCommand(IOrganizationService organizationService)
        {
            Service = organizationService;
            Settings = GetSettings();
        }

        protected DeployerBaseCommand(IOrganizationService organizationService, T settings)
        {
            Service = organizationService;
            Settings = settings;
        }


        public T GetSettings()
        {
            try
            {
                string filePath;

                if (ConfigurationManager.AppSettings.AllKeys.Contains("ConfigurationPath"))
                {
                    filePath = ConfigurationManager.AppSettings["ConfigurationPath"];
                }
                else
                {
                    filePath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".config");
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                var declaringType = GetType();
                {
                    var settingsType = declaringType.Name;

                    var configData = xmlDoc.SelectSingleNode(String.Format("/Settings/{0}", settingsType));

                    using (StringReader sr = new StringReader(configData.InnerXml))
                    {
                        return (T)new XmlSerializer(typeof(T)).Deserialize(sr);
                    }
                }

                throw new Exception("Could not determine type to get settings for");
            }
            catch (ConfigurationErrorsException ex)
            {
                throw new ConfigurationErrorsException("Make sure ConfigurationPath is defined in your app.config", ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException(
                    "Please check to make sure the setting ConfigurationPath is set properly in your app.config file.",
                    ex);
            }
            catch (XPathException ex)
            {
                throw new XPathException("Please make sure that your ConfigurationFile is properly formatted", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Could not deserialize the Settings", ex);
            }
        }


        public abstract void Execute();
    }
}
