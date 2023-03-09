using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using testing.Data;
using testing.Models;

namespace testing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly testingContext _context;
       // private readonly string loggedname
        private  bool isSession=false;
        public IConfiguration Configuration { get; }
        public HomeController(ILogger<HomeController> logger, testingContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            Configuration = config;
        }
        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {

          
            var numbers = new cCounters();
            //testingContext t=;
            SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("testingContext"));
            await conn.OpenAsync();
            var command = new SqlCommand("countNumClient", conn);
            // var command = new SqlCommand(sql, connection);

            var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                numbers.numClients = reader.GetInt32("numClients");
            }

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                numbers.expData = reader.GetInt32("expData");
            }

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                numbers.extClients = reader.GetInt32("extClients");
            }
            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                numbers.impData = reader.GetInt32("impData");
            }

            await reader.CloseAsync();
            await conn.CloseAsync();

            return View("Index", numbers);
        }

        /*protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();

        }
        */
      /* public async cCounters LoadDataAsync()
        {
            isSession = true;
            var numbers = new cCounters();
            //testingContext t=;
            SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("testingContext"));
            await conn.OpenAsync();
            var command = new SqlCommand("countNumClient", conn);
            // var command = new SqlCommand(sql, connection);


            var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                numbers.numClients = reader.GetInt32("numClients");
            }

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                numbers.expData = reader.GetInt32("expData");
            }

            await reader.CloseAsync();
            await conn.CloseAsync();
            return numbers;
        }*/

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
