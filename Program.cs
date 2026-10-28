using Microsoft.Data.Sqlite;
class Program
{
	static void Main(string[] args)
	{
		using SqliteConnection db = new("Data Source = database.db");
		db.Open();

		using SqliteCommand command = db.CreateCommand();
		command.CommandText =
		"""
		CREATE TABLE IF NOT EXISTS People(
			Id INTEGER PRIMARY KEY,
			Name TEXT NOT NULL,
			Age INTEGER NOT NULL
		);
		
		""";
		command.ExecuteNonQuery();

		Console.WriteLine("Skapa en person");
		Console.Write("Namn: ");
		string pName = Console.ReadLine();
		Console.Write("Age: ");
		int pAge = int.Parse(Console.ReadLine());

		using var insert = db.CreateCommand();
		insert.CommandText = "INSERT INTO People(Name, Age) VALUES ($pName, $pAge)";
		insert.Parameters.AddWithValue("$pName", pName);
		insert.Parameters.AddWithValue("$pAge", pAge);

		insert.ExecuteNonQuery();

		while (true)
		{
			Console.Write("Välj vem du vill hitta information om: ");
			string search = Console.ReadLine();

			using var selectPeople = db.CreateCommand();
			selectPeople.CommandText = "SELECT Id, Name, Age FROM People WHERE Name = $name";
			selectPeople.Parameters.AddWithValue("$name", search);

			using var reader = selectPeople.ExecuteReader();

			while (reader.Read())
			{
				int id = reader.GetInt32(0);
				string name = reader.GetString(1);
				int age = reader.GetInt32(2);
				Console.WriteLine("-----------------------------------");
				Console.WriteLine($"Id: {id}, Name: {name}, Age: {age}");
			}
		}
	}
}

class Person(string name, int age)
{
	public string Name { get; set; } = name;
	public int Age { get; set; } = age;
}
