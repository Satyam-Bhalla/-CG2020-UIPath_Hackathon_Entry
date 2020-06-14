using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

namespace ActiveMessageBoxes
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            if (!workflowAnalyzerConfigService.HasFeature("WorkflowAnalyzerV4"))
            {
                return;
            }
            var forbiddenWorkflowRule = new Rule<IWorkflowModel>("ActiveMessageBoxes", "ST-USG-012", InspectVariableForString);

            forbiddenWorkflowRule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Warning;
            forbiddenWorkflowRule.RecommendationMessage = "No message box should be available during project deployment.";
            workflowAnalyzerConfigService.AddRule<IWorkflowModel>(forbiddenWorkflowRule);
        }


        private InspectionResult InspectVariableForString(IWorkflowModel WorkflowToInspect, Rule configuredRule)
        {
            

            var messageList = new List<InspectionMessage>();



            foreach (var activity in WorkflowToInspect.Root.Children)
            {
                if (!string.IsNullOrEmpty(activity.Type) && activity.Type.Contains("MessageBox"))
                {
                        messageList.Add(new InspectionMessage()
                        {
                            Message = $"MessageBox found in workflow {WorkflowToInspect.DisplayName} with display name as {activity.DisplayName}."
                        });
                }
            }

            if (messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "Message box should not be present during deployment.",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }

            return new InspectionResult() { HasErrors = false };
        }

    }
}