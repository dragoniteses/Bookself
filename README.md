# CustomerApi

A simple ASP.NET Core Web API for customer registration.

Contents:
- `CustomerApi/` - project
- `DB/` - local SQLite DB (ignored by .gitignore)

Quick publish to GitHub

1) Create repository on GitHub (web) or use GitHub CLI:

   gh auth login
   gh repo create <your-username>/<repo-name> --public --source=. --remote=origin --push

2) Or manually with git:

   git init
   git add .
   git commit -m "Initial commit"
   git branch -M main
   git remote add origin https://github.com/<your-username>/<repo-name>.git
   git push -u origin main

Notes:
- The `DB/` folder and `*.db` files are ignored by `.gitignore` to avoid committing local databases.
- If you already committed DB files, remove them and commit:

   git rm --cached DB/customerinfo.db
   git commit -m "Remove local DB from repo"
   git push

If you want, I can create a GitHub Actions workflow to build and run tests on push.
