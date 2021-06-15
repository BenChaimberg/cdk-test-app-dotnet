using Amazon.CDK;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.Pipelines;

namespace CdkStepFunctions
{
    public class PipelineStack : Stack
    {
        public PipelineStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var asmArtifact = new Artifact_();
            var sourceArtifact = new Artifact_();

            _ = new CdkPipeline(this, "Pipeline", new CdkPipelineProps
            {
                CloudAssemblyArtifact = asmArtifact,
                SourceAction = new CodeStarConnectionsSourceAction(new CodeStarConnectionsSourceActionProps
                {
                    ActionName = "Source",
                    ConnectionArn = "your-codestar-connection-arn",
                    Output = sourceArtifact,
                    Owner = "your-owner",
                    Repo = "your-repo",
                }),
                SynthAction = new SimpleSynthAction(new SimpleSynthActionProps
                {
                    CloudAssemblyArtifact = asmArtifact,
                    SourceArtifact = sourceArtifact,
                    SynthCommand = "cdk synth",
                })
            });
        }
    }
}
