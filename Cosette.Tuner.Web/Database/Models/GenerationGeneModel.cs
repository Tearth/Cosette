namespace Cosette.Tuner.Web.Database.Models
{
    public class GenerationGeneModel : GeneModel
    {
        public int GenerationId { get; set; }
        public GenerationModel Generation { get; set; }
    }
}
