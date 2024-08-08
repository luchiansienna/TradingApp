namespace Ferovinum.Services.DTO
{
    public class PortfolioDTO
    {
        public float LifeToDateFeeNotional { get; set; }

        public float LifeToDateProductNotional { get; set; }

        public float OutstandingFeeNotional { get; set; }

        public float OutstandingProductNotional { get; set; }

        public float WeightedAverageRealisedAnnualisedYield { get; set; }

        public float WeightedAverageRealisedDuration { get; set; }
    }
}
