namespace BarakaBg.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BarakaBg.Data.Common.Repositories;
    using BarakaBg.Data.Models;
    using Xunit;

    public class VotesServiceTests
    {
        [Fact]
        public async Task WhenUserVotes2TimesOnly1VoteShouldBeCounted()
        {
            var repo = new FakeVotesRepository();
            var service = new VotesService(new FakeVotesRepository());

            await service.SetVoteASync(1, "1", 1);
            await service.SetVoteASync(1, "1", 5);
            await service.SetVoteASync(1, "1", 5);
            await service.SetVoteASync(1, "1", 5);
            await service.SetVoteASync(1, "1", 5);

            Assert.Equal(1, repo.All().Count());
            Assert.Equal(5, repo.All().First().Value);
        }
    }

    public class FakeVotesRepository : IRepository<Vote>
    {
        private readonly List<Vote> list = new List<Vote>();

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Vote> All()
        {
            return this.list.AsQueryable();
        }

        public IQueryable<Vote> AllAsNoTracking()
        {
            return this.list.AsQueryable();
        }

        public Task AddAsync(Vote entity)
        {
            this.list.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Vote entity)
        {
        }

        public void Delete(Vote entity)
        {
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }
    }
}