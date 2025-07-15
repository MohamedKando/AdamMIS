

using AdamMIS.Contract.Reports;
using AdamMIS.Entities.ReportsEnitites;
using FastReport;

namespace AdamMIS.Mapping
{
    public class MappingConfugrations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<Reports, ReportResponse>().Map(dest => dest.CategoryName, src => src.Category.Name);

            config.NewConfig<UserReports, UserReportResponse>()
            .Map(dest => dest.UserName, src => src.User.UserName)

            .Map(dest => dest.ReportFileName, src => src.Report.FileName)
            .Map(dest => dest.CategoryName, src => src.Report.Category.Name);
        }
    }
}
