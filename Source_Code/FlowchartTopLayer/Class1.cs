using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

namespace FlowchartTopLayer
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            if (!workflowAnalyzerConfigService.HasFeature("WorkflowAnalyzerV4"))
            {
                return;
            }
            var forbiddenProjectrule = new Rule<IProjectModel>("Flowchart Top Layer", "ST-DBP-008", InspectVariableForString);
            forbiddenProjectrule.RecommendationMessage = "Best practices advice for main entry workflow to be a flowchart";

            forbiddenProjectrule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Warning;
            
            workflowAnalyzerConfigService.AddRule<IProjectModel>(forbiddenProjectrule);
        }

        private InspectionResult InspectVariableForString(IProjectModel ProjectToInspect, Rule configuredRule)
        {

            var messageList = new List<InspectionMessage>();

            if (ProjectToInspect.EntryPoint.Root != null && !(ProjectToInspect.EntryPoint.Root.Type.ToString().ToLower().Contains("flowchart")))
            {
                messageList.Add(new InspectionMessage()
                {

                    Message = $"Main entry workflow of the project should be a flowchart"

                });
            }
            if (messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "Main entry workflow of the project should be a flowchart",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }

            return new InspectionResult() { HasErrors = false };
        }

    }
}
