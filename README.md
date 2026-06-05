# TaskManager

Aplicație CLI simplă de tip Task Manager scrisă în C# (.NET 8), construită pentru Punctul 3 al examenului — **Infrastructura tehnică a unui proiect**.

Aplicația permite adăugarea, listarea, finalizarea și ștergerea de task-uri cu prioritizare (Low / Medium / High / Critical) și sortare automată după prioritate și due date. Task-urile sunt **persistate pe disk** într-un fișier JSON, deci se păstrează între sesiuni.

## Persistență

Task-urile sunt salvate într-un fișier JSON situat la:

- **Windows:** `%APPDATA%\TaskManager\tasks.json`
- **Linux/macOS:** `~/.config/TaskManager/tasks.json`

Fișierul este actualizat automat după fiecare `add`, `done` sau `remove`. La pornire, aplicația încarcă task-urile existente și continuă numerotarea ID-urilor de unde a rămas.

## Structura proiectului

```
TaskManager/
├── src/
│   ├── TaskManager.Core/      # Biblioteca cu logica de business
│   │                          #   - TaskService, TodoTask, Priority
│   │                          #   - ITaskRepository, JsonTaskRepository, InMemoryTaskRepository
│   └── TaskManager.Cli/       # Aplicație CLI executabilă
├── tests/
│   └── TaskManager.Tests/     # Unit tests cu xUnit
├── .github/workflows/ci.yml   # GitHub Actions pipeline
├── .editorconfig              # Reguli stil cod
└── TaskManager.sln            # Soluția
```

## Mapare pe cerințele examenului

| Cerință | Implementare |
|---|---|
| **Source control** | Repository Git pe GitHub |
| **Unit testing** | Proiect `TaskManager.Tests` cu xUnit (18 teste, inclusiv pentru persistență) |
| **Continuous Integration** | GitHub Actions — `.github/workflows/ci.yml` |
| **Automatic build** | Pas `dotnet build` rulat la fiecare push / PR |
| **Automatic unit testing execution** | Pas `dotnet test` cu rezultate exportate în format TRX |
| **Static code analysis** | Roslyn analyzers (`EnableNETAnalyzers=true`, `AnalysisMode=Recommended`) + `dotnet format --verify-no-changes` |
| **Deployment (awareness + simple step)** | `dotnet publish` urmat de copierea artefactelor într-un folder `deployment/` cu timestamp + upload ca artifact |

## Cum se rulează local

```bash
# Restore + build
dotnet build

# Rulează testele
dotnet test

# Pornește aplicația CLI
dotnet run --project src/TaskManager.Cli
```

## Comenzi în CLI

```
add <titlu> [|Priority]   Adaugă task. Ex: add Fix CI |Critical
list                      Listează task-urile sortate după prioritate
done <id>                 Marchează task ca finalizat
remove <id>               Șterge task
help                      Ajutor
exit                      Ieșire
```

## CI/CD Pipeline

La fiecare push pe `main` sau pull request, GitHub Actions rulează automat:

1. Checkout cod
2. Setup .NET 8 SDK
3. `dotnet restore`
4. **Static analysis** — `dotnet format --verify-no-changes` (raportează probleme de stil)
5. **Build** — `dotnet build` în Release
6. **Test** — `dotnet test` cu raport TRX + colectare coverage
7. **Publish** — `dotnet publish` pentru CLI
8. **Deploy** — copiere artefacte într-un folder `deployment/taskman-<timestamp>/` + upload ca artifact

Status-ul rulărilor este vizibil în tab-ul **Actions** al repository-ului.

## Acces profesori

Pentru a primi acces la repository, adăugați colaboratorii din Settings → Collaborators (sau setați repo-ul ca public).
