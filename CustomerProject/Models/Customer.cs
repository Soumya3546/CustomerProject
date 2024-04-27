using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerProject.Models
{
	public class Customer
	{
		private static Random _random = new Random();
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CustomerId { get; set; }
		public string CustomerName { get; set; }

		public string UserName {  get; set; }
		public DateTime DOB {  get; set; }
		public string City {  get; set; }
		public string AccountNumber { get; set; }
		public int AccountBalance { get; set; }

		public string Password { get; set; }

		public static string GenerateRandomDigits(List<string> existingAccountNumbers)
		{
			string accountNumber = "";
			do
			{		
				for (int i = 0; i < 10; i++)
				{
					accountNumber = accountNumber + Convert.ToString((char)('0' + _random.Next(10)));
				}
			}
			while (existingAccountNumbers.Contains(accountNumber)) ;
				return accountNumber;
			
		}
	}
}
