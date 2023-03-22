using BusinessLayer_AppointmentScheduler.Contracts;
using BusinessLayer_AppointmentScheduler.Repositories;
using BusinessLayer_AppointmentScheduler.RequestFeatures;
using BusinessLayer_AppointmentScheduler.Services;
using DataLayer_AppointmentScheduler.DbContext;
using DataLayer_AppointmentScheduler.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using System.Text;

namespace AppointmentScheduler.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
            => services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy" , builder => 
                builder.AllowAnyOrigin() // builder.WithOrigin("https://Example.com")
                .AllowAnyMethod()   //builder.WithMethods("POST","GET")
                .AllowAnyHeader()); //builder.WithHeaders("accept ", "content-type")
            });
        public static void ConfigureIISIntegration(this IServiceCollection services)
            => services.Configure<IISOptions>(options =>
            {

            });
        public static void ConfigureSQLContext(this IServiceCollection services , IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("sqlConnection");
            services.AddDbContext<RepositoryContext>(
                opts =>
                {
                    opts.UseSqlServer(connectionString,
                        b => b.MigrationsAssembly("AppointmentScheduler"));
                });
        }
        //Error was here in the Generic of <T>
        public static void ConfigureIdentity<T>(this IServiceCollection services ) where T : User
        {
            var builder = services.AddIdentityCore<T>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;

            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole),
                builder.Services);
            builder.AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders();
        }
        public static void ConfigureJWT(this IServiceCollection services , IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("SECRET");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                        ValidAudience = jwtSettings.GetSection("validAudience").Value,
                        IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                    };
                });
        }
        public static void ConfigureLifeTimeServices(this IServiceCollection services)
        {
            services.AddScoped<User, Employee>();
            services.AddScoped<User, Manager>();
            services.AddScoped<User, Admin>();
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IRepositoryBase<Employee>, EmployeeRepository>();
            services.AddScoped<IRepositoryBase<Manager>, ManagerRepository>();
            services.AddScoped<IRepositoryBase<Skill>, SkillRepository>();
            services.AddScoped<IRepositoryBase<Appointment>, AppointmentRepository>();
            services.AddScoped<IRepositoryBase<Department>, DepartmentRepository>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IManagerRepository, ManagerRepository>();
            services.AddScoped<IAuthenticationManager, AuthenticationManager>(); // ChatGPT  
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IRepositoryBase<Admin>, AdminRepository>();

        }
        //public static async Task<IQueryable<T>> Paginate<T>(this IQueryable<T> query,
        //    int page, int pageSize)
        //{
        //    if (query == null)
        //        throw new ArgumentNullException($"{nameof(query)}");
        //    if (page <= 0)
        //        page = 1;
        //    if (pageSize <= 0)
        //        pageSize = 10;
        //    var totalCount = await query.CountAsync();
        //    var pageCount = (int)(Math.Ceiling(totalCount/(double)pageSize));
        //    var results = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        //    return new RequestParameters<T>
        //    {
        //        PageCount = pageCount,
        //        CurrentPage = page,
        //        PageSize = pageSize,
        //        TotalCount = totalCount,
        //        Results = results

        //    };
        //}
    }

}
