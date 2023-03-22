using AutoMapper;
using BusinessLayer_AppointmentScheduler.Contracts;
using BusinessLayer_AppointmentScheduler.Dto;
using BusinessLayer_AppointmentScheduler.Services;
using DataLayer_AppointmentScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AppointmentScheduler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly IAuthenticationManager _authManager;
        private readonly ILoggerManager _logger;
        private readonly UserManager<Manager> _userManager;
        private readonly IMapper _mapper;

        public ManagerController(IRepositoryManager repository,
            IAuthenticationManager authManager,
            ILoggerManager logger,
            UserManager<Manager> userManager
            , IMapper mapper)
        {
            _repository = repository;
            _authManager = authManager;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }
        [HttpPost("Registeration")]
        public async Task<IActionResult> ManagerRegister([FromBody] UserForRegisterationDTO managerForRegister)
        {
            var user = _mapper.Map<Manager>(managerForRegister);
            var result = await _userManager.CreateAsync(user, managerForRegister.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            //await _userManager.AddToRolesAsync(user, managerForRegister.Roles!);
            await _userManager.AddToRoleAsync(user, "Manager");
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


        [HttpGet]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var managers = await _repository.Manager.GetAllManagersAsync(trackChanges: false);
                var managersDto = _mapper.Map<IEnumerable<AllManagersDTO>>(managers);
                return Ok(managersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(GetAllManagers)} action {ex}");
                return StatusCode(500, "Internal Server Error!");
            }
        }

        [HttpGet("{managerId}")]
        public async Task<IActionResult> GetManager(string managerId)
        {
            if (managerId == null)
            {
                _logger.LogInfo("Manager ID is Null !");
                return BadRequest(ModelState);
            }
            var manager = await _repository.Manager.GetManagerByIdAsync(managerId, trackChanges: false);
            if (manager is null)
            {
                _logger.LogInfo($"Manager With ID : ({managerId}) doesn't exist in the database");
                return NotFound($"Manager With ID : ({managerId}) doesn't exist in the database");
            }
            var managerDto = _mapper.Map<SingleManagerDTO>(manager);
            return Ok(managerDto);
        }
        [HttpDelete("{managerId}")]
        public async Task<IActionResult> DeleteManager(string managerId)
        {
            var manager = await _repository.Manager.GetManagerByIdAsync(managerId, trackChanges: false);
            if (manager == null)
            {
                _logger.LogInfo($"Employee with ID: {managerId} doesn't exist in the DB");
                return NotFound();
            }
            _repository.Manager.DeleteManager(manager);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("Skill")]
        public async Task<IActionResult> AddManagerSkills(string managerId
            , [FromBody] List<SkillForCreationDTO> skills)
        {
            if (skills is null)
            {
                _logger.LogError("SkillForCreationDTO object sent from client is null");
                return BadRequest("SkillForCreationDTO object is Null!");
            }
            var skillEntity = _mapper.Map<IEnumerable<Skill>>(skills).ToList();
            _repository.Skill.AssignSkillsForManager(managerId, skillEntity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("Skills")]
        public async Task<IActionResult> GetSkillsForManager(string managerId)
        {
            if (managerId.IsNullOrEmpty())
            {
                _logger.LogInfo($"Manager's ID is Null!");
                return BadRequest("Manager's ID is NULL");
            }
            var manager = await _repository.Manager.GetManagerByIdAsync(managerId, trackChanges: false);
            if (manager == null)
            {
                _logger.LogInfo($"Manager with ID: {managerId} doesn't exist in the DB");
                return NotFound();
            }
            var skill = await _repository.Skill.GetSkillsForManager(managerId);
            var skillDto = _mapper.Map<IEnumerable<SkillsDTO>>(skill);
            return Ok(skillDto);
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> AssignContractWithEmployee(string employeeId
            , [FromBody] EmployeeForUpdateDTO employee)
        {
            if (employee is null)
            {
                _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var employeeEntity = await _repository.Employee.GetEmployeeByIdAsync(employeeId, trackChanges: true);
            //TrackChanges → True
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with ID : ({employee}) doesn't exist in the database");
                return NotFound();
            }
            _mapper.Map(employee, employeeEntity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("employeeId")]
        public async Task<IActionResult>ChangeEmployeeDepartment(string employeeId 
            , [FromBody] PatchDepartmentDTO employee )
        {
            if(employee is null)
            {
                _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var employeeEntity = await _repository.Employee.GetEmployeeByIdAsync(employeeId, trackChanges: true);
            if (employeeEntity is null)
            {
                _logger.LogInfo($"Employee with ID : ({employee}) doesn't exist in the database");
                return NotFound();
            }
            _mapper.Map(employee, employeeEntity);
            TryValidateModel(employee);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Employee.UpdateEmployee(employeeEntity);
            await _repository.SaveChangesAsync();
            return NoContent() ;
        }

    }
}
