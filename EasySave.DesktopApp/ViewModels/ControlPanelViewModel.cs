using EasySave.lib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.DesktopApp.ViewModels
{
    public class ControlPanelViewModel
    {
        public SaveWorkManager _SaveWorkManager = new SaveWorkManager();
        public int TestNameSaveWork(string SaveWorkName)
        {
            return _SaveWorkManager.TestNameSaveWork(SaveWorkName);
        }

        public int TestTypeSaveWork(string SaveWorkTypeToConvert)
        {
            return _SaveWorkManager.TestTypeSaveWork(SaveWorkTypeToConvert);
        }

        public int TestSourcePathSaveWork(string SaveWorkSourcePath)
        {
            return _SaveWorkManager.TestSourcePathSaveWork(SaveWorkSourcePath);
        }

        public int TestDestinationPathSaveWork(string SaveWorkDestinationPath)
        {
            return _SaveWorkManager.TestDestinationPathSaveWork(SaveWorkDestinationPath);
        }

    }
}
