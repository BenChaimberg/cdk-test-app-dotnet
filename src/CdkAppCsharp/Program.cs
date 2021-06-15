using Amazon.CDK;

namespace CdkStepFunctions
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            _ = new PipelineStack(app, "PipelineStack");
            _ = new CdkStepFunctionsStack(app, "CdkStepFunctionsStack");

            app.Synth();
        }
    }
}
