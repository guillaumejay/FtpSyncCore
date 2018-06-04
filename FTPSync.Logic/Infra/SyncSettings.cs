// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace FTPSync.Logic.Infra
{
    public interface IFTPSettings
    {
         string protocol { get; set; }
        string address { get; set; }
        string userName { get; set; }
        string password { get; set; }
        string directory { get; set; }
        string encryption { get; set; }
        string mode { get; set; }

    }

    public abstract class DefinitionFTP: IFTPSettings,IValidatableObject
    {
        public static readonly string ModeActive="Active";
        public static readonly string ModePassive= "Passive";
        public static readonly string EncryptionSSL = "SSL";
        public static readonly string EncryptionTLS = "TLS";
        public static readonly string EncryptionNone = "None";

        [Required]
        public string protocol { get; set; }
        [Required]
        public string address { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string directory { get; set; }
        [Required]
        public string mode { get; set; }
        public string encryption { get; set; }

        public static bool TryValidate(object obj, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                obj, context, results,
                validateAllProperties: true
            );
        }
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result=new List<ValidationResult>();
            ICollection<ValidationResult> lstvalidationResult;

            //bool valid = TryValidate(this, out lstvalidationResult);
            //if (!valid)
            //    result.AddRange(lstvalidationResult);
            var validValues = new List<string> { ModeActive, ModePassive };
            if (validValues.Contains(mode) == false)
            {
                result.Add(new ValidationResult($"mode must be one of {String.Join(",", validValues)}"));
            }
            validValues = new List<string> { "FTP", "SFTP" };
            if (validValues.Contains(protocol) == false)
            {
                result.Add(new ValidationResult($"protocol must be one of {String.Join(",", validValues)}"));
            }
            validValues = new List<string> { EncryptionNone,EncryptionSSL,EncryptionTLS };
            if (protocol == "FTP")
            {
                if (validValues.Contains(encryption) == false)
                {
                    result.Add(new ValidationResult($"encryption must be one of {String.Join(",", validValues)}"));
                }
            }
            return result;
        }
    }

    [DebuggerDisplay("{address} Delete {deleteFileAfterTransfer}")]
    public class SourceFTP: DefinitionFTP
    { 
        public bool deleteFileAfterTransfer { get; set; }
    }

    public class DestinationFTP:DefinitionFTP
    {
        public static readonly string IfExistsOverwrite = "Overwrite";
        public static readonly string IfExistsDontTransfer = "DontTransfer";

        public string actionIfFileExists { get; set; }


        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = base.Validate(validationContext).ToList();
            var validValues = new List<string> {IfExistsOverwrite,IfExistsDontTransfer};
            if (validValues.Contains(actionIfFileExists) == false)
            {
                result.Add(new ValidationResult($"actionIfFileExists must be one of {String.Join(",",validValues)}"));
            }

            return result;
        }
    }

    public class SyncSettings
    {
        public int serviceIntervalInMinutes { get; set; }
        public string changeFileNamePrepend { get; set; }
        public SourceFTP sourceFTP { get; set; }
        public DestinationFTP destinationFTP { get; set; }
        public List<ValidationResult> Validate()
        {
            var result = new List<ValidationResult>();
            result.AddRange(sourceFTP.Validate(null));
            result.AddRange(destinationFTP.Validate(null));
            //ICollection<ValidationResult> results;
            //if (DefinitionFTP.TryValidate(sourceFTP, out results))
            //{
            //    result.AddRange(results);
            //}
            //if (DefinitionFTP.TryValidate(destinationFTP, out results))
            //{
            //    result.AddRange(results);
            //}
            return result;
        }

        public static SyncSettings Load()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");
            var configuration = builder.Build();
            var settings = new SyncSettings();
            configuration.Bind(settings);
            return settings;
        }
    }
}
