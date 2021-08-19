namespace BarakaBg.Web.ViewModels.Products
{
    using System;

    public class ProductRatingViewModel
    {
        public double AverageVote { get; set; }

        public double AverageVoteRounded => Math.Round(this.AverageVote * 2, MidpointRounding.AwayFromZero) / 2;
    }
}
