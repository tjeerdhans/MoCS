using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business.Interfaces
{
    public interface IExecuteCmd
    {
        int ExecuteCommandSync(object command);
    }
}
