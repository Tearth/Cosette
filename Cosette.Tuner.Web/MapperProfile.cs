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
                .ForMember(p => p.SelfPlayStatistics, p => p.MapFrom((src, dest, order, context) =>
                {
                    var referenceEngineStatistics = context.Mapper.Map<SelfPlayStatisticsModel>(src.ReferenceEngineStatistics);
                    var experimentalEngineStatistics = context.Mapper.Map<SelfPlayStatisticsModel>(src.ExperimentalEngineStatistics);

                    referenceEngineStatistics.IsReferenceEngine = true;
                    experimentalEngineStatistics.IsReferenceEngine = false;

                    var outputList = new List<SelfPlayStatisticsModel>
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

            CreateMap<SelfPlayStatisticsDataRequest, SelfPlayStatisticsModel>()
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => DateTime.UtcNow))
                .ForMember(p => p.AverageTimePerGame, p => p.MapFrom(q => q.AverageTimePerGame))
                .ForMember(p => p.AverageDepth, p => p.MapFrom(q => q.AverageDepth))
                .ForMember(p => p.AverageNodesCount, p => p.MapFrom(q => q.AverageNodesCount))
                .ForMember(p => p.AverageNodesPerSecond, p => p.MapFrom(q => q.AverageNodesPerSecond))
                .ForMember(p => p.Wins, p => p.MapFrom(q => q.Wins))
                .ForMember(p => p.Draws, p => p.MapFrom(q => q.Draws));

            CreateMap<GenerationDataRequest, GenerationModel>()
                .ForMember(p => p.TestId, p => p.MapFrom(q => q.TestId))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => DateTime.UtcNow))
                .ForMember(p => p.ElapsedTime, p => p.MapFrom(q => q.ElapsedTime))
                .ForMember(p => p.BestFitness, p => p.MapFrom(q => q.BestFitness))
                .ForMember(p => p.BestGenes, p => p.MapFrom(q => q.BestChromosomeGenes))
                .ForMember(p => p.BestGenes, p => p.MapFrom(q => q.BestChromosomeGenes));

            CreateMap<TestModel, TestViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc))
                .ForMember(p => p.Type, p => p.MapFrom(q => q.Type));

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
                .ForMember(p => p.ReferenceEngineStatistics, p => p.MapFrom(q => q.SelfPlayStatistics.FirstOrDefault(p => p.IsReferenceEngine)))
                .ForMember(p => p.ExperimentalEngineStatistics, p => p.MapFrom(q => q.SelfPlayStatistics.FirstOrDefault(p => !p.IsReferenceEngine)))
                .ForMember(p => p.Genes, p => p.MapFrom(q => q.Genes));

            CreateMap<SelfPlayStatisticsModel, SelfPlayStatisticsViewModel>()
                .ForMember(p => p.Id, p => p.MapFrom(q => q.Id))
                .ForMember(p => p.CreationTimeUtc, p => p.MapFrom(q => q.CreationTimeUtc))
                .ForMember(p => p.IsReferenceEngine, p => p.MapFrom(q => q.IsReferenceEngine))
                .ForMember(p => p.AverageTimePerGame, p => p.MapFrom(q => q.AverageTimePerGame))
                .ForMember(p => p.AverageDepth, p => p.MapFrom(q => q.AverageDepth))
                .ForMember(p => p.AverageNodesCount, p => p.MapFrom(q => q.AverageNodesCount))
                .ForMember(p => p.AverageNodesPerSecond, p => p.MapFrom(q => q.AverageNodesPerSecond))
                .ForMember(p => p.Wins, p => p.MapFrom(q => q.Wins))
                .ForMember(p => p.Draws, p => p.MapFrom(q => q.Draws));
        }
    }
}
