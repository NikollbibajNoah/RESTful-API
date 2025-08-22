
-- Beispiel-Admin-User (Passwort muss noch Hash sein!)
INSERT INTO "AuthUsers" ("Username", "Email", "PasswordHash", "Role")
VALUES ('admin', 'admin@example.com', 'dummyhash', 'Admin')
    ON CONFLICT DO NOTHING;
