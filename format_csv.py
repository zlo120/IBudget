import csv
import re
import os

if __name__ == "__main__":
    csv_file = input("Please input csv file: ")
    csv_file = csv_file.replace("\"", "")
    rows = [['Date','Amount','Description']]
    with open(csv_file, 'r', newline='') as infile:
        reader = csv.reader(infile)
        for row in reader:
            if row[-1] == "":
                row = row[:-1]
            description = row[2]
            cleaned_description = re.sub(r'\s+', ' ', description)
            row[2] = cleaned_description
            rows.append(row)
        
    directory, filename = os.path.split(csv_file)
    name, extension = os.path.splitext(filename)
    new_filename = f"{name}_CLEANED{extension}"
    new_path = os.path.join(directory, new_filename)
    with open(new_path, 'w', newline='') as outfile:
        writer = csv.writer(outfile)
        writer.writerows(rows)