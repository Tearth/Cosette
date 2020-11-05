namespace Cosette.Tuner.Web.Database.Models
{
    public class ChromosomeGeneModel : GeneModel
    {
        public int ChromosomeId { get; set; }
        public virtual GenerationModel Chromosome { get; set; }
    }
}
