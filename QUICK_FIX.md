# ?? QUICK FIX - Copy/Paste Ready

## The Problem ?
```
MSBUILD : error MSB1011: Specify which project or solution file
```

## The Fix ?
Always add `onvatenter.sln` to commands!

---

## Copy & Paste These Commands

### Test Everything (Windows CMD)
```batch
cd C:\Users\rafae\Source\Repos\oop-s2-2-mvc-83331
dotnet clean onvatenter.sln
dotnet restore onvatenter.sln
dotnet build onvatenter.sln
dotnet test onvatenter.sln
```

### Test Everything (PowerShell)
```powershell
cd C:\Users\rafae\Source\Repos\oop-s2-2-mvc-83331
dotnet clean onvatenter.sln
dotnet restore onvatenter.sln
dotnet build onvatenter.sln
dotnet test onvatenter.sln
```

### Quick Test
```bash
cd C:\Users\rafae\Source\Repos\oop-s2-2-mvc-83331
dotnet test onvatenter.sln
```

### Push to GitHub
```bash
git add .
git commit -m "fix: Add explicit solution file path to build commands"
git push origin main
```

---

## ? What to Expect

After running tests:
```
Récapitulatif du test : total : 9; échec : 0; réussi : 9
```

? = All good!  
? = Check console for errors

---

## ?? That's It!

You're done! ??

Tests will run on GitHub Actions automatically after push.
