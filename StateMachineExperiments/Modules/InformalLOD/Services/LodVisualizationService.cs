using Stateless.Graph;
using StateMachineExperiments.Modules.InformalLOD.Models;
using System.IO;

namespace StateMachineExperiments.Modules.InformalLOD.Services
{
    public interface ILodVisualizationService
    {
        string GenerateDotGraph(InformalLineOfDuty lodCase);
        void SaveDotGraphToFile(InformalLineOfDuty lodCase, string filePath);
    }

    public class LodVisualizationService : ILodVisualizationService
    {
        private readonly ILodStateMachineFactory _stateMachineFactory;

        public LodVisualizationService(ILodStateMachineFactory stateMachineFactory)
        {
            _stateMachineFactory = stateMachineFactory;
        }

        public string GenerateDotGraph(InformalLineOfDuty lodCase)
        {
            var stateMachine = _stateMachineFactory.CreateStateMachine(lodCase);
            return UmlDotGraph.Format(stateMachine.GetInfo());
        }

        public void SaveDotGraphToFile(InformalLineOfDuty lodCase, string filePath)
        {
            var dotGraph = GenerateDotGraph(lodCase);
            File.WriteAllText(filePath, dotGraph);
        }
    }
}
