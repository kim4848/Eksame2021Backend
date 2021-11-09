using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace License.Models
{
    public class License
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Partitionkey { get; set; } = "DynamicTemplate";
        public string CompanyName { get; set; }
        public string CustomerId { get; set; }
        public string DomainName { get; set; }
        public bool DisableAdDomainCheck { get; set; }
        public bool Deactivated { get; set; }
        public string ExpirationDate { get; set; }
        public string SupportEmail { get; set; }
        public int AllowedClientUsers { get; set; }
        public int AllowedCloudUsers { get; set; }
        public string CloudStatisticsKey { get; set; }
        public IncludedFeaturesWrapper IncludedFeatures { get; set; } = new IncludedFeaturesWrapper();
        public AddonInfoWrapper AddonInfos { get; set; } = new AddonInfoWrapper();


        public static List<FeatureDTO<T>> GetAllFeatures<T>() where T : System.Enum
        {
            var result = new List<FeatureDTO<T>>();

            var values = typeof(T).GetEnumValues().Cast<T>();

            foreach (var item in values)
            {
                result.Add(new FeatureDTO<T>()
                {
                    Id = item,
                    Name = GetEnumDescription(item)
                });
            }

            return result;
        }

        public static string GetEnumDescription<T>(T value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var desc = (fi.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute)?.Description;

            if (!string.IsNullOrWhiteSpace(desc))
                return desc;
            else
                return value.ToString();
        }
    }

    #region Feature

    public class IncludedFeaturesWrapper
    {
        public StandardFeature StandardFeatures { get; set; } = new StandardFeature();
        public EsdhIntegration EsdhIntegrations { get; set; } = new EsdhIntegration();
        public Addon Addons { get; set; } = new Addon();
        public LookupIntegration LookupIntegrations { get; set; } = new LookupIntegration();
        public OutputManagementIntegration OutputManagementIntegrations { get; set; } = new OutputManagementIntegration();
    }

    public abstract class FeatureDTOBase
    {
        public string Name { get; set; }
    }

    public class FeatureDTO<T> : FeatureDTOBase
    {
        public T Id { get; set; }
    }

    #endregion

    #region Addon info

    public class AddonInfoWrapper
    {
        public DtCloudInfo DtCloud { get; set; }
    }

    public class DtCloudInfo
    {
        public string PortalBaseUri { get; set; }
    }

    #endregion

    #region Enums

    public class StandardFeature
    {

        public bool ComboBlock { get; set; }
        public bool PrinterTraySelection { get; set; }
        public bool ResourceAdministration { get; set; }
        public bool SendAsPdf { get; set; }
        public bool SignatureAdministration { get; set; }
        public bool StandardContent { get; set; }
        public bool StandardContentCreation { get; set; }
        public bool Templates { get; set; }
        public bool UserAdministration { get; set; }
        public bool Offline { get; set; }
    }

    public class EsdhIntegration
    {
        public bool AbakionLegal { get; set; }
        public bool AcadreIntegration { get; set; }
        public bool ComboBlock { get; set; }
        public bool AcosWebSakIntegration { get; set; }
        public bool cBrainF2Integration { get; set; }
        public bool CiceronIntegration { get; set; }
        public bool ColumnaCuraIntegration { get; set; }
        public bool KMDMomentum { get; set; }
        public bool NovaxIntegration { get; set; }
        public bool SBSYSIntegration { get; set; }

    }

    public class Addon
    {
        public bool CloudStatistics { get; set; }
        public bool MergeSourceLookup { get; set; }
        public bool MergeSourcePrepare { get; set; }
        public bool RuleField { get; set; }
        public bool SBSYSIntegration { get; set; }
        public bool DtCloud { get; set; }

    }

    public class LookupIntegration
    {
        public bool CvrOnline { get; set; }
        public bool PData { get; set; }
        public bool SPCvrOnline { get; set; }
        public bool SpFamiliePlus { get; set; }
        public bool SpPersonStamdata { get; set; }

    }

    public class OutputManagementIntegration
    {
        public bool ConvergensIOManager { get; set; }
        public bool Doc2Mail { get; set; }
        public bool SpPersonStamdata { get; set; }
        public bool MinaMeddelanden { get; set; }
        public bool PrintViaServiceplatformen { get; set; }
        public bool SBSend { get; set; }
        public bool StraalforsConnect { get; set; }
        public bool SvarUt { get; set; }

    }

    #endregion
}