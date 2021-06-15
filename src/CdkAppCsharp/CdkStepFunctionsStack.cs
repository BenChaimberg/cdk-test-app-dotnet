using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;

namespace CdkStepFunctions
{
    public class CdkStepFunctionsStack : Stack
    {
        internal CdkStepFunctionsStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here

            var vpc = new Vpc(this, "Vpc");

            var cluster = new Cluster(this, "Cluster", new ClusterProps
            {
                EnableFargateCapacityProviders = true,
                Vpc = vpc
            });

            var taskDefinition = new FargateTaskDefinition(this, "TaskDefinition");
            taskDefinition.AddToTaskRolePolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "s3:GetObject" },
                Resources = new[] { "arn:aws:s3:::*" }
            }));
            taskDefinition.AddContainer("Container", new ContainerDefinitionProps
            {
                Image = ContainerImage.FromRegistry("httpd:2.4"),
            });

            var ecsRunTask = new EcsRunTask(this, "EcsRunTask", new EcsRunTaskProps
            {
                Cluster = cluster,
                ContainerOverrides = new[]
                {
                    new ContainerOverride
                    {
                        ContainerDefinition = taskDefinition.DefaultContainer,
                        Environment = new[]
                        {
                            new TaskEnvironmentVariable { Name = "TASK_TOKEN", Value = JsonPath.TaskToken, },
                        }
                    }
                },
                IntegrationPattern = IntegrationPattern.WAIT_FOR_TASK_TOKEN,
                LaunchTarget = new EcsFargateLaunchTarget(),
                TaskDefinition = taskDefinition,
            });

            _ = new StateMachine(this, "StateMachine", new StateMachineProps
            {
                Definition = ecsRunTask,
            });
        }
    }
}
