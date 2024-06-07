using System.Diagnostics;

namespace GameDevMathSolutionsAngelo;

// Not bothering with access modifiers and other best practices since this 
// code is throwaway. 

// Could be more succinct, but I wanted it to be easy to read.

// I combined both the dice game, and Crazy8s into one script. Just to make
// things a bit less time consuming for myself.

enum DiceRollMode
{
	Min,
	Max
}

class Program
{
	static readonly Random rng = new();

	static void Main(string[] args)
	{
		int userGameChoice;
		int numTrials;

		Console.WriteLine("Enter 1 to simulate D100. Enter 2 to simulate Crazy8s:");
		while (!int.TryParse(Console.ReadLine(), out userGameChoice) || userGameChoice < 1 || userGameChoice > 2)
		{
			Console.WriteLine("Invalid input. Please enter `1` or `2` (`CTRL C` to exit):");
		}

		if (userGameChoice == 1)
		{
			const int numFaces = 100;
			int numRollsPerTrial;

			Console.WriteLine("Enter the number of D100 rolls per simulation trial:");
			while (!int.TryParse(Console.ReadLine(), out numRollsPerTrial) || numRollsPerTrial <= 0)
			{
				Console.WriteLine("Invalid input. Enter a positive integer for the number of rolls per trial (`CTRL C` to exit):");
			}

			numTrials = GetNumTrialsFromUser();

			MonteCarlo(() => Dice(numRollsPerTrial, numFaces), numTrials, logResults: true);
		}
		else
		{
			numTrials = GetNumTrialsFromUser();

			MonteCarlo(Crazy8s, numTrials, logResults: true);
		}

		static int GetNumTrialsFromUser() 
		{
			int nTrials;
			Console.WriteLine("Enter the number of simulation trials:");
			while (!int.TryParse(Console.ReadLine(), out nTrials) || nTrials <= 0)
			{
				Console.WriteLine("Invalid input. Enter a positive integer for the number of trials (`CTRL C` to exit):");
			}
			return nTrials;
		}
	}

	static double[] MonteCarlo(Func<double> simFunction, int numTrials, bool logResults = false)
	{
		Debug.Assert(numTrials > 0);

		if (logResults)
		{
			Console.WriteLine("Running Monte Carlo Simulation...");
		}

		double[] results = new double[numTrials];
		double cumulativeSum = 0.0;

		for (int trial = 0; trial < numTrials; trial++)
		{
			double trialResult = simFunction();
			results[trial] = trialResult;
			cumulativeSum += trialResult;

			if (logResults)
			{
				bool isOnePercentCheckpoint = (trial + 1) % (numTrials / 100) == 0;
				if (isOnePercentCheckpoint)
				{
					double runningAverage = cumulativeSum / (trial + 1);
					int percentComplete = (trial + 1) * 100 / numTrials + 1;
					Console.WriteLine($"Running Average at {percentComplete}% ({trial + 1} trials): {runningAverage:F2}");
				}
			}
		}

		return results;
	}

	static int Dice(int numRolls, int numFaces, DiceRollMode mode = DiceRollMode.Max)
	{
		Debug.Assert(numRolls > 0);
		Debug.Assert(numFaces > 0);

		int result = mode == DiceRollMode.Max ? 1 : numFaces;
		for (int i = 0; i < numRolls; i++)
		{
			int rollValue = rng.Next(1, numFaces + 1);
			result = mode switch
			{
				DiceRollMode.Min => Math.Min(result, rollValue),
				DiceRollMode.Max => Math.Max(result, rollValue),
				_ => throw new NotImplementedException(),
			};
		}
		return result;
	}

	static double Crazy8s()
	{
		const double costPlay = 1.0;
		const double bonusGameWin = 50.0;
		const double bonusPartial = 10.0;
		const int numWinsNeededPerReel = 8;
		const int inverseReelWinProbabilityLeft = 9;
		const int inverseReelWinProbabilityRight = 10;
		const int reelWinValue = 8;

		int reelWinCountLeft = 0;
		int reelWinCountRight = 0;
		int numBonusWins = 0;
		int numPlays = 0;

		while (true) 
		{
			numPlays++;

			int reelResultLeft = rng.Next(1, inverseReelWinProbabilityLeft + 1);
			int reelResultRight = rng.Next(1, inverseReelWinProbabilityRight + 1);
			bool isReelWinLeft = reelResultLeft == reelWinValue;
			bool isReelWinRight = reelResultRight == reelWinValue;

			if (isReelWinLeft)
			{
				reelWinCountLeft++;
			}

			if (isReelWinRight)
			{
				reelWinCountRight++;
			}

			bool gameIsOver = reelWinCountLeft >= numWinsNeededPerReel 
				&& reelWinCountRight >= numWinsNeededPerReel;
			if (gameIsOver)
			{
				break;
			}

			// This is after the "guard clause" so that bonus wins are only
			// counted on non-game-ending conditions.
			if (isReelWinLeft && reelWinCountLeft > numWinsNeededPerReel)
			{
				numBonusWins++;
			}
			if (isReelWinRight && reelWinCountRight > numWinsNeededPerReel)
			{
				numBonusWins++;
			}
		}

		double earnings = bonusPartial * numBonusWins + bonusGameWin - costPlay * numPlays;
		return earnings;
	}
}
