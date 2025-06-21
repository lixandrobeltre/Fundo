#!/bin/bash
# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to become available
echo "⏳ Waiting for SQL Server..."
for i in {1..30}; do
  /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -Q "SELECT 1;" -C && break
  echo "❌ SQL Server not ready yet, retrying..."
  sleep 2
done

# Run the SQL script
echo "⚙️ Running db.sql..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /init.sql -C
sleep 2 

# Load sample data
echo "database created..| seed data loading started.."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /sample_data.sql -C

wait
