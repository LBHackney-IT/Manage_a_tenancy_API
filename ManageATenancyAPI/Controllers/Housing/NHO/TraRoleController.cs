using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Authorize]
    [Route("v1/[controller]")]
    public class TraRoleController : Controller
    {
        private ITraRoleAction _roleAction;

        public TraRoleController(ITraRoleAction roleAction)
        {
            _roleAction = roleAction;
        }

        [Route("roles/")]
        [HttpGet]
        public async Task<HackneyResult<IList<TraRole>>> GetRoles()
        {
            return HackneyResult<IList<TraRole>>.Create(await _roleAction.GetRoles());
        }
    }
}
