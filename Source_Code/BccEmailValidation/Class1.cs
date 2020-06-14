using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

namespace HackathonDemo
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            if (!workflowAnalyzerConfigService.HasFeature("WorkflowAnalyzerV4"))
            {
                return;
            }
            var forbiddenStringRule = new Rule<IActivityModel>("BccEmailValidation", "ST-DBP-025", InspectVariableForString);
            
            forbiddenStringRule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Warning;
            forbiddenStringRule.RecommendationMessage = "The Bcc field of Send Mail activities should be validated before deployment if not empty";
            workflowAnalyzerConfigService.AddRule<IActivityModel>(forbiddenStringRule);
        }
        

        private InspectionResult InspectVariableForString(IActivityModel activityToInspect, Rule configuredRule)
        {
            if (activityToInspect.Variables.Count == 0)
            {
                return new InspectionResult() { HasErrors = false };
            }

            var messageList = new List<InspectionMessage>();

           
            
            foreach (var activity in activityToInspect.Children)
            {

                // BCC Rule(ADD RECOMMENDATION MESSAGE AS WELL)
                if (activity.Type.ToString().Contains("UiPath.Mail.Activities"))
                { 
                    foreach (var temp in activity.Arguments)
                    {
                        if (temp.DisplayName == "Bcc" && !(string.IsNullOrEmpty(temp.DefinedExpression)))
                        {
                            messageList.Add(new InspectionMessage()
                            {

                                Message = $"BCC was found in {activity.DisplayName.ToString()} with email address as { temp.DefinedExpression.ToString() }"


                            });
                        }

                    }
                }
            }

            if (messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "Please check Bcc property in Send Mail activity and validate the email addresses before deploying",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }

            return new InspectionResult() { HasErrors = false };
        }

    }
}
