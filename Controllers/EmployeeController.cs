using AutoMapper;
using BusinessLayer_AppointmentScheduler.Contracts;
using BusinessLayer_AppointmentScheduler.Dto;
using BusinessLayer_AppointmentScheduler.RequestFeatures;
using BusinessLayer_AppointmentScheduler.Services;
using DataLayer_AppointmentScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AppointmentScheduler.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IAuthenticationManager _authManager;
        private readonly UserManager<Employee> _userEmployeeManager;
        public EmployeeController(IRepositoryManager repository, ILoggerManager logger
            , IMapper mapper, IAuthenticationManager authManager, UserManager<Employee> userEmployeeManager)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
            _userEmployeeManager = userEmployeeManager;
        }
        [HttpPost("Registeration")]
        public async Task<IActionResult> EmployeeRegister([FromBody] UserForRegisterationDTO employeeForRegister)
        {
            var user = _mapper.Map<Employee>(employeeForRegister);
            var result = await _userEmployeeManager.CreateAsync(user, employeeForRegister.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            //await _userEmployeeManager.AddToRolesAsync(user, employeeForRegister.Roles!);
            await _userEmployeeManager.AddToRoleAsync(user, "Employee");
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
        public async Task<IActionResult> GetEmployees([FromQuery] RequestParameters parameters)
        {
                var employees = await _repository.Employee.GetAllEmployeesAsync(parameters, trackChanges: false);
                var employeesDto = _mapper.Map<IEnumerable<AllEmployeesDTO>>(employees);
                return Ok(employeesDto);
            //throw new Exception("Exception"); to test GLOBAL_ERROR_HANDLING METHOD
        }
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeWithId(string employeeId)
        {
            if (employeeId.IsNullOrEmpty())
            {
                _logger.LogInfo($"Employee ID is Null!");
                return BadRequest("Employee ID is NULL");
            }
            var employee = await _repository.Employee.GetEmployeeByIdAsync(employeeId, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with ID: {employeeId} doesn't exist in the DB");
                return NotFound();
            }
            var employeeDto = _mapper.Map<SingleEmployeeDTO>(employee);
            return Ok(employeeDto);

        }
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(string employeeId)
        {
            var employee = await _repository.Employee.GetEmployeeByIdAsync(employeeId, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with ID: {employeeId} doesn't exist in the DB");
                return NotFound();
            }
            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("Skill")]
        public async Task<IActionResult> AddEmployeeSkills(string employeeId
            , [FromBody] List<SkillForCreationDTO> skills)
        {
            if (skills is null) {
                _logger.LogError("SkillForCreationDTO object sent from client is null");
                return BadRequest("SkillForCreationDTO object is Null!");
            }
            var skillEntity = _mapper.Map<IEnumerable<Skill>>(skills).ToList();
            _repository.Skill.AssignSkillsForEmployee(employeeId, skillEntity);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("Skills")]
        public async Task<IActionResult> GetSkillsForEmployee(string employeeId)
        {
            if (employeeId.IsNullOrEmpty())
            {
                _logger.LogInfo($"Employee ID is Null!");
                return BadRequest("Employee ID is NULL");
            }
            var employee = await _repository.Employee.GetEmployeeByIdAsync(employeeId, trackChanges: false);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with ID: {employeeId} doesn't exist in the DB");
                return NotFound();
            }
            var skill = await _repository.Skill.GetSkillsForEmployee(employeeId);
            var skillDto = _mapper.Map<IEnumerable<SkillsDTO>>(skill);
            return Ok(skillDto);
        }
        [HttpGet("departmentId")]
        public async Task<IActionResult> GetEmployeesForDepartment([FromQuery] RequestParameters parameters, string departmentId)
        {
            if(departmentId.IsNullOrEmpty())
            {
                _logger.LogInfo($"Department ID is Null!");
                return BadRequest("Department ID is NULL");
            }
            var department  = await _repository.Department.GetDepartmentByIdAsync(departmentId, trackChanges: false);
            if (department == null)
            {
                _logger.LogInfo($"Department with ID : {(departmentId)} doesn't exist in the database.");
                return NotFound($"Department with ID : {(departmentId)} doesn't exist in the database.");
            }
            var employees = await _repository.Employee.GetAllEmployeesDepartment(parameters, departmentId);
            var employeesDto = _mapper.Map<IEnumerable<AllEmployeesDTO>>(employees);
            return Ok(employeesDto);

        }
    }
}
