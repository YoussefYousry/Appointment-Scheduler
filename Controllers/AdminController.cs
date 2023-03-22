using AutoMapper;
using BusinessLayer_AppointmentScheduler.Contracts;
using BusinessLayer_AppointmentScheduler.Dto;
using BusinessLayer_AppointmentScheduler.Services;
using DataLayer_AppointmentScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly IAuthenticationManager _authManager;
        private readonly ILoggerManager _logger;
        private readonly UserManager<Admin> _userAdminManager;
        private readonly IMapper _mapper;

        public AdminController(IRepositoryManager repository,
            IAuthenticationManager authManager,
            ILoggerManager logger,
            UserManager<Admin> userAdminManager
            , IMapper mapper)
        {
            _repository = repository;
            _authManager = authManager;
            _logger = logger;
            _userAdminManager = userAdminManager;
            _mapper = mapper;
        }
        [HttpPost("Registeration")]
        public async Task<IActionResult> AdminRegister([FromBody] UserForRegisterationDTO adminForRegister)
        {
            var user = _mapper.Map<Admin>(adminForRegister);
            var result = await _userAdminManager.CreateAsync(user, adminForRegister.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            //await _userAdminManager.AddToRolesAsync(user, adminForRegister.Roles!);
            await _userAdminManager.AddToRoleAsync(user, "Admin");
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForLoginDTO user)
        {
            if (!await _authManager.ValidateUser(user))
            {
                _logger.LogWarn($"{nameof(Authenticate)}: Authentication Failed. Wrong Username/Password!");
                return Unauthorized();
            }
            return Ok(
                new
                {
                    Token = await _authManager.CreateToken()
                });
        }
        [HttpPost("Department")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentForCreationDTO department)
        {
            if(department is null)
            {
                _logger.LogError("DepartmentForCreationDTO object sent from client is null");
                return BadRequest("DepartmentForCreationDTO object is Null!");
            }
            var departmentEntity = _mapper.Map<Department>(department);
            _repository.Department.CreateDepartment(departmentEntity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{managerId}")]
        public async Task<IActionResult> AssignContractWithManager(string managerId
    , [FromBody] ManagerForUpdateDTO manager)
        {
            if (manager is null)
            {
                _logger.LogError("ManagerForUpdateDTO object sent from client is null.");
                return BadRequest("ManagerForUpdateDTO object is null");
            }
            var managerEntity = await _repository.Manager.GetManagerByIdAsync(managerId, trackChanges: true);
            //TrackChanges → True
            if (managerEntity is null)
            {
                _logger.LogInfo($"Manager with ID : ({managerId}) doesn't exist in the database");
                return NotFound();
            }
            _mapper.Map(manager, managerEntity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
