using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Web.Database.Models;
using Cosette.Tuner.Web.ViewModels;

namespace Cosette.Tuner.Web
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ChromosomeDataRequest, ChromosomeModel>()
                .ForMember(p => p.TestId, p => p.MapFrom(q => q.TestId))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => DateTime.UtcNow))
                .ForMember(p => p.ElapsedTime, p => p.MapFrom(q => q.ElapsedTime))
                .ForMember(p => p.Fitness, p => p.MapFrom(q => q.Fitness))
                .ForMember(p => p.ReferenceEngineWins, p => p.MapFrom(q => q.ReferenceEngineWins))
                .ForMember(p => p.ExperimentalEngineWins, p => p.MapFrom(q => q.ExperimentalEngineWins))
                .ForMember(p => p.Draws, p => p.MapFrom(q => q.Draws))
                .ForMember(p => p.EnginesStatistics, p => p.MapFrom((src, dest, order, context) =>
                {
                    var referenceEngineStatistics = context.Mapper.Map<EngineStatisticsModel>(src.ReferenceEngineStatistics);
                    var experimentalEngineStatistics = context.Mapper.Map<EngineStatisticsModel>(src.ExperimentalEngineStatistics);

                    referenceEngineStatistics.IsReferenceEngine = true;
                    experimentalEngineStatistics.IsReferenceEngine = false;

                    var outputList = new List<EngineStatisticsModel>
                    {
                        referenceEngineStatistics, 
                        experimentalEngineStatistics
                    };

                    return outputList;
                }))
                .ForMember(p => p.Genes, p => p.MapFrom(q => q.Genes));

            CreateMap<GeneDataRequest, GenerationGeneModel>()
                .ForMember(p => p.Name, p => p.MapFrom(q => q.Name))
                .ForMember(p => p.Value, p => p.MapFrom(q => q.Value));

            CreateMap<GeneDataRequest, ChromosomeGeneModel>()
                .ForMember(p => p.Name, p => p.MapFrom(q => q.Name))
                .ForMember(p => p.Value, p => p.MapFrom(q => q.Value));

            CreateMap<EngineStatisticsDataRequest, EngineStatisticsModel>()
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => DateTime.UtcNow))
                .ForMember(p => p.AverageTimePerGame, p => p.MapFrom(q => q.AverageTimePerGame))
                .ForMember(p => p.AverageDepth, p => p.MapFrom(q => q.AverageDepth))
                .ForMember(p => p.AverageNodesCount, p => p.MapFrom(q => q.AverageNodesCount))
                .ForMember(p => p.AverageNodesPerSecond, p => p.MapFrom(q => q.AverageNodesPerSecond));

            CreateMap<GenerationDataRequest, GenerationModel>()
                .ForMember(p => p.TestId, p => p.MapFrom(q => q.TestId))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => DateTime.UtcNow))
                .ForMember(p => p.ElapsedTime, p => p.MapFrom(q => q.ElapsedTime))
                .ForMember(p => p.BestFitness, p => p.MapFrom(q => q.BestFitness))
                .ForMember(p => p.BestGenes, p => p.MapFrom(q => q.BestChromosomeGenes))
                .ForMember(p => p.BestGenes, p => p.MapFrom(q => q.BestChromosomeGenes));

            CreateMap<TestModel, TestViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc));

            CreateMap<GenerationModel, GenerationViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc))
                .ForMember(p => p.ElapsedTime, p => p.MapFrom(q => q.ElapsedTime))
                .ForMember(p => p.BestFitness, p => p.MapFrom(q => q.BestFitness))
                .ForMember(p => p.BestGenes, p => p.MapFrom(q => q.BestGenes));

            CreateMap<GenerationGeneModel, GeneViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.Name, p => p.MapFrom(q => q.Name))
                .ForMember(p => p.Value, p => p.MapFrom(q => q.Value));

            CreateMap<ChromosomeGeneModel, GeneViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.Name, p => p.MapFrom(q => q.Name))
                .ForMember(p => p.Value, p => p.MapFrom(q => q.Value));

            CreateMap<ChromosomeModel, ChromosomeViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc))
                .ForMember(p => p.ElapsedTime, p => p.MapFrom(q => q.ElapsedTime))
                .ForMember(p => p.Fitness, p => p.MapFrom(q => q.Fitness))
                .ForMember(p => p.ReferenceEngineWins, p => p.MapFrom(q => q.ReferenceEngineWins))
                .ForMember(p => p.ExperimentalEngineWins, p => p.MapFrom(q => q.ExperimentalEngineWins))
                .ForMember(p => p.Draws, p => p.MapFrom(q => q.Draws))
                .ForMember(p => p.EnginesStatistics, p => p.MapFrom(q => q.EnginesStatistics))
                .ForMember(p => p.Genes, p => p.MapFrom(q => q.Genes));

            CreateMap<EngineStatisticsModel, EngineStatisticsViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc))
                .ForMember(p => p.IsReferenceEngine, p => p.MapFrom(q => q.IsReferenceEngine))
                .ForMember(p => p.AverageTimePerGame, p => p.MapFrom(q => q.AverageTimePerGame))
                .ForMember(p => p.AverageDepth, p => p.MapFrom(q => q.AverageDepth))
                .ForMember(p => p.AverageNodesCount, p => p.MapFrom(q => q.AverageNodesCount))
                .ForMember(p => p.AverageNodesPerSecond, p => p.MapFrom(q => q.AverageNodesPerSecond));
        }
    }
}
