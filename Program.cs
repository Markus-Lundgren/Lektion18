using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Data.Sqlite;
class Program
{
	static void Main(string[] args)
	{
		using SqliteConnection db = new("Data Source = databaseloot.db");
		db.Open();

		using var command = db.CreateCommand();

		command.CommandText = """
			CREATE TABLE IF NOT EXISTS Loot(
				Id INTEGER PRIMARY KEY,
				Name TEXT,
				Value INTEGER,
				Description TEXT,
				Type TEXT
			)
		""";
		command.ExecuteNonQuery();

		Console.WriteLine("Välkommen till loot shop");
		while (true)
		{
			Console.WriteLine("Välj alternativ");
			Console.WriteLine("1. Lägg till i databasen");
			Console.WriteLine("2. Visa alla objekt");
			Console.WriteLine("3. Visa det dyraste objektet");
			Console.WriteLine("4. Visa alla objekt av en viss typ");

			if (!int.TryParse(Console.ReadLine(), out int choice))
			{
				Console.WriteLine("Du måste skriva ett val med en siffra!");
				continue;
			}

			choice = Math.Clamp(choice, 1, 4);

			using var dbCommand = db.CreateCommand();

			switch (choice)
			{
				case 1:
					Console.Write("Skriv namn: ");
					string name = Console.ReadLine();
					Console.Write("Skriv värde: ");
					int value = int.Parse(Console.ReadLine());
					Console.Write("Skriv en kort beskrivning: ");
					string description = Console.ReadLine();
					Console.Write("Skriv vilken typ av objekt detta är: ");
					string type = Console.ReadLine();

					dbCommand.CommandText = "INSERT INTO Loot(Name, Value, Description, Type) VALUES($name, $value, $description, $type)";
					dbCommand.Parameters.AddWithValue("$name", name);
					dbCommand.Parameters.AddWithValue("$value", value);
					dbCommand.Parameters.AddWithValue("$description", description);
					dbCommand.Parameters.AddWithValue("$type", type);

					dbCommand.ExecuteNonQuery();
					break;
				case 2:
					dbCommand.CommandText = "SELECT * FROM Loot";
					using (var reader = dbCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							Console.WriteLine($"Name: {reader.GetString(1)}, Value: {reader.GetInt32(2)}, Description: {reader.GetString(3)}, Type: {reader.GetString(4)}");
						}
					}
					break;
				case 3:
					dbCommand.CommandText = """
					SELECT * FROM Loot
					ORDER BY Value DESC
					LIMIT 1
					""";
					using (var reader = dbCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							Console.WriteLine($"Name: {reader.GetString(1)}, Value: {reader.GetInt32(2)}, Description: {reader.GetString(3)}, Type: {reader.GetString(4)}");
						}
					}
					break;
				case 4:
					Console.Write("Välj vilken typ av objekt: ");

					dbCommand.CommandText = "SELECT * FROM Loot WHERE Type = $type";
					dbCommand.Parameters.AddWithValue("$type", Console.ReadLine());

					using (var reader = dbCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							Console.WriteLine($"Name: {reader.GetString(1)}, Value: {reader.GetInt32(2)}, Description: {reader.GetString(3)}, Type: {reader.GetString(4)}");
						}
					}
					break;
			}
		}
	}
}

public class Loot(string name, int value, string description, string type)
{
	public string Name { get; private set; } = name;
	public int Value { get; private set; } = value;
	public string Description { get; private set; } = description;
	public string Type { get; private set; } = type;

}