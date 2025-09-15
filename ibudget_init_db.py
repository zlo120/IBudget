import os
from pathlib import Path
import shutil
from datetime import datetime
import pymongo
from bson.json_util import dumps
from pymongo.collection import Collection
import time

# global constants
APPDATA_DIR = os.getenv('LOCALAPPDATA')
ROOT_DIR_NAME = "IBudget"
DB_DIR_NAME = "IBudgetDB"
BACKUP_DB_DIR_NAME = "Backup"
DB_FILE_NAME = "IBudget.db"
PROJECT_SLN_FILE_NAME = "IBudget.sln"
INFRASTRUCTURE_PROJECT_NAME = "IBudget.Infrastructure"
MIGRATIONS_FOLDER_NAME = "Migrations"
DB_DIR = f"{APPDATA_DIR}\\{ROOT_DIR_NAME}\\{DB_DIR_NAME}"
BACKUP_DIR = f"{APPDATA_DIR}\\{ROOT_DIR_NAME}\\{BACKUP_DB_DIR_NAME}"
MONGO_DB_NAME = "IBudget"
COLLECTION_NAME = "userDictionaries"

# UTILITY FUNCTIONS
class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

def log_error(msg: str) -> None:
    print(f"{bcolors.WARNING}[ERROR]{bcolors.ENDC}: {msg}")

def log_info(msg: str) -> None:
    print(f"{bcolors.OKBLUE}[INFO]{bcolors.ENDC}: {msg}")

def log_success(msg: str) -> None:
    print(f"{bcolors.OKGREEN}[SUCCESS]{bcolors.ENDC}: {msg}")

def get_input(msg: str) -> str:
    return input(f"{bcolors.BOLD}[USER INPUT]{bcolors.ENDC} {msg}: ")

def exit_application(error_code: int) -> None:
    input("\nPress enter to close this application... ")
    exit(error_code)

# PROCESSING FUNCTIONS

# SQLITE SECTION
def setup_appdata_dir() -> None:
    log_info("Setting up appdata dir")
    if not os.path.exists(APPDATA_DIR + f"\\{ROOT_DIR_NAME}"):
        log_info(f"The directory {APPDATA_DIR}\\{ROOT_DIR_NAME} does not exist. Creating necessary directories now...")
        os.makedirs(DB_DIR)
        os.makedirs(BACKUP_DIR)    
    if not os.path.exists(DB_DIR):
        log_info(f"The directory {DB_DIR} does not exist. Creating necessary directories now...")
        os.makedirs(DB_DIR)
    if not os.path.exists(BACKUP_DIR):
        log_info(f"The directory {BACKUP_DIR} does not exist. Creating necessary directories now...")
        os.makedirs(BACKUP_DIR)    
    if not os.path.exists(DB_DIR) or not os.path.exists(BACKUP_DIR):
        log_error("Appdata directories were not created properly...")
        exit_application(1)
    log_success("Appdata directories validation completed")

def setup_db_file() -> None:
    log_info("Trying to locate db file")
    path_to_file = f"{APPDATA_DIR}\\{ROOT_DIR_NAME}\\{DB_DIR_NAME}\\{DB_FILE_NAME}"
    db_file = Path(path_to_file)
    if db_file.is_file():
        # file exists
        filename, file_extension = os.path.splitext(DB_FILE_NAME)
        timestamp = datetime.now().strftime("%d_%m_%Y__%H_%M_%S")
        new_filename = f"{filename}_{timestamp}{file_extension}"
        path_to_moved_file = f"{APPDATA_DIR}\\{ROOT_DIR_NAME}\\{BACKUP_DB_DIR_NAME}\\{new_filename}"
        log_info("An existing db file is present, we will move this into the backup folder")
        
        while True:
            try:
                shutil.move(path_to_file, path_to_moved_file)
                break
            except PermissionError:
                log_error("Cannot access the database file! You may have it open currently. Please close the file, we will wait until we can access it...")
                time.sleep(3)

    else:
        log_info("File was not found, creating db file now...")
    file = open(path_to_file, "x")
    file.close()

    if db_file.is_file():
        log_success("The db file was created")
    else:
        log_error("Db file was not created. Now exiting...")
        exit_application(1)

def validate_project_dir(dir: str) -> bool:
    curr_dir = os.path.dirname(os.path.realpath(__file__))
    if not os.path.exists(dir):
        log_error(f"The directory {dir} does not exist")
        return False
    log_success(f"Found the directory {dir}")
    log_info(f"Looking for the solution file {PROJECT_SLN_FILE_NAME}...")
    os.chdir(dir)
    solution_file = Path(PROJECT_SLN_FILE_NAME)
    if solution_file.is_file():
        log_success(f"Found the solution file {PROJECT_SLN_FILE_NAME}")
        os.chdir(curr_dir)
        return True
    else:
        log_error("Could not find the solution file, perhaps this directory is not the project directory?")
        os.chdir(curr_dir)
        return False

def setup_migrations() -> None:
    migrations_dir = f"{INFRASTRUCTURE_PROJECT_NAME}\\{MIGRATIONS_FOLDER_NAME}"
    if os.path.exists(migrations_dir):
        log_info("Removing existing migrations first")
        shutil.rmtree(migrations_dir)
    log_info("Executing the migrations command...")
    os.system("dotnet-ef migrations add MyMigration --context Context --project IBudget.Infrastructure --startup-project IBudget.ConsoleUI")

    if os.path.exists(migrations_dir):
        log_success("Migrations was successfully created")
    else:
        exit_application(1)

# Main function for setting up sqlite db
def setup_sqlite() -> None:
    setup_appdata_dir()
    setup_db_file()
    solution_file = Path(PROJECT_SLN_FILE_NAME)
    if solution_file.is_file():
        log_info("This appears to be the directory for the IBudget backend")
    else:
        project_dir = ""
        while(True):
            user_input = get_input("Please input your working directory of IBudget")
            if user_input == "":
                exit_application(0)

            if validate_project_dir(user_input):
                project_dir = user_input
                break

        if project_dir == "":
            log_error("There was an error getting project dir, closing application...")
            exit_application(1)

        os.chdir(project_dir)
        
    setup_migrations()
    log_info("Executing the database update command...")
    os.system("dotnet-ef database update --context Context --project IBudget.Infrastructure  --startup-project IBudget.ConsoleUI")

# MONGODB SECTION

def create_backup(collection: Collection) -> None:
    timestamp = datetime.now().strftime("%d_%m_%Y__%H_%M_%S") 
    filename = f"mongo_backup_{timestamp}.json"
    dir = f"{BACKUP_DIR}\\{filename}"
    document = collection.find_one({"userId": 1})
    json_data = dumps(document, indent=2)
    with open(dir, 'w') as backup_file:
        backup_file.write(json_data)

# Main function for setting up mongodb
def setup_mongodb() -> None:
    client = pymongo.MongoClient("mongodb://localhost:27017/")
    db_list = client.list_database_names()
    if MONGO_DB_NAME in db_list:
        log_info(f"The db {MONGO_DB_NAME} exists")   
        ibudget_db = client[MONGO_DB_NAME]     
        if COLLECTION_NAME in ibudget_db.list_collection_names():
            log_info(f"The collection {COLLECTION_NAME} exists, we will be resetting the collection now...")
            create_backup(ibudget_db[COLLECTION_NAME]) 
            client.drop_database(MONGO_DB_NAME)       
        else:
            log_info(f"The collection {COLLECTION_NAME} does not exist. Creating now...")
            user_dictionaries_collection = ibudget_db[COLLECTION_NAME]
    else:
        log_info(f"The db {MONGO_DB_NAME} does not exist. Creating now...")

    ibudget_db = client[MONGO_DB_NAME]
    user_dictionaries_collection = ibudget_db[COLLECTION_NAME]
    default_user = { 
        "userId": 1,
        "ExpenseDictionaries": [], 
        "RuleDictionaries": [],
        "BatchHashes": []
    }
    user_dictionaries_collection.insert_one(default_user)
    
    if MONGO_DB_NAME in client.list_database_names() and COLLECTION_NAME in ibudget_db.list_collection_names():
        log_success("MongoDb is now set up")
    else:
        log_error("MongoDb was not set up correctly...")
        exit_application(1)

# MAIN APPLICATION
if __name__ == "__main__":
    setup_sqlite()
    setup_mongodb()
    # end of application
    exit_application(0)