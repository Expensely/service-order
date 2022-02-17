# MultiCarrier

## Application
### Setup
Install the dotnet tools that have been defined in [dotnet-tools.json](./config/dotnet-tools.json).
```bash
dotnet tool restore
```
Any dotnet tools used in the development of this application must be specified in [dotnet-tools.json](./config/dotnet-tools.json).

### Migrations
#### Add migration
Run the migration from the `src` folder
```bash
cd src
dotnet ef migrations add { migration name } --startup-project ./MultiCarrier.Database.Migrator --project ./MultiCarrier.Database
```
