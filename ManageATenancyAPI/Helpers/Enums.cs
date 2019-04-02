using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Helpers
{       
        public enum SqlConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

    public enum HackneyProcessStage
    {
        [Description("not completed")]
        NotCompleted = 0,
        [Description("completed")]
        Completed = 8
    }

    public enum HackneyServiceArea
    {
        [Description("area 1")]
        Area1 = 1,
        [Description("area 2")]
        Area2 = 2
    }
}
