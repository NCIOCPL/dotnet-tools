@echo off
powershell -ExecutionPolicy Unrestricted -file %1\hooks\pre-commit.ps1 %1 %2
