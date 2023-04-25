﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HW_030423
{
	internal class Program
	{
		static void Main()
		{
			Account account = new Account("Bank depositor");
			account.SetState(new SilverState(0.0, account));
			account.Deposit(500.0);
			account.Deposit(300.0);
			account.PayInterest();
			account.Deposit(550.0);
			account.PayInterest();
			account.Withdraw(2000.00);
			account.Withdraw(1100.00);
			account.PayInterest();
			
			Console.ReadKey(true);
		}
	}

	public class Account
	{
		private State state;
		private string owner;

		public Account(string owner)
		{
			this.owner = owner;
		}

		public double GetBalance()
		{
			return state.GetBalance();
		}

		public State GetState()
		{
			return state;
		}

		public void SetState(State state)
		{
			this.state = state;
		}

		public void Deposit(double amount)
		{
			state.Deposit(amount);
			string buffer = $"Deposited {amount}$ -----\n";
			Console.WriteLine(buffer);
			buffer = $"Balance {this.GetBalance()}$\n";
			Console.WriteLine(buffer);
			buffer = $"Status {this.GetState().GetType().Name}\n\n";
			Console.WriteLine(buffer);
		}

		public void Withdraw(double amount)
		{
			if (state.Withdraw(amount)) {
				string buffer = $"Withdraw {amount}$ -----\n";
				Console.WriteLine(buffer);
				buffer = $"Balance {this.GetBalance()}$\n";
				Console.WriteLine(buffer);
				buffer = $"Status {this.GetState().GetType().Name}\n\n";
				Console.WriteLine(buffer);
			}
		}

		public void PayInterest()
		{
			if (state.PayInterest()) {
				Console.WriteLine("Interest Paid -----\n");
				string buffer = $"Balance {this.GetBalance()}$\n";
				Console.WriteLine(buffer);
				buffer = $"Status {this.GetState().GetType().Name}\n\n";
				Console.WriteLine(buffer);
			}
		}

		public override string ToString()
		{
			return owner;
		}
	}

	public abstract class State
	{
		protected Account account;
		protected double balance;
		protected double interest;
		protected double lowerLimit;
		protected double upperLimit;

		public Account GetAccount()
		{
			return account;
		}


		public void SetAccount(Account account)
		{
			this.account = account;
		}


		public double GetBalance()
		{
			return balance;
		}


		public void SetBalance(double balance)
		{
			this.balance = balance;
		}


		public abstract void Deposit(double amount);

		public abstract bool Withdraw(double amount);

		public abstract bool PayInterest();
	}

	public class RedState : State
	{
		private void Initialize()
		{
			interest = 0.0;
			lowerLimit = -100.0;
			upperLimit = 0.0;
		}
		private void StateChangeCheck()
		{
			if (balance > upperLimit) {
				account.SetState(new SilverState(this));
			}
		}

		public RedState(State state)
		{
			this.balance = state.GetBalance();
			this.account = state.GetAccount();
			Initialize();
		}

		public override void Deposit(double amount)
		{
			balance += amount;
			StateChangeCheck();
		}

		public override bool Withdraw(double amount)
		{
			Console.WriteLine("No funds available for withdrawal!\n");
			return false;
		}

		public override bool PayInterest()
		{
			Console.WriteLine("No interest paid!\n");
			return false;
		}
	}

	public class SilverState : State
	{
		private void Initialize()
		{
			interest = 0.01;
			lowerLimit = 0.0;
			upperLimit = 1000.0;
		}
		private void StateChangeCheck()
		{
			if (balance < lowerLimit) {
				account.SetState(new RedState(this));
			} else if (balance > upperLimit) {
				account.SetState(new GoldState(this));
			}
		}

		public SilverState(double balance, Account account)
		{
			this.balance = balance;
			this.account = account;
			Initialize();
		}

		public SilverState(State state) : this(state.GetBalance(), state.GetAccount()) { }

		public override void Deposit(double amount)
		{
			balance += amount;
			StateChangeCheck();
		}

		public override bool Withdraw(double amount)
		{
			balance -= amount;
			StateChangeCheck();
			return true;
		}

		public override bool PayInterest()
		{
			balance += interest * balance;
			StateChangeCheck();
			return true;
		}
	}

	public class GoldState : State
	{
		private void Initialize()
		{
			interest = 0.07;
			lowerLimit = 1000.0;
			upperLimit = 10000000.0;
		}
		private void StateChangeCheck()
		{
			if (balance < lowerLimit) {
				account.SetState(new RedState(this));
			} else if (balance > upperLimit) {
				account.SetState(new GoldState(this));
			}
		}

		public GoldState(double balance, Account account)
		{
			this.balance = balance;
			this.account = account;
			Initialize();
		}

		public GoldState(State state) : this(state.GetBalance(), state.GetAccount()) { }

		public override void Deposit(double amount)
		{
			balance += amount;
			StateChangeCheck();
		}

		public override bool Withdraw(double amount)
		{
			balance -= amount;
			StateChangeCheck();
			return true;
		}

		public override bool PayInterest()
		{
			balance += interest * balance;
			StateChangeCheck();
			return true;
		}
	}
}
