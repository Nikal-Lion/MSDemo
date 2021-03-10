using Microsoft.AspNetCore.Mvc;
using MS.Models.ViewModel;
using MS.Services;
using MS.WebCore.Core;
using System.Threading.Tasks;

namespace MS.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RoleController : AuthorizeController
    {
        private readonly IRoleService _roleService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleService"></param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ExecuteResult> Post(RoleViewModel viewModel)
        {
            return await _roleService.Create(viewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ExecuteResult> Put(RoleViewModel viewModel)
        {
            return await _roleService.Update(viewModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ExecuteResult> Delete(long id)
        {
            return await _roleService.Delete(new RoleViewModel { Id = id });
        }
    }
}