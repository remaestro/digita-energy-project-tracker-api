-- Script pour réinitialiser les utilisateurs
-- À exécuter dans SQL Server Management Studio ou via sqlcmd

USE DigitaEnergy_ProjectTracker;
GO

-- Supprimer tous les utilisateurs existants
DELETE FROM Users;
GO

-- Réinitialiser l'identité si nécessaire
-- DBCC CHECKIDENT ('Users', RESEED, 0);
-- GO

PRINT 'Utilisateurs supprimés. Redémarrez l''API pour recréer les utilisateurs avec les nouveaux mots de passe.';
