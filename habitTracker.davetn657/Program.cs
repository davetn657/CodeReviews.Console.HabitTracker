using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habitTracker.davetn657.db";

List<HabitKeys> tables = new List<HabitKeys>();
string? tableName = string.Empty;
string? measurment = string.Empty;

tableName = "drinking_water";
measurment = "quantity";
GenerateTables();

tableName = "eating_meals";
measurment = "meals";
GenerateTables();

tableName = "jumping_jacks";
measurment = "reps";
GenerateTables();

WelcomeScreen();

void WelcomeScreen()
{
    bool validInput = false;

    while (!validInput)
    {
        Console.WriteLine("--------------------------------------------------\n");
        Console.WriteLine("Welcome to your Habit Trackers!");
        Console.WriteLine("--------------------------------------------------\n");

        Console.WriteLine("1. Choose Existing");
        Console.WriteLine("2. Create New");
        Console.Write("Enter here: ");


        switch (Console.ReadLine())
        {
            case "1":
                validInput = true;
                GetUserTables();
                break;
            case "2":
                CreateNewTable();
                break;
            default:
                Console.Clear();
                Console.WriteLine("Please enter a valid input!");
                break;
        }
    }
}

void GetUserTables()
{

    Console.WriteLine("--------------------------------------------------\n");
    Console.WriteLine("All Existing Habit Trackers!");
    Console.WriteLine("--------------------------------------------------\n");

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = @"SELECT name FROM sqlite_master
                                WHERE type = 'table';";
        
        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                if (reader.GetString(0) != "sqlite_sequence")
                {
                    var command = connection.CreateCommand();
                    command.CommandText = $"PRAGMA table_info({reader.GetString(0)})";
                    SqliteDataReader columnReader = command.ExecuteReader();

                    var habit = new HabitKeys();
                    habit.Name = reader.GetString(0);

                    while (columnReader.Read())
                    {
                        habit.Add(columnReader.GetString(1));
                    }

                    tables.Add(habit);

                    Console.WriteLine(reader.GetString(0));
                }
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("There are no existing habits to track! Please create a new habit");
            WelcomeScreen();
        }

        connection.Close();
    }

    GetUserChosenTable();
    UserInput();
}

void GetUserChosenTable()
{
    bool endLoop = false;

    while (!endLoop)
    {
        Console.Write("Enter which habit you want to edit: ");
        tableName = Console.ReadLine().Replace(' ', '_');

        foreach (var table in tables)
        {
            if (table.Name.ToLower() == tableName.ToLower())
            {
                Console.Clear();
                Console.WriteLine($"Opening {tableName}");

                tableName = table.Name.ToLower();
                measurment = table.Measurement;
                endLoop = true;
                break;
            }
        }

        if(!endLoop) Console.WriteLine("Enter a valid table name");
    }
}

void CreateNewTable()
{
    Console.WriteLine("--------------------------------------------------\n");
    Console.WriteLine("Create a New Habit!");
    Console.WriteLine("--------------------------------------------------\n");

    //let user choose what unit of measurement 
    while (true)
    {
        Console.Write("Enter a habit name: ");
        tableName = Console.ReadLine();

        if(tableName != string.Empty)
        {
            tableName = tableName.Replace(' ', '_');
            break;
        }
        else Console.WriteLine("Please enter a valid name!");
    }
    
    Console.WriteLine("--------------------------------------------------\n");
    while (true)
    {
        Console.Write("Enter a unit of measurment: ");
        measurment = Console.ReadLine();

        if (measurment != string.Empty) break;
        else Console.WriteLine("Please enter a valid measurement!");
    }

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        try
        {
            tableCmd.CommandText =
                @$"CREATE TABLE IF NOT EXISTS {tableName} (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                {measurment} INTEGER
                );";

            tableCmd.ExecuteNonQuery();
            Console.Clear();
        }
        catch (SqliteException)
        {
            Console.Clear();
            Console.WriteLine("Could not create table!\nThe table name or the unit of measurement may not be valid");
        }
        finally
        {
            connection.Close();
        }

    }

    
}

void UserInput()
{
    bool endApp = false;

    while(!endApp)
    {
        Console.WriteLine("--------------------------------------------------\n");
        Console.WriteLine($"Habit Tracker for {tableName}\n");

        Console.WriteLine("\nPick a option");
        Console.WriteLine("0. Exit Application");
        Console.WriteLine("1. View All Records");
        Console.WriteLine("2. Add New Record");
        Console.WriteLine("3. Remove Record");
        Console.WriteLine("4. Edit Record");
        Console.WriteLine("5. Report Summary");
        Console.WriteLine("6. Choose Different Habit");
        Console.Write("Enter here: ");

        string? input = Console.ReadLine();
        Console.Clear();

        switch (input)
        {
            case "0":
                endApp = true;
                break;
            case "1":
                ViewRecords();
                break;
            case "2":
                InsertRecord();
                break;
            case "3":
                DeleteRecord();
                break;
            case "4":
                UpdateRecord();
                break;
            case "5":
                GenerateSummaryReport();
                break;
            case "6":
                WelcomeScreen();
                break;
            default:
                Console.Clear();
                Console.WriteLine("Enter a valid input");
                break;

        }
    }
}

void ViewRecords()
{
    Console.Clear();

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"SELECT * FROM {tableName};";

        List<Habit> tableData = new List<Habit>();

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                tableData.Add(
                    new Habit
                    {
                        Id = reader.GetInt32(0),
                        Date = reader.GetString(1),
                        Quantity = reader.GetInt32(2)
                    }
                    );
            }

            int count = 1;
            Console.WriteLine("--------------------------------------------------\n");
            foreach (var data in tableData)
            {
                Console.WriteLine($"ID - {data.Id}, Date - {data.Date}, - {measurment}: {data.Quantity}");
                count++;
            }
            Console.WriteLine("--------------------------------------------------\n");
        }
        else
        {
            Console.WriteLine("No rows found");
        }

        connection.Close();
    }
}

void InsertRecord()
{
    string date = GetUserDate();
    string quantity = GetUserQuantity();

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.Parameters.Add("@date", SqliteType.Text).Value = date;
        tableCmd.Parameters.Add("@quantity", SqliteType.Text).Value = quantity;

        tableCmd.CommandText = $"INSERT INTO {tableName} (date, '{measurment}') VALUES(@date, @quantity)";

        tableCmd.ExecuteNonQuery();

        connection.Close();

        Console.Clear();
        Console.WriteLine($"Added data: ({date}, {quantity}) to database");
    }
}

void DeleteRecord()
{
    string? deleteInput = GetUserRecordID();

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.Parameters.Add("@record", SqliteType.Text).Value = deleteInput;

        tableCmd.CommandText = @$"DELETE from {tableName} 
                                  WHERE id is @record";

        tableCmd.ExecuteNonQuery();

        connection.Close();
    }
}

void UpdateRecord()
{
    string recordToChange = GetUserRecordID();
    string? dataChange = string.Empty;
    string columnToChange;

    Console.Clear();

    while (true)
    {
        Console.WriteLine("--------------------------------------------------\n");
        Console.WriteLine("What to edit.");
        Console.WriteLine("1. Date");
        Console.WriteLine("2. Quantity");
        Console.WriteLine("--------------------------------------------------\n");
        Console.Write("Enter here: ");
        string? editColumn = Console.ReadLine();

        if (editColumn == "1")
        {
            columnToChange = "date";
            dataChange = GetUserDate();
            break;
        }
        else if (editColumn == "2")
        {
            columnToChange = measurment;
            dataChange = GetUserQuantity();
            break;
        }
        Console.Clear();
        Console.WriteLine("Choose a valid input!");
    }

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.Parameters.Add("@recordtochange", SqliteType.Text).Value = recordToChange;
        tableCmd.Parameters.Add("@datachange", SqliteType.Text).Value = dataChange;
        tableCmd.Parameters.Add("@column", SqliteType.Text);

        tableCmd.CommandText = @$"UPDATE {tableName}
                                  SET {columnToChange} = @datachange
                                  WHERE id = @recordtochange;";

        tableCmd.ExecuteNonQuery();
        connection.Close();
    }
}

void GenerateSummaryReport()
{
    Console.WriteLine("--------------------------------------------------\n");
    Console.WriteLine("Yearly summary");
    Console.WriteLine("--------------------------------------------------\n");

    Console.WriteLine("Enter a year you want to view");
    string? date = Console.ReadLine();

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.Parameters.Add("@date", SqliteType.Text).Value = date;

        tableCmd.CommandText = $@"SELECT * FROM {tableName}
                                 WHERE strftime('%Y', date) = @date";

        int totalRecords = 0;
        int totalQuantity = 0;

        SqliteDataReader reader = tableCmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                totalRecords++;
                totalQuantity += Convert.ToInt32(reader.GetString(0));
            }
        }

        Console.WriteLine("--------------------------------------------------\n");
        Console.WriteLine("Total Records: " + totalRecords);
        Console.WriteLine($"Total {measurment}: {totalQuantity}");
        Console.WriteLine("--------------------------------------------------\n");

        connection.Close();
    }

    Console.Write("Press enter to return.");
    Console.ReadLine();
}

string GetUserDate()
{
    string? date;
    DateTime formatChecker = DateTime.Today;

    while (true)
    {
        Console.Write("Enter a date (yyyy-mm-dd): ");
        date = Console.ReadLine();
        if (DateTime.TryParse(date, out formatChecker))
        {
            date = formatChecker.ToString("yyyy-MM-dd");
            break;
        }
        else if (date.ToLower() == "today")
        {
            date = formatChecker.ToString("yyyy-MM-dd");
            break;
        }
        
        Console.WriteLine("Please enter a valid date");
    }

    return date;
}

string GetUserQuantity()
{
    int quantity;

    while (true)
    {
        Console.Write("Enter a quantity: ");
        if (Int32.TryParse(Console.ReadLine(), out quantity)) break;
        Console.WriteLine("Please enter a valid quantity");
    }

    return quantity.ToString();
}

string GetUserRecordID()
{
    bool idFound = false;
    string? id = string.Empty;

    while (!idFound)
    {
        ViewRecords();
        Console.Write("Choose the id of the record to delete/edit: ");
        id = Console.ReadLine();

        using(var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            tableCmd.CommandText = @$"SELECT count(*) FROM {tableName} 
                                     WHERE id = {id}";

            int count = Convert.ToInt32(tableCmd.ExecuteScalar());

            if (count == 1) idFound = true;
            connection.Close();
        }
    }

    return id;
}

void GenerateTables()
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();
        try
        {
            tableCmd.CommandText =
                @$"CREATE TABLE IF NOT EXISTS {tableName} (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                {measurment} INTEGER
                );";

            tableCmd.ExecuteNonQuery();
            GenerateData();
            Console.Clear();
        }
        catch (SqliteException)
        {
            Console.Clear();
            Console.WriteLine("Could not create table!\nThe table name or the unit of measurement may not be valid");
        }
        finally
        {
            connection.Close();
        }

    }
}

void GenerateData()
{
    Random random = new Random();
    DateTime date = DateTime.Today.AddDays(-10);
    bool isEmpty = false;

    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();
        var tableCmd = connection.CreateCommand();

        tableCmd.CommandText = $"SELECT * FROM {tableName}";

        SqliteDataReader reader = tableCmd.ExecuteReader();
        if (!reader.HasRows) isEmpty = true;

        connection.Close();
    }

    if(isEmpty)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();

            while (date != DateTime.Today)
            {
                tableCmd.CommandText = $@"INSERT INTO {tableName} (date, {measurment}) 
                                    VALUES('{date.ToString("yyyy-MM-dd")}', '{random.Next(0, 100)}');";

                tableCmd.ExecuteNonQuery();

                date = date.AddDays(1);
            }

            connection.Close();
        }
    }

}