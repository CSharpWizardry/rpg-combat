# rpg-combat

Back-end API for a Combat game of RPG Characters.

## Instructions to run

Please ensure .net core is installed. Then download the source-code and in a terminal inside the _rpg_combat_ project folder run:  
`dotnet build`    
`dotnet run` 

access `http://localhost:5000/swagger/index.html` to se a description of available commands or use the provided PostmanCollection inside the project.

## To work on the project

This project uses EntityFrameworkCore and migrations (currently using Sqlite as database), make sure you have it installed:  
`dotnet tool install --global dotnet-ef`