namespace BarakaBg.Services.Data
{
    using BarakaBg.Services.Data.Models;
    using BarakaBg.Web.ViewModels.Home;

    public interface IGetCountsService
    {
        CountsDto GetCounts();
    }
}