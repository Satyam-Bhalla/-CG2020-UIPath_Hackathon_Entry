using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

namespace BooleanNamingValidation
{
    public class RuleRepository : IRegisterAnalyzerConfiguration
    {
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            if (!workflowAnalyzerConfigService.HasFeature("WorkflowAnalyzerV4"))
            {
                return;
            }
            var forbiddenStringRule = new Rule<IActivityModel>("BooleanVariableVerbs", "ST-NMG-010", InspectVariableForString);

            forbiddenStringRule.DefaultErrorLevel = System.Diagnostics.TraceLevel.Warning;
            forbiddenStringRule.RecommendationMessage = "Boolean variable names should imply True or false, use verb prefixes and suffixes such as _exists,is_,has_";
            workflowAnalyzerConfigService.AddRule<IActivityModel>(forbiddenStringRule);
        }


        private InspectionResult InspectVariableForString(IActivityModel activityToInspect, Rule configuredRule)
        {
            if (activityToInspect.Variables.Count == 0)
            {
                return new InspectionResult() { HasErrors = false };
            }

            var messageList = new List<InspectionMessage>();



            foreach (var variable in activityToInspect.Variables)
            {

                //BOOLEAN EXPRESSION CHECK RULE, HAS TO BE DONE FOR ARGUMENTS AS WELL(ADD RECOMMENDATION MESSAGE AS WELL)
                if (variable.Type.ToString().Contains("System.Boolean"))
                {
                    if ((variable.DisplayName.ToString().Trim().ToLower().StartsWith("has_")) || (variable.DisplayName.ToString().Trim().ToLower().StartsWith("is_")) || (variable.DisplayName.ToString().ToLower().EndsWith("_exists")))
                    {
                        continue;
                    }
                    else
                    {
                        messageList.Add(new InspectionMessage()
                        {

                            Message = $"Variable { variable.DisplayName } does not follow the required naming convention of the boolean variables"


                        });
                    }




                }
            }

            if (messageList.Count > 0)
            {
                return new InspectionResult()
                {
                    HasErrors = true,
                    InspectionMessages = messageList,
                    RecommendationMessage = "Boolean variable does not follow the required naming convention",
                    ErrorLevel = configuredRule.ErrorLevel
                };
            }

            return new InspectionResult() { HasErrors = false };
        }

    }
}

