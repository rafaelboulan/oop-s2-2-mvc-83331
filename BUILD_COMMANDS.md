# ?? Build & Test Commands - CORRECT SYNTAX

## ? PROBLÈME RÉSOLU

**Erreur:** `MSB1011: Specify which project or solution file`

**Cause:** Utiliser `dotnet restore` sans paramètre quand plusieurs `.csproj` existent

**Solution:** Toujours ajouter `onvatenter.sln`

---

## ? COMMANDES CORRECTES

### Restore
```bash
dotnet restore onvatenter.sln
```

### Build (Debug)
```bash
dotnet build onvatenter.sln
```

### Build (Release)
```bash
dotnet build onvatenter.sln --configuration Release
```

### Run Tests
```bash
dotnet test onvatenter.sln
```

### Clean
```bash
dotnet clean onvatenter.sln
```

### Full Cycle
```bash
dotnet clean onvatenter.sln
dotnet restore onvatenter.sln
dotnet build onvatenter.sln
dotnet test onvatenter.sln
```

---

## ?? Batch Script (Updated)

**build-and-test.bat:**
```batch
@echo off
dotnet clean onvatenter.sln
dotnet restore onvatenter.sln
dotnet build onvatenter.sln
dotnet test onvatenter.sln --configuration Release
```

**build-and-test.ps1:**
```powershell
dotnet clean onvatenter.sln
dotnet restore onvatenter.sln
dotnet build onvatenter.sln
dotnet test onvatenter.sln --configuration Release
```

---

## ? EXPECTED OUTPUT

```
Récapitulatif du test : total : 9; échec : 0; réussi : 9
```

? All 9 tests passing!

---

## ?? NOW RUN TESTS

```bash
cd C:\Users\rafae\Source\Repos\oop-s2-2-mvc-83331
dotnet test onvatenter.sln
```

Then commit and push:
```bash
git add .
git commit -m "feat: Fix build by using explicit solution file path"
git push origin main
```

Done! ??
