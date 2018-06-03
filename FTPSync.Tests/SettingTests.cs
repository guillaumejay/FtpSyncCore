using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FTPSync.Logic.Infra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace FTPSync.Tests
{
    /// <summary>
    /// Basic tests for settings validity
    /// </summary>
    [TestClass]
    public class SettingTests
    {
        [TestMethod]
        public void IfExistsHasValidValue()
        {
            DestinationFTP settings =
                CreateDefaultSettings();
            settings.actionIfFileExists = "test";
            var validate = settings.Validate(null);
            Check.That(validate).CountIs(1);
            settings.actionIfFileExists = DestinationFTP.IfExistsDontTransfer;
            validate = settings.Validate(null);
            Check.That(validate).CountIs(0);
            settings.actionIfFileExists = DestinationFTP.IfExistsOverwrite;
            validate = settings.Validate(null);
            Check.That(validate).CountIs(0);
        }

        private static DestinationFTP CreateDefaultSettings()
        {
            return new DestinationFTP
            {
                actionIfFileExists = DestinationFTP.IfExistsOverwrite,
                mode = DestinationFTP.ModeActive,
                protocol = "FTP",
                address = "localhost",
                encryption = "None"
            };
        }

        [TestMethod]
        public void ModeHasValidValue()
        {
            DestinationFTP settings = CreateDefaultSettings();
            settings.mode = "test";
            ICollection<ValidationResult> validate;
            DestinationFTP.TryValidate(settings, out validate);
            Check.That(validate).CountIs(1);
            settings.mode = DefinitionFTP.ModeActive;
            DestinationFTP.TryValidate(settings, out validate);
            Check.That(validate).CountIs(0);
            settings.mode = DefinitionFTP.ModePassive;
            DestinationFTP.TryValidate(settings, out validate);
            Check.That(validate).CountIs(0);
        }
        [TestMethod]
        public void AddressIsRequired()
        {
            DestinationFTP settings = CreateDefaultSettings();
            settings.address = null;
            ICollection<ValidationResult> result;
            DefinitionFTP.TryValidate(settings, out result);
            Check.That(result).CountIs(1);
        }
        [TestMethod]
        public void EncryptionMustBeValid()
        {
            DestinationFTP settings = CreateDefaultSettings();
            settings.encryption = null;
            ICollection<ValidationResult> result;
            DefinitionFTP.TryValidate(settings, out result);
            Check.That(result).CountIs(1);
            settings.encryption = "TLS";
            DefinitionFTP.TryValidate(settings, out result);
            Check.That(result).CountIs(0);
            settings.encryption = "SSL";
            DefinitionFTP.TryValidate(settings, out result);
            Check.That(result).CountIs(0);
        }
    }
}
