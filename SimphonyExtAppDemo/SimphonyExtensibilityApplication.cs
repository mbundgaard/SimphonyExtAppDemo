using System;
using System.Diagnostics;
using System.Linq;
using Micros.Ops;
using Micros.Ops.Extensibility;
using Micros.PosCore.Extensibility;
using Micros.PosCore.Extensibility.Ops;
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
        private bool _selectCheckInProgress;
        private int _selectedCheckNumber;

        public SimphonyExtensibilityApplication(IExecutionContext context) : base(context)
        {
            //OpsReadyEvent += (s, a) =>
            //{
            //    OpsContext.ShowMessage("OpsReadyEvent was fired, which means that Simphony is now ready to use (well, almost ready)");
            //    return EventProcessingInstruction.Continue;
            //};

            //OpsExitEvent += (s, a) =>
            //{
            //    OpsContext.ShowMessage("OpsExitEvent was fired, which means that Simphony is stopping");
            //    return EventProcessingInstruction.Continue;
            //};

            OpsPickUpCheckEventPreview += (s, a) =>
            {
                if (!_selectCheckInProgress) return EventProcessingInstruction.Continue;

                _selectedCheckNumber = (int)a.GetPropertyValue("CheckNumber");

                return EventProcessingInstruction.AbortEvent;
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
                    case "selectchecktest": SelectCheckTest(arg as string); break;

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

        private void SelectCheckTest(string rvcNumberString)
        {
            if (!int.TryParse(rvcNumberString, out var rvcNumber))
                throw new Exception("Please specify the RVC number as argument on the function call");

            _selectCheckInProgress = true;

            OpsContext.ProcessCommand(new OpsCommand { Command = OpsCommandType.PickUpCheckFromListRvcIndex, Index = rvcNumber });

            _selectCheckInProgress = false;

            if (_selectedCheckNumber != 0)
            {
                OpsContext.ShowMessage($"Check number {_selectedCheckNumber} was selected");
                _selectedCheckNumber = 0;
            }
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
