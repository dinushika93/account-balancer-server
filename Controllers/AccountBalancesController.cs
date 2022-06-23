using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountBalancer.Models;
using System.Data;
using ExcelDataReader;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace AccountBalancer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountBalancesController : ControllerBase
    {
        private readonly AccountBalanceContext _context;

        public AccountBalancesController(AccountBalanceContext context)
        {
            _context = context;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        [Route("UploadFile")]
        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(string year, string month)
        {
            try
            {
                #region Variable Declaration  
                string message = "";
                var httpRequest = HttpContext.Request;
                DataSet excelRecords = new DataSet();
                IExcelDataReader reader = null;
                Stream FileStream = null;

                if(Request.Form.Files.Count > 0)
                {

                    var inputfile = Request.Form.Files[0];

                    #endregion

                    FileStream = inputfile.OpenReadStream();

                    if (FileStream != null)
                    {
                        if (inputfile.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (inputfile.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else
                            message = "The file format is not supported.";

                        excelRecords = reader.AsDataSet();
                        reader.Close();

                        if (excelRecords != null && excelRecords.Tables.Count > 0)
                        {
                            DataTable dtStudentRecords = excelRecords.Tables[0];

                            AccountBalance accountBalance = new AccountBalance();
                            accountBalance.Id = year.ToString() + string.Format(month, "00");
                            accountBalance.RDBalance = Convert.ToDecimal(dtStudentRecords.Rows[1][0]);
                            accountBalance.CanteenBalance = Convert.ToDecimal(dtStudentRecords.Rows[1][1]);
                            accountBalance.CarBalance = Convert.ToDecimal(dtStudentRecords.Rows[1][1]);
                            accountBalance.MarketingBalance = Convert.ToDecimal(dtStudentRecords.Rows[1][3]);
                            accountBalance.ParkingFinesBalance = Convert.ToDecimal(dtStudentRecords.Rows[1][4]);

                            bool accountBalanceUpdated = await UpdateAccountBalancesAsync(accountBalance, int.Parse(year), int.Parse(month));


                            if (accountBalanceUpdated)
                                return Ok("The Excel file has been successfully uploaded.");

                            else
                                return BadRequest("Something Went Wrong!, The Excel file uploaded has fiald.");
                        }
                        else
                            return BadRequest("Selected file is empty.");
                    }
                    else
                        return BadRequest("Invalid File.");
                }
                else
                    return BadRequest("Please select file to proceed");


            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<Boolean> UpdateAccountBalancesAsync(AccountBalance accountBalance, int year, int month)
        {

            var accountBalanceOfLastMonth = await _context.AccountBalances.FindAsync(String.Format("{0:yyyyMM}", new DateTime(year, month, DateTime.Now.Day).AddMonths(-1)));

            if (accountBalanceOfLastMonth != null)
            {
                accountBalance.RDBalance += accountBalanceOfLastMonth.RDBalance;
                accountBalance.CanteenBalance += accountBalanceOfLastMonth.CanteenBalance;
                accountBalance.MarketingBalance += accountBalanceOfLastMonth.MarketingBalance;
                accountBalance.ParkingFinesBalance += accountBalanceOfLastMonth.ParkingFinesBalance;
            }


            return await PostAccountBalance(accountBalance);
        }

        // GET: api/PeriodicalAccountBalance
        [Route("PeriodicalAccountBalance/{fromMonth}/{fromYear}/{toMonth}/{toYear}")]
        [HttpGet()]
        public IActionResult GetPeriodicalAccountBalance(string fromMonth, string fromYear, string toMonth, string toYear)
        {
            string startPeriod = fromYear + fromMonth;
            string endPeriod = toYear + toMonth;

            var accountBalanceList = _context.AccountBalances.Where(x =>
            String.Compare(x.Id, startPeriod) >= 0
            && String.Compare(x.Id, endPeriod) <= 0).ToList();


            //var accountBalancesCollection = new Dictionary<string, decimal>();

            AccountSummary[] accountSummaries = new AccountSummary[5];

            AccountSummary RDAccount = new AccountSummary();
            RDAccount.Account = "R&D";
            RDAccount.BalanceArray = accountBalanceList.Select(x => x.RDBalance).ToArray();
            accountSummaries[0] = RDAccount;

            AccountSummary Canteen = new AccountSummary();
            Canteen.Account = "Canteen";
            Canteen.BalanceArray = accountBalanceList.Select(x => x.CanteenBalance).ToArray();
            accountSummaries[1] = Canteen;

            AccountSummary Car = new AccountSummary();
            Car.Account = "Car";
            Car.BalanceArray = accountBalanceList.Select(x => x.CarBalance).ToArray();
            accountSummaries[2] = Car;

            AccountSummary Marketing = new AccountSummary();
            Marketing.Account = "Marketing";
            Marketing.BalanceArray = accountBalanceList.Select(x => x.MarketingBalance).ToArray();
            accountSummaries[3] = Marketing;

            AccountSummary ParkingFines = new AccountSummary();
            ParkingFines.Account = "ParkingFines";
            ParkingFines.BalanceArray = accountBalanceList.Select(x => x.ParkingFinesBalance).ToArray();
            accountSummaries[4] = ParkingFines;

            var obj = new { accountSummaries = accountSummaries, id = accountBalanceList.Select(x => x.Id.Insert(4, "/")) };


            return new JsonResult(obj);         
        }


        // GET: api/CurrentBalance
        [HttpGet]
        [Route("CurrentBalance")]
        public  ActionResult<AccountBalance> GetCurrentBalance()
        {
            if (_context.AccountBalances == null)
            {
                return NotFound();
            }
            var accountBalance =  _context.AccountBalances.OrderByDescending(x => x.Id).FirstOrDefault();

            if (accountBalance == null)
            {
                return NotFound();
            }

            return accountBalance;
        } 

        // POST: api/AccountBalances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<Boolean> PostAccountBalance(AccountBalance accountBalance)
        {
            if (_context.AccountBalances == null)
            {
                return false;
            }
            _context.AccountBalances.Add(accountBalance);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountBalanceExists(accountBalance.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;

        }

        private bool AccountBalanceExists(string id)
        {
            return (_context.AccountBalances?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
