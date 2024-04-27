using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerProject.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly CustomerDBContext _context;

		public CustomerController(CustomerDBContext context)
		{
			_context = context;
		}

		[HttpGet("GetAllCustomers")]
		public ActionResult<List<Customer>> GetAll()
		{
			return _context.Customers.ToList();
		}

		// Helper method to calculate password hash (you can use a more secure method in production)
		private string CalculatePasswordHash(string password)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
		}

		// Helper method to verify password
		private bool VerifyPassword(string password, string passwordHash)
		{
			return CalculatePasswordHash(password) == passwordHash;
		}

		[HttpPost("RegisterCustomer")]
		public async Task<ActionResult<Customer>> RegisterCustomer(string customerName, DateTime dob, string city, string userName, string password)
		{
			try
			{
				var existingCustomer = await _context.Customers.SingleOrDefaultAsync(c => c.UserName == userName);
				if (existingCustomer != null)
				{
					return new Customer();
				}

				List<string> accountNumbers = await _context.Customers
									  .Select(x => x.AccountNumber)
									  .ToListAsync();


				var customer = new Customer
				{
					CustomerName = customerName,
					DOB = dob,
					City = city,
					UserName = userName,
					Password = CalculatePasswordHash(password),
					AccountNumber = Customer.GenerateRandomDigits(accountNumbers),
					AccountBalance = 0 // By default account balance is 0
				};

				await _context.Customers.AddAsync(customer);
				await _context.SaveChangesAsync();
				return Ok(customer);
				
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}



		[HttpPost("login")]
		public ActionResult<Customer> LoginCustomer(string userName, string password)
		{
			try
			{
				var customer = _context.Customers.FirstOrDefault(c => c.UserName == userName);
				if (customer == null || !VerifyPassword(password, customer.Password))
				{
					throw new InvalidOperationException("Invalid username or password");
				}
				return Ok(customer);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{customerId}/balance")]
		public ActionResult<decimal> GetAccountBalance(int customerId)
		{
			try
			{
				var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
				return Ok(customer.AccountBalance);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{customerId}")]
		public ActionResult<Customer> GetCustomerDetails(int customerId)
		{
			try
			{
				return Ok(_context.Customers.FirstOrDefault(c => c.CustomerId == customerId));
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{customerid}/deposit")]
		public ActionResult depositmoney(int customerid, int amount)
		{
			try
			{
				var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerid);
				if (customer != null)
				{
					customer.AccountBalance += amount;
					_context.SaveChanges();
				}
				return Ok("Money successfully deposited");
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("{customerId}/withdraw")]
		public ActionResult WithdrawMoney(int customerId, int amount)
		{
			try
			{
				var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
				if (customer != null)
				{
					if (customer.AccountBalance >= amount && amount>0)
					{
						customer.AccountBalance -= amount;
						_context.SaveChanges();
					}
					else
					{
						throw new InvalidOperationException("Insufficient funds or check the entered amount");
					}
					
				}
				return Ok("Money successfully withdrawn");
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("senior")]
		public ActionResult<List<Customer>> GetSeniorCitizens()
		{

			try
			{
				return Ok(_context.Customers.Where(c => (DateTime.Now.Year - c.DOB.Year) >= 60).ToList());
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("city/{city}")]
		public ActionResult<List<Customer>> GetCustomersByCity(string city)
		{
			try
			{
				return Ok(_context.Customers.Where(c => c.City == city).ToList());
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
