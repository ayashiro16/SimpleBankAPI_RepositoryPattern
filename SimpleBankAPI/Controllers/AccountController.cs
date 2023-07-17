using Microsoft.AspNetCore.Mvc;
using SimpleBankAPI.Models.Requests;
using SimpleBankAPI.Models.Responses;
using Account = SimpleBankAPI.Models.Entities.Account;
using IAccountServices = SimpleBankAPI.Interfaces.IAccountServices;

namespace SimpleBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _account;
        
        public AccountController(IAccountServices account)
        {
            _account = account;
        }
        
        /// <summary>
        /// Retrieves account from database
        /// </summary>
        /// <param name="id">The account ID</param>
        /// <returns>The account associated with the provided ID</returns>
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            var account = await _account.FindAccount(id);
            if (account is null)
            {
                return NotFound();
            }
            
            return account;
        }
        
        /// <summary>
        /// Create and store a new account with the provided user's name
        /// </summary>
        /// <param name="request">The string "Name" of the account holder</param>
        /// <returns>The account details of the newly created account</returns>
        [HttpPost]
        public async Task<ActionResult<Account>> PostNewAccount([FromBody] CreateAccount request)
        {
            try
            {
                var account = await _account.CreateAccount(request.Name);
                return account;
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates deposit to add funds to an account
        /// </summary>
        /// <param name="id">The account ID</param>
        /// <param name="request">The decimal "Amount" to be withdrawn</param>
        /// <returns>The account details of the account following the deposit</returns>
        [HttpPost("{id:Guid}/deposits")]
        public async Task<ActionResult<Account>> PostDepositFunds(Guid id, [FromBody] GetAmount request)
        {
            try
            {
                var account = await _account.DepositFunds(id, request.Amount);
                if (account is null)
                {
                    return NotFound();
                }

                return account;
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// Creates withdrawal to take from an account
        /// </summary>
        /// <param name="id">The account ID</param>
        /// <param name="request">The decimal "Amount" to be withdrawn</param>
        /// <returns>The account details of the account following the withdrawal</returns>
        [HttpPost("{id:Guid}/withdrawals")]
        public async Task<ActionResult<Account>> PostWithdrawFunds(Guid id, [FromBody] GetAmount request)
        {
            try
            {
                var account = await _account.WithdrawFunds(id, request.Amount);
                if (account is null)
                {
                    return NotFound();
                }

                return account;
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Creates a transfer that takes funds from sender account and deposits to receiver account
        /// </summary>
        /// <param name="request">Sender's account ID: "SenderId", Recipient's account ID: "RecipientId", and decimal "Amount" to be transferred</param>
        /// <returns>The account details of both the sender and the recipient following the transfer</returns>
        [HttpPost("transfers")]
        public async Task<ActionResult<Transfer>> PostTransferFunds([FromBody] TransferFunds request)
        {
            try
            {
                var accounts = await _account.TransferFunds(request.SenderId, request.RecipientId, request.Amount);
                return accounts switch
                {
                    { Sender: null, Recipient: null } => NotFound("Sender and recipient accounts could not be found"),
                    { Sender: null, Recipient: not null } => NotFound("Sender account could not be found"),
                    { Sender: not null, Recipient: null } => NotFound("Recipient account could not be found"),
                    _ => accounts
                };
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
