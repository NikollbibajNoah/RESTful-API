
-- Beispiel-Admin-User
-- password hash in original: admin12345
INSERT INTO "AuthUsers" ("Username", "Email", "PasswordHash", "Role")
VALUES ('admin', 'admin@example.com', 'AQAAAAIAAYagAAAAEPHkMlxmavXRizYiRFgb7pSSSsZo3bPiGJuIR+K4wyTzme6W0SzJH360mhzWjB/HyA==', 'Admin')
    ON CONFLICT DO NOTHING;
