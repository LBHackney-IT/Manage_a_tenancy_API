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
        Completed = 8,
        [Description("escalated")]
        Escalated = 9
    }
}
