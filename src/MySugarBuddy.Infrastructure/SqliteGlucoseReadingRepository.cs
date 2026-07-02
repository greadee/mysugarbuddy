using System.Globalization;
using Microsoft.Data.Sqlite;
using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Infrastructure;

public class SqliteGlucoseReadingRepository : IGlucoseReadingRepository
{
    private const string DateTimeFormat = "O";
    private readonly string _databasePath;

    public SqliteGlucoseReadingRepository(string databasePath)
    {
        _databasePath = databasePath;
    }

    public void SaveReadings(IReadOnlyList<GlucoseReading> readings)
    {
        EnsureDatabaseFolderExists();

        using var connection = OpenConnection();
        EnsureSchema(connection);

        using var transaction = connection.BeginTransaction();
        foreach (var reading in readings)
        {
            SaveReading(connection, transaction, reading);
        }

        transaction.Commit();
    }

    public IReadOnlyList<GlucoseReading> LoadReadings()
    {
        if (!File.Exists(_databasePath))
        {
            return Array.Empty<GlucoseReading>();
        }

        using var connection = OpenConnection();
        EnsureSchema(connection);

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT value_mg_per_dl, recorded_at
            FROM glucose_readings
            ORDER BY id;
            """;

        using var reader = command.ExecuteReader();
        var readings = new List<GlucoseReading>();

        while (reader.Read())
        {
            var valueMgPerDl = reader.GetInt32(0);
            var recordedAt = DateTime.Parse(reader.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            readings.Add(new GlucoseReading(valueMgPerDl, recordedAt));
        }

        return readings;
    }

    private SqliteConnection OpenConnection()
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = _databasePath
        }.ToString();

        var connection = new SqliteConnection(connectionString);
        connection.Open();

        return connection;
    }

    private static void EnsureSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS glucose_readings (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                value_mg_per_dl INTEGER NOT NULL,
                recorded_at TEXT NOT NULL
            );
        """;
        command.ExecuteNonQuery();
    }

    private static void SaveReading(SqliteConnection connection, SqliteTransaction transaction, GlucoseReading reading)
    {
        var recordedAt = reading.RecordedAt.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
        var existingId = FindReadingId(connection, transaction, recordedAt);

        if (existingId is null)
        {
            using var insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = """
                INSERT INTO glucose_readings (value_mg_per_dl, recorded_at)
                VALUES ($value_mg_per_dl, $recorded_at);
                """;
            insertCommand.Parameters.AddWithValue("$value_mg_per_dl", reading.ValueMgPerDl);
            insertCommand.Parameters.AddWithValue("$recorded_at", recordedAt);
            insertCommand.ExecuteNonQuery();

            return;
        }

        using var updateCommand = connection.CreateCommand();
        updateCommand.Transaction = transaction;
        updateCommand.CommandText = """
            UPDATE glucose_readings
            SET value_mg_per_dl = $value_mg_per_dl
            WHERE id = $id;
            """;
        updateCommand.Parameters.AddWithValue("$value_mg_per_dl", reading.ValueMgPerDl);
        updateCommand.Parameters.AddWithValue("$id", existingId.Value);
        updateCommand.ExecuteNonQuery();
    }

    private static long? FindReadingId(SqliteConnection connection, SqliteTransaction transaction, string recordedAt)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            SELECT id
            FROM glucose_readings
            WHERE recorded_at = $recorded_at
            ORDER BY id
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$recorded_at", recordedAt);

        var result = command.ExecuteScalar();
        return result is null ? null : Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private void EnsureDatabaseFolderExists()
    {
        var folderPath = Path.GetDirectoryName(_databasePath);

        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
