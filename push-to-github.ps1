# ========================================
# Script de push vers GitHub - Rafael Boulan
# ========================================
# Lance simplement ce script avec F5 dans PowerShell
# Tu seras peut-être demandé de te connecter à GitHub
# ========================================

$GITHUB_USERNAME = "rafaelboulan"
$REPO_NAME = "oop-s2-2-mvc-83331"

Write-Host "🚀 Initialisation du repo Git..."
git init

Write-Host "📝 Configuration Git..."
git config user.name "Rafael Boulan"
git config user.email "rafael.boulan@example.com"

Write-Host "📦 Ajout de tous les fichiers..."
git add .

Write-Host "💾 Premier commit..."
git commit -m "Initial commit - Food Safety Inspection Tracker - ASP.NET Core MVC"

Write-Host "🔗 Connexion à GitHub..."
git remote add origin "https://github.com/$GITHUB_USERNAME/$REPO_NAME.git"

Write-Host "🌿 Renommage branche..."
git branch -M main

Write-Host "📤 Push vers GitHub (tu seras peut-être demandé de te connecter)..."
git push -u origin main

Write-Host "✅ Terminé! Ton code est sur GitHub: https://github.com/$GITHUB_USERNAME/$REPO_NAME"
