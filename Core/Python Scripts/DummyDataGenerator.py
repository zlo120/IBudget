import json
import random
from datetime import datetime, timedelta

def generate_dummy_data():
    data = {
        "DateRange": ["2023-01-01T00:00:00", "2023-12-31T23:59:59"],
        "Tags": [
            {"ID": i, "Name": f"Tag_{i}"}
            for i in range(1, 21)  # Generating 20 tags for variety
        ],
        "incomeRecords": generate_income_records(),
        "expenseRecords": generate_expense_records()
    }
    return data

def generate_income_records():
    income_records = []
    for i in range(1, 1001):
        record = {
            "ID": i,
            "Category": f"{random.choice(['Salary', 'Freelance', 'Bonus', 'Investment'])}",
            "Date": generate_random_date(),
            "Amount": round(random.uniform(500, 5000), 2),
            "Frequency": random.randint(0, 4),
            "Source": f"{random.choice(['Job', 'Freelance Work', 'Investments', 'Part-time Work'])}"
        }
        income_records.append(record)
    return income_records

def generate_expense_records():
    expense_records = []
    for i in range(1, 1001):
        record = {
            "ID": i,
            "Category": f"{random.choice(['Groceries', 'Utilities', 'Entertainment', 'Health', 'Transportation'])}",
            "Date": generate_random_date(),
            "Amount": round(random.uniform(10, 200), 2),
            "Frequency": random.randint(0, 4),
            "Notes": f"{random.choice(['Weekly grocery shopping', 'Electricity bill', 'Movie night', 'Health insurance premium', 'Car fuel'])}",
            "TagIDs": random.sample(range(1, 21), random.randint(4, 5))  # Selecting 4-5 random TagIDs
        }
        expense_records.append(record)
    return expense_records

def generate_random_date():
    start_date = datetime(2023, 1, 1)
    end_date = datetime(2023, 12, 31)
    random_date = start_date + timedelta(
        seconds=random.randint(0, int((end_date - start_date).total_seconds()))
    )
    return random_date.isoformat()

if __name__ == "__main__":
    dummy_data = generate_dummy_data()
    with open("dummy_data.json", "w") as json_file:
        json.dump(dummy_data, json_file, indent=2)

    print("Dummy data generated and saved to dummy_data.json")
