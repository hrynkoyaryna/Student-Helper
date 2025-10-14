CREATE TYPE task_priority_enum AS ENUM ('low','medium','high');
CREATE TYPE event_type_enum AS ENUM ('personal','academic','online');

CREATE DOMAIN name_domain AS VARCHAR(100)
    CHECK (
        VALUE ~* '^[A-Za-zА-Яа-яЁёЇїІіЄєҐґ]+([\-'' ][A-Za-zА-Яа-яЁёЇїІіЄєҐґ]+)*$'
        AND LENGTH(VALUE) >= 2
    );
	
CREATE DOMAIN email_domain AS VARCHAR(255)
    CHECK (VALUE ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$');
	
CREATE DOMAIN phone_domain AS VARCHAR(13)
    CHECK (VALUE ~* '^(?:\+380\d{9}|0\d{9})$');

CREATE DOMAIN color_domain AS VARCHAR(20)
    CHECK (VALUE ~* '^#[0-9A-Fa-f]{6}$');

CREATE DOMAIN url_domain AS TEXT
    CHECK (VALUE ~* '^(http|https)://[A-Za-z0-9./?=_-]+$');
	
CREATE DOMAIN timezone_domain AS VARCHAR(50)
    CHECK (VALUE ~ '^[A-Za-z/_+-]+$');

CREATE DOMAIN user_status_domain AS VARCHAR(50)
    CHECK (VALUE IN ('active','inactive','banned'));

CREATE DOMAIN auth_provider_domain AS VARCHAR(50)
    CHECK (VALUE IN ('password','openid_connect','google','telegram'));

CREATE DOMAIN source_type_domain AS VARCHAR(50)
    CHECK (VALUE IN ('university_api','file','link','manual'));

CREATE DOMAIN task_status_domain AS VARCHAR(50)
    CHECK (VALUE IN ('pending','completed','overdue'));

CREATE DOMAIN integration_provider_domain AS VARCHAR(50)
    CHECK (VALUE IN ('google_calendar','telegram_bot'));
	
CREATE DOMAIN notification_channel_domain AS VARCHAR(50)
    CHECK (VALUE IN ('push','telegram'));

CREATE DOMAIN notification_status_domain AS VARCHAR(50)
    CHECK (VALUE IN ('pending','sent','failed'));
	
CREATE DOMAIN note_link_type_domain AS VARCHAR(50)
    CHECK (VALUE IN ('timetable_event','task','exam'));


CREATE TABLE "user" (
    id SERIAL PRIMARY KEY,
    first_name name_domain NOT NULL,
    last_name name_domain NOT NULL,
    email email_domain UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    status user_status_domain DEFAULT 'active',
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE auth_identity (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    provider auth_provider_domain NOT NULL,
    external_subject_id VARCHAR(255),
    external_email email_domain,
    last_login_at TIMESTAMP
);

CREATE TABLE password_reset_token (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    token VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    attempts INT DEFAULT 0
);

CREATE TABLE profile (
    user_id INT PRIMARY KEY REFERENCES "user"(id) ON DELETE CASCADE,
    avatar_url url_domain,
    locale VARCHAR(10) DEFAULT 'en',
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE group_academic (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL,
    faculty VARCHAR(100),
    degree VARCHAR(50),
    year INT
);

CREATE TABLE subject (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    short_name VARCHAR(50),
    description TEXT,
    default_color color_domain
);

CREATE TABLE lecturer (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(255) NOT NULL,
    email email_domain,
    phone phone_domain,
    notes TEXT
);

CREATE TABLE room (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    building VARCHAR(100),
    address TEXT
);

CREATE TABLE schedule_source (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    source_type source_type_domain,
    source_url url_domain,
    file_ref TEXT,
    last_sync_at TIMESTAMP,
    last_sync_status VARCHAR(50)
);

CREATE TABLE event (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    subject_id INT REFERENCES subject(id),
    title VARCHAR(255),
    lecturer_id INT REFERENCES lecturer(id),
    room_id INT REFERENCES room(id),
    start_at TIMESTAMP NOT NULL,
    end_at TIMESTAMP,
    type event_type_enum,
    description TEXT,
    recurrence_rule TEXT,
    recurrence_exceptions TEXT,
    source_id INT REFERENCES schedule_source(id),
    is_all_day BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    CHECK (end_at IS NULL OR end_at > start_at)
);

CREATE TABLE task (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    subject_id INT REFERENCES subject(id),
    title VARCHAR(255),
    description TEXT,
    due_at TIMESTAMP,
    status task_status_domain DEFAULT 'pending',
    priority task_priority_enum DEFAULT 'medium',
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    completed_at TIMESTAMP,
    CHECK (completed_at IS NULL OR (completed_at >= created_at AND (due_at IS NULL OR completed_at <= due_at)))
);

CREATE TABLE exam (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    subject_id INT REFERENCES subject(id),
    title VARCHAR(255),
    exam_date DATE,
    start_at TIME,
    end_at TIME,
    description TEXT
);

CREATE TABLE note (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    title VARCHAR(255),
    body TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    is_pinned BOOLEAN DEFAULT FALSE
);

CREATE TABLE note_link (
    id SERIAL PRIMARY KEY,
    note_id INT REFERENCES note(id) ON DELETE CASCADE,
    link_type note_link_type_domain,
    link_id INT
);

CREATE TABLE notification_setting (
    user_id INT PRIMARY KEY REFERENCES "user"(id),
    push_enabled BOOLEAN DEFAULT TRUE,
    remind_before_minutes INT[],
    telegram_connected BOOLEAN DEFAULT FALSE,
    timezone timezone_domain DEFAULT 'UTC'
);

CREATE TABLE integration (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    provider integration_provider_domain,
    oauth_access_token TEXT,
    oauth_refresh_token TEXT,
    oauth_expires_at TIMESTAMP,
    metadata JSONB,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE scheduled_notification (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    channel notification_channel_domain,
    entity_type VARCHAR(50),
    entity_id INT,
    fire_at TIMESTAMP,
    sent_at TIMESTAMP,
    status notification_status_domain DEFAULT 'pending',
    error_message TEXT
);

CREATE TABLE security_log (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "user"(id),
    event VARCHAR(100),
    created_at TIMESTAMP DEFAULT NOW(),
    ip_address INET,
    user_agent TEXT,
    metadata JSONB
);

CREATE TABLE app_log (
    id SERIAL PRIMARY KEY,
    level VARCHAR(20),
    message TEXT,
    context JSONB,
    created_at TIMESTAMP DEFAULT NOW()
);

CREATE INDEX idx_user_email ON "user"(email);
CREATE INDEX idx_event_user_start ON event(user_id, start_at);
CREATE INDEX idx_task_user_due ON task(user_id, due_at);
CREATE INDEX idx_exam_user_date ON exam(user_id, exam_date);

ALTER TABLE "user"
ADD COLUMN group_id INT REFERENCES group_academic(id);

CREATE UNIQUE INDEX unique_auth_identity ON auth_identity (user_id, provider);
CREATE UNIQUE INDEX unique_external_subject ON auth_identity (provider, external_subject_id)
WHERE external_subject_id IS NOT NULL;

ALTER TABLE password_reset_token ADD CONSTRAINT unique_token UNIQUE (token);

