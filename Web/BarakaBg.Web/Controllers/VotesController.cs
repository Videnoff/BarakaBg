namespace BarakaBg.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BarakaBg.Services.Data;
    using BarakaBg.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : BaseController
    {
        private readonly IVotesService votesService;

        public VotesController(IVotesService votesService)
        {
            this.votesService = votesService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostVoteResponseModel>> Post(PostVoteInputModel inputModel)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.votesService.SetVoteASync(inputModel.ProductId, userId, inputModel.Value);

            var averageVotes = this.votesService.GetAverageVotes(inputModel.ProductId);

            return new PostVoteResponseModel
            {
                AverageVote = averageVotes,
            };
        }
    }
}
