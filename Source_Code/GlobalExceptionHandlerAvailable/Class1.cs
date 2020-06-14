using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

namespace GlobalExceptionHandlerAvailable
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            if (!workflowAnalyzerConfigService.HasFeature("WorkflowAnalyzerV4"))
            {
                return;
            }
            var forbiddenProjectRule = new Rule<IProjectModel>("GlobalExceptionHandlerAvailable", "ST-DBP-026", InspectVariableForString);

            forbiddenProjectRule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Warning;
            forbiddenProjectRule.RecommendationMessage = "Global Exception handler is recommended";
            workflowAnalyzerConfigService.AddRule<IProjectModel>(forbiddenProjectRule);
        }

        private InspectionResult InspectVariableForString(IProjectModel ProjectToInspect, Rule configuredRule)
        {
            
            var messageList = new List<InspectionMessage>();
            if (string.IsNullOrEmpty(ProjectToInspect.ExceptionHandlerWorkflowName))
            {
                messageList.Add(new InspectionMessage()
                {
                    Message = $"A Global exception handler recommended for better exception handling"
                });
            }

            if (messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "A Global exception handler recommended for better exception handling",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }
            return new InspectionResult() { HasErrors = false };
        }

    }
}

