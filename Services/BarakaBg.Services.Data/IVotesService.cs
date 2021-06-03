namespace BarakaBg.Services.Data
{
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task SetVoteASync(int productId, string userId, byte value);

        double GetAverageVotes(int productId);
    }
}