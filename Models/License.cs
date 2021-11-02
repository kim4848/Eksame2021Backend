using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace License.Models
{
    public class License
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]

        public string LicenseId { get; set; }
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
        public IncludedFeaturesWrapper IncludedFeatures { get; set; }
        public AddonInfoWrapper AddonInfos { get; set; }


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
        public List<StandardFeature> StandardFeatures { get; set; } = new List<StandardFeature>();
        public List<EsdhIntegration> EsdhIntegrations { get; set; } = new List<EsdhIntegration>();
        public List<Addon> Addons { get; set; } = new List<Addon>();
        public List<LookupIntegration> LookupIntegrations { get; set; } = new List<LookupIntegration>();
        public List<OutputManagementIntegration> OutputManagementIntegrations { get; set; } = new List<OutputManagementIntegration>();
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

    public enum StandardFeature
    {
        [Description("ComboBlocks")]
        ComboBlock = 1, // Tidligere ConcatenatePhrases
        [Description("OneClick templates")]
        OneClickDocuments = 2,
        [Description("Printer tray selection")]
        PrinterTraySelection = 3,
        [Description("Resource management")]
        ResourceAdministration = 4,
        [Description("Send as PDF")]
        SendAsPdf = 5,
        [Description("Signature management")]
        SignatureAdministration = 6,
        [Description("Standard Content")]
        StandardContent = 7, // Tidligere Phrases
        [Description("Standard Content creation")]
        StandardContentCreation = 8, // Tidligere PhraseCreation
        [Description("Templates")]
        Templates = 9,
        [Description("Local user profiles")]
        UserAdministration = 10,
        [Description("Offline mode")]
        Offline = 11
    }

    public enum EsdhIntegration
    {
        [Description("Abakion Legal")]
        AbakionLegal = 0,
        [Description("Acadre")]
        AcadreIntegration = 1,
        [Description("Acos WebSak")]
        AcosWebSakIntegration = 3,
        [Description("cBrain F2")]
        cBrainF2Integration = 4,
        [Description("Ciceron")]
        CiceronIntegration = 5,
        [Description("Columna Cura")]
        ColumnaCuraIntegration = 6,
        [Description("CSC Vitae")]
        CSCVitaeIntegration = 7,
        [Description("D4")]
        D4Phrases = 8,
        [Description("DUBU 3")]
        DUBU3Integration = 10,
        [Description("Dynamics CRM")]
        DynamicsCRMIntegration = 11,
        [Description("Dynamics CRM (udvidet GI)")]
        DynamicsCRMIntegrationExtendedGI = 12,
        [Description("eDoc")]
        eDocIntegration = 13,
        [Description("EDP Vision")]
        EDPVision = 14,
        [Description("EG Sensum")]
        EGSensumCCIntegration = 15,
        [Description("EG Sensum (Deprecated)")]
        EGSensumIntegration = 16,
        [Description("Elements")]
        ElementsIntegration = 17,
        [Description("ePhorte")]
        ePhorteIntegration = 18,
        [Description("Evry ESA")]
        EvryESAIntegration = 19,
        [Description("FICS")]
        FICSIntegration = 20,
        [Description("GetOrganized")]
        GetOrganizedIntegration = 21,
        [Description("Idox Uniform")]
        IdoxUniformIntegration = 22,
        [Description("Information@Work")]
        InformationAtWorkIntegration = 23,
        [Description("KMD Care")]
        KMDCareIntegration = 24,
        [Description("KMD Momentum")]
        KMDMomentum = 25,
        [Description("KMD Nexus")]
        KMDNexusIntegration = 26,
        [Description("KMD Nova")]
        KMDNovaIntegration = 27,
        [Description("KMD Sag")]
        KMDSagIntegration = 28,
        [Description("Modus")]
        ModusIntegration = 29,
        [Description("Novax")]
        NovaxIntegration = 30,
        [Description("Platina")]
        PlatinaIntegration = 31,
        [Description("Profile")]
        ProfileIntegration = 32,
        [Description("Public 360")]
        Public360Integration = 33,
        [Description("SBSYS")]
        SBSYSIntegration = 35,
        [Description("TK2")]
        TK2Integration = 36,
        [Description("W3D3")]
        W3D3Integration = 37,
        [Description("Custom XML file (configurable)")]
        XMLFileIntegration = 38,
    }

    public enum Addon
    {
        [Description("Cloud Statistics access")]
        CloudStatistics = 1,
        [Description("Batchlookup of danish CPR/CVR/P-no. in merge template")]
        MergeSourceLookup = 2,
        [Description("Prepare any Excel sheet for merging")]
        MergeSourcePrepare = 3,
        [Description("Rule field (Remember to maintain 'Holidays.xml' to calculate working days)")]
        RuleField = 4,
        [Description("DynamicTemplate Cloud")]
        DtCloud = 5
    }

    public enum LookupIntegration
    {
        [Description("CVR Online")]
        CvrOnline = 0,
        [Description("P-Data")]
        PData = 1,
        [Description("Serviceplatformen CVR Online 3.0")]
        SPCvrOnline = 2,
        [Description("Serviceplatformen FAMILIE+")]
        SpFamiliePlus = 3,
        [Description("Serviceplatformen Person Stamdata udvidet")]
        SpPersonStamdata = 4,
        [Description("Serviceplatformen STAM+")]
        SpStamPlus = 5,
        [Description("Custom SQL database (configurable)")]
        SqlDB = 6
    }

    public enum OutputManagementIntegration
    {
        [Description("Convergens I/O Manager")]
        ConvergensIOManager = 0,
        [Description("OneTooX")]
        Doc2Mail = 1,
        [Description("Mina Meddelanden")]
        MinaMeddelanden = 2,
        [Description("Print via Serviceplatformen")]
        PrintViaServiceplatformen = 3,
        [Description("Send og journaliser")]
        SBSend = 4,
        [Description("SBSIP")]
        SBSIP = 5,
        [Description("Strålfors Connect")]
        StraalforsConnect = 6,
        [Description("KS SvarUT")]
        SvarUt = 7
    }

    #endregion
}