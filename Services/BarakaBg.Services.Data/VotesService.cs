namespace BarakaBg.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;

    public class VotesService : IVotesService
    {
        private readonly IRepository<Vote> votesRepository;

        public VotesService(IRepository<Vote> votesRepository)
        {
            this.votesRepository = votesRepository;
        }

        public async Task SetVoteASync(int productId, string userId, byte value)
        {
            var vote = this.votesRepository
                .All()
                .FirstOrDefault(x => x.ProductId == productId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Vote
                {
                    ProductId = productId,
                    UserId = userId,
                };

                await this.votesRepository.AddAsync(vote);
            }

            vote.Value = value;
            await this.votesRepository.SaveChangesAsync();
        }

        public double GetAverageVotes(int productId)
        {
            var averageVotes = this.votesRepository
                .All()
                .Where(x => x.ProductId == productId)
                .Average(x => x.Value);

            return averageVotes;
        }
    }
}