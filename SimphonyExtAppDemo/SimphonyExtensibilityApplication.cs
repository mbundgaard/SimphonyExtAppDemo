using System;
using System.Linq;
using Micros.Ops.Extensibility;
using Micros.PosCore.Extensibility;
using SimphonyExtAppDemo.Clients.Database;
using SimphonyExtAppDemo.Helpers;

namespace SimphonyExtAppDemo
{
    public class ApplicationFactory : IExtensibilityAssemblyFactory
    {
        public ExtensibilityAssemblyBase Create(IExecutionContext context) => new SimphonyExtensibilityApplication(context);
        public void Destroy(ExtensibilityAssemblyBase app) => app.Destroy();
    }

    public class SimphonyExtensibilityApplication : OpsExtensibilityApplication
    {
        public SimphonyExtensibilityApplication(IExecutionContext context) : base(context)
        {
            OpsReadyEvent += (s, a) =>
            {
                OpsContext.ShowMessage("OpsReadyEvent was fired, which means that Simphony is now ready to use");
                return EventProcessingInstruction.Continue;
            };

            OpsExitEvent += (s, a) =>
            {
                OpsContext.ShowMessage("OpsExitEvent was fired, which means that Simphony is stopping");
                return EventProcessingInstruction.Continue;
            };
        }

        public override string CallFunc(object sender, string function, object arg, out object oRet)
        {
            try
            {
                switch (function?.ToLower())
                {
                    case "version": Version(); break;
                    case "databasetest": DatabaseTest(arg as string); break;

                    default: throw new Exception("Invalid script name");
                }
            }
            catch (Exception e)
            {
                OpsContext.ShowError(e.Message);
            }

            oRet = null;
            return string.Empty;
        }

        private void Version()
        {
            OpsContext.ShowMessage(VersionHelper.NameAndVersion);
        }

        private void DatabaseTest(string rvcNumberString)
        {
            if (!int.TryParse(rvcNumberString, out var rvcNumber))
                throw new Exception("Please specify the RVC number as argument on the function call");

            var databaseClient = new DatabaseClient();

            var closedChecks = databaseClient.GetClosedChecksByRvc(rvcNumber).ToList();

            OpsContext.ShowMessage($"CAPS DB contains {closedChecks.Count} closed checks for revenue center {rvcNumber}");
        }
    }
}
