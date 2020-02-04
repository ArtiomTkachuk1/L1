/*
 * Created by SharpDevelop.
 * User: guyver
 * Date: 23.01.2018
 * Time: 16:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Functions
{
	class Function
	{
		public Function()
		{

		}
		public double Simpson(double a, double b, int n)
		{
			double h = (b - a) / n;
			double k1 = 0, k2 = 0;
			for (int i = 1; i < n; i += 2)
			{
				k1 += this.Calc(a + i * h);
				k2 += this.Calc(a + (i + 1) * h);
			}
			return h / 3 * (this.Calc(a) + 4 * k1 + 2 * k2);
		}
		public double HalfDiv(double a, double b, double sigma, int n)
		{
			double f_a = this.Calc(a);
			double f_b = this.Calc(b);
			for (int i = 1; (i < n)&&((b - a) >sigma); i += 2)
			{
				double xm = (a + b) / 2;
				double f_xm = this.Calc(xm);
				if ((f_a * f_xm) <= 0)
				{
					a = xm;
					f_a = f_xm;
				}
				else
				{
					b = xm;
					f_b = f_xm;
				}
			}
			return (a+b)/2;
		}
		public virtual string ToString()
		{
			return "abstract";
		}
		public virtual double Calc(double x)
		{
			return 0;
		}
		public virtual Function Diff()
		{
			return null;
		}
	}
	class Const : Function
	{
		protected double arg;
		public Const(double value)
		{
			arg = value;
		}
		public override string ToString()
		{
			return arg.ToString();
		}
		public override double Calc(double x)
		{
			return arg;
		}
		public override Function Diff()
		{
			return new Const(0);
		}
	}
	class Arg : Function
	{
		public override string ToString()
		{
			return "x";
		}
		public override double Calc(double x)
		{
			return x;
		}
		public override Function Diff()
		{
			return new Const(1);
		}
	}
	class Unar : Function
	{
		protected Function LP;
		public Unar(Function f)
		{
			LP = f;
		}
		public override string ToString()
		{
			return this.GetType().Name + "(" + LP.ToString() + ")";
		}
	}
	class Sin : Unar
	{
		public Sin(Function f) :base(f){ }
		public override double Calc(double x)
		{
			return Math.Sin(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Cos(LP);
		}
	}
	class Cos : Unar

	{
		public Cos(Function f) : base(f) { }
		public override double Calc(double x)
		{
			return Math.Cos(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Sin(LP);
		}
	}
	class tg : Unar

	{
		public tg(Function f) : base(f) { }
		public override double Calc(double x)
		{
			return Math.Tan(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Division(new Const(1),new Pow(new Cos(LP),new Const(2)));
		}
	}
	class ctg : Unar

	{
		public ctg(Function f) : base(f) { }
		public override double Calc(double x)
		{
			return 1/Math.Tan(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Division(new Const(-1), new Pow(new Sin(LP), new Const(2)));
		}
	}
	class Exp : Unar
	{
		public Exp(Function f) : base(f) { }
		public override double Calc(double x)
		{
			return Math.Exp(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Exp(LP);
		}
	}
	class Ln : Unar
	{
		public Ln(Function f) : base(f) { }
		public override double Calc(double x)
		{
			return Math.Log(LP.Calc(x));
		}
		public override Function Diff()
		{
			return new Division(new Const(1),LP);
		}
	}
	class Binar : Function
	{
		public Binar(Function f1, Function f2)
		{
			LP = f1;
			RP = f2;
		}
		public string SetBraces(Function F)
		{
			if ((F.GetType().Name == "Arg") || (F.GetType().Name == "Const") || (F.GetType().BaseType.Name == "Unar"))
			{
				return F.ToString();
			}
			return "(" + F.ToString() + ")";
		}
		protected Function LP;
		protected Function RP;
	}
	class Sum : Binar
	{
		public Sum(Function f1, Function f2) : base(f1, f2){}
		public override string ToString()
		{
			return SetBraces(LP)+"+"+ SetBraces(RP);
		}
		public override double Calc(double x)
		{
			return LP.Calc(x)+RP.Calc(x);
		}
		public override Function Diff()
		{
			return new Sum(LP.Diff(),RP.Diff());
		}
	}
	class Minus : Binar
	{
		public Minus(Function f1, Function f2) : base(f1, f2) { }
		public override string ToString()
		{
			return SetBraces(LP) + "-" + SetBraces(RP);
		}
		public override double Calc(double x)
		{
			return LP.Calc(x) - RP.Calc(x);
		}
		public override Function Diff()
		{
			return new Minus(LP.Diff(), RP.Diff());
		}
	}
	class Mul : Binar
	{
		public Mul(Function f1, Function f2) : base(f1, f2) {}
		public override string ToString()
		{
			if (SetBraces(RP) == "1")
			{
				return SetBraces(LP);
			}
			if (SetBraces(LP) == "1")
			{
				return SetBraces(RP);
			}
			return SetBraces(LP) + "*" + SetBraces(RP);
		}
		public override double Calc(double x)
		{
			return LP.Calc(x) * RP.Calc(x);
		}
		public override Function Diff()
		{
			return new Sum(new Mul(LP.Diff(), RP), new Mul(LP, RP.Diff()));
		}
	}
	class Pow : Binar
	{
		public Pow(Function f1, Function f2) : base(f1, f2) { }
		public override string ToString()
		{
			if (SetBraces(RP) == "1")
			{
				return SetBraces(LP);
			}
			return SetBraces(LP) + "^" + SetBraces(RP);
		}
		public override double Calc(double x)
		{
			return Math.Pow(LP.Calc(x),RP.Calc(x));
		}
		public override Function Diff()
		{
			Boolean Lconst = false;
			if(LP.Diff().ToString() == "0")
			{
				Lconst = true;
			}
			Boolean Rconst = false;
			if (RP.Diff().ToString() == "0")
			{
				Rconst = true;
			}
			if ((Lconst==true) && (Rconst==true))
			{
				return new Const(0);
			}
			if (Lconst == true)
			{
				return new Mul(new Pow(LP, RP), new Ln(LP));
			}
			if (Rconst == true)
			{
				return new Mul(new Pow(LP, new Minus(RP, new Const(1))), RP);
			}
			return new Sum(new Mul(RP, new Mul(LP.Diff(), new Pow(LP, new Minus(RP, new Const(1))))), new Mul(RP.Diff(), new Mul(new Pow(LP, RP), new Ln(LP))));
		}
	}
	class Division : Binar
	{
		public Division(Function f1, Function f2) : base(f1, f2) { }
		public override string ToString()
		{
			if (SetBraces(RP) == "1")
			{
				return SetBraces(LP);
			}
			return SetBraces(LP) + "/" + SetBraces(RP);
		}
		public override double Calc(double x)
		{
			return LP.Calc(x) / RP.Calc(x);
		}
		public override Function Diff()
		{
			return new Division(new Minus(new Mul(LP.Diff(), RP), new Mul(LP, RP.Diff())), new Pow(RP, new Const(2)));
		}
	}
	class Polynom : Function 
	{
		protected Function Pol;
		public Polynom(Function[] Functions) 
		{
			if (Functions.Length == 1)
			{
				Pol = Functions[0];
			}
			else
			{
				Function[] newFunctions = new Function[Functions.Length-1];
				for(int i=0;i< newFunctions.Length; i++)
				{
					newFunctions[i] = Functions[i];
				}
				Pol = new Sum(Functions[Functions.Length - 1], new Polynom(newFunctions));
			}
			
		}
		public override string ToString()
		{
			return Pol.ToString();
		}
		public override double Calc(double x)
		{
			return Pol.Calc(x);
		}
		public override Function Diff()
		{
			return Pol.Diff();
		}

	}
	class Program
	{
		
		public static void Main(string[] args)
		{
			double x = 1;
			Function f1 = new Sin(new Exp(new Arg()));
			Console.WriteLine(f1.ToString());
			Console.WriteLine(f1.Diff().ToString());
			Console.WriteLine(f1.Diff().Calc(x));
			Function f2 = new Division(new Cos(new Arg()),new Arg());
			Console.WriteLine(f2.ToString());
			Console.WriteLine(f2.Diff().ToString());
			Console.WriteLine(f2.Diff().Calc(x));
			Function[] Functions = new Function[]
				{
				 new tg(new Arg()),
				 new ctg(new Arg()),
				 new Cos(new Arg())
				};
			Function f3 = new Polynom(Functions);
			Console.WriteLine(f3.ToString());
			Console.WriteLine(f3.Diff().ToString());
			Console.WriteLine(f3.Diff().Calc(x));
			Console.WriteLine(f3.Simpson(2, 10, 1000));
			Console.WriteLine(f3.HalfDiv(2, 10, 0.1, 1000));
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}
/*
TO DO
division by zero exceptions
*/